using Market.Application;
using Market.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Market.Infrastructure.BackgroundJobs;

/// <summary>
/// POZADINSKI PROCES (BackgroundService) – scheduler za periodične zadatke.
/// Pokreće se zajedno s API-jem i svakih 15 s provjeri koji zadatak je "na redu":
///   1. ExpireAnnouncements – deaktivira vijesti kojima je istekao termin prikaza
///   2. ReleasePreOrders    – na dan izlaska igre pretvori pre-order u plaćenu narudžbu,
///                            izda licencne ključeve i pošalje korisniku obavještenje
/// Stanje i historija izvršavanja se bilježe u BackgroundJobMonitor (admin stranica "Pozadinski procesi").
/// </summary>
public class ScheduledTasksService : BackgroundService
{
    public const string ExpireAnnouncementsJob = "ExpireAnnouncements";
    public const string ReleasePreOrdersJob = "ReleasePreOrders";

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly BackgroundJobMonitor _monitor;
    private readonly ILogger<ScheduledTasksService> _logger;

    // Isti zadatak se ne smije izvršavati dvaput istovremeno (npr. ručno + automatski)
    private readonly Dictionary<string, SemaphoreSlim> _locks = new()
    {
        [ExpireAnnouncementsJob] = new SemaphoreSlim(1, 1),
        [ReleasePreOrdersJob] = new SemaphoreSlim(1, 1),
    };

    private readonly Dictionary<string, (int intervalSeconds, Func<LoadoutDbContext, Task<string>> action)> _jobs;
    private readonly Dictionary<string, DateTime> _nextRun = new();

    public ScheduledTasksService(IServiceScopeFactory scopeFactory, BackgroundJobMonitor monitor, ILogger<ScheduledTasksService> logger)
    {
        _scopeFactory = scopeFactory;
        _monitor = monitor;
        _logger = logger;

        _jobs = new()
        {
            [ExpireAnnouncementsJob] = (60, ExpireAnnouncementsAsync),
            [ReleasePreOrdersJob] = (60, ReleasePreOrdersAsync),
        };

        _monitor.Register(ExpireAnnouncementsJob, "Deaktivira vijesti kojima je istekao termin prikaza.", 60);
        _monitor.Register(ReleasePreOrdersJob, "Na dan izlaska igre izdaje ključeve za pre-ordere i obavještava kupce.", 60);
        foreach (var name in _jobs.Keys) _nextRun[name] = DateTime.Now.AddSeconds(5);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Scheduler started.");
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(15));

        do
        {
            foreach (var name in _jobs.Keys)
            {
                if (DateTime.Now >= _nextRun[name])
                {
                    await RunJobAsync(name, "Scheduler", stoppingToken);
                    _nextRun[name] = DateTime.Now.AddSeconds(_jobs[name].intervalSeconds);
                }
            }
        }
        while (await timer.WaitForNextTickAsync(stoppingToken));
    }

    /// <summary>Pokreće zadatak (poziva ga scheduler ili admin preko "Pokreni sada").</summary>
    public async Task<bool> RunJobAsync(string name, string trigger, CancellationToken ct = default)
    {
        if (!_jobs.TryGetValue(name, out var job)) return false;

        var gate = _locks[name];
        if (!await gate.WaitAsync(0, ct)) return true; // već radi -> preskoči

        var started = DateTime.Now;
        _monitor.MarkStarted(name);
        try
        {
            // BackgroundService je singleton, a DbContext je scoped -> svako izvršavanje dobija svoj scope
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<LoadoutDbContext>();
            var result = await job.action(db);
            _monitor.MarkFinished(name, started, true, result, trigger);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Background job {Job} failed", name);
            _monitor.MarkFinished(name, started, false, ex.Message, trigger);
        }
        finally
        {
            gate.Release();
        }
        return true;
    }

    // ---------------- ZADACI ----------------

    private static async Task<string> ExpireAnnouncementsAsync(LoadoutDbContext db)
    {
        var now = DateTime.Now;
        var expired = await db.Notifications
            .Where(n => n.UserId == null && n.IsActive && n.ExpiresAt != null && n.ExpiresAt <= now)
            .ToListAsync();

        foreach (var n in expired) n.IsActive = false;
        await db.SaveChangesAsync();

        return expired.Count == 0 ? "Nema isteklih vijesti." : $"Deaktivirano vijesti: {expired.Count}.";
    }

    private static async Task<string> ReleasePreOrdersAsync(LoadoutDbContext db)
    {
        var today = DateOnly.FromDateTime(DateTime.Now);

        // Pre-order narudžbe (status 4) čije su SVE igre izašle
        var orders = await db.Orders
            .Include(o => o.OrderGames).ThenInclude(og => og.Game)
            .Where(o => o.StatusId == 4 && o.OrderGames.All(og => og.Game.ReleaseDate <= today))
            .ToListAsync();

        int keys = 0;
        foreach (var order in orders)
        {
            foreach (var og in order.OrderGames)
            {
                for (int i = 0; i < og.Quantity; i++)
                {
                    db.Licenses.Add(new License
                    {
                        UserId = order.UserId,
                        GameId = og.GameId,
                        LicenseKey = Guid.NewGuid().ToString("N").ToUpper()[..16],
                        IssueDate = DateTime.Now,
                        ExpiryDate = DateTime.Now.AddYears(10)
                    });
                    keys++;
                }
            }

            order.StatusId = 1; // pre-order -> plaćeno (ključevi izdani)

            // Lično obavještenje kupcu
            var titles = string.Join(", ", order.OrderGames.Select(og => og.Game.Title));
            db.Notifications.Add(new Notification
            {
                UserId = order.UserId,
                Title = "Tvoj pre-order je stigao!",
                Message = $"{titles} je izašla. Licencni ključ te čeka u 'Moje kupovine'.",
                Type = "Info",
                IsRead = false,
                IsActive = true,
                CreatedAt = DateTime.Now
            });
        }

        await db.SaveChangesAsync();
        return orders.Count == 0
            ? "Nema pre-ordera za izdavanje."
            : $"Izdano narudžbi: {orders.Count}, ključeva: {keys}.";
    }
}
