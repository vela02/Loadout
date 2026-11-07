namespace Market.Infrastructure.BackgroundJobs;

/// <summary>Jedno izvršavanje pozadinskog zadatka (za historiju na admin stranici).</summary>
public class JobRunInfo
{
    public string JobName { get; set; } = "";
    public DateTime StartedAt { get; set; }
    public double DurationMs { get; set; }
    public bool Success { get; set; }
    public string Result { get; set; } = "";
    public string Trigger { get; set; } = "Scheduler";   // Scheduler | Manual
}

/// <summary>Trenutno stanje jednog pozadinskog zadatka.</summary>
public class JobStatusInfo
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public int IntervalSeconds { get; set; }
    public bool IsRunning { get; set; }
    public DateTime? LastRunAt { get; set; }
    public DateTime? NextRunAt { get; set; }
    public bool? LastSuccess { get; set; }
    public string? LastResult { get; set; }
    public int TotalRuns { get; set; }
    public int FailedRuns { get; set; }
}

/// <summary>
/// "Uvid u stanje background procesa": singleton koji pamti status i historiju izvršavanja.
/// Podaci su u memoriji (resetuju se restartom servera) – za admin pregled je to dovoljno.
/// </summary>
public class BackgroundJobMonitor
{
    private readonly object _lock = new();
    private readonly Dictionary<string, JobStatusInfo> _jobs = new();
    private readonly LinkedList<JobRunInfo> _history = new();
    private const int MaxHistory = 50;

    public void Register(string name, string description, int intervalSeconds)
    {
        lock (_lock)
        {
            _jobs[name] = new JobStatusInfo
            {
                Name = name,
                Description = description,
                IntervalSeconds = intervalSeconds,
                NextRunAt = DateTime.Now.AddSeconds(5)
            };
        }
    }

    public void MarkStarted(string name)
    {
        lock (_lock) { if (_jobs.TryGetValue(name, out var j)) j.IsRunning = true; }
    }

    public void MarkFinished(string name, DateTime startedAt, bool success, string result, string trigger)
    {
        lock (_lock)
        {
            if (_jobs.TryGetValue(name, out var j))
            {
                j.IsRunning = false;
                j.LastRunAt = startedAt;
                j.LastSuccess = success;
                j.LastResult = result;
                j.TotalRuns++;
                if (!success) j.FailedRuns++;
                j.NextRunAt = DateTime.Now.AddSeconds(j.IntervalSeconds);
            }

            _history.AddFirst(new JobRunInfo
            {
                JobName = name,
                StartedAt = startedAt,
                DurationMs = Math.Round((DateTime.Now - startedAt).TotalMilliseconds, 1),
                Success = success,
                Result = result,
                Trigger = trigger
            });
            while (_history.Count > MaxHistory) _history.RemoveLast();
        }
    }

    public object GetSnapshot()
    {
        lock (_lock)
        {
            return new
            {
                Jobs = _jobs.Values.Select(j => new JobStatusInfo
                {
                    Name = j.Name, Description = j.Description, IntervalSeconds = j.IntervalSeconds,
                    IsRunning = j.IsRunning, LastRunAt = j.LastRunAt, NextRunAt = j.NextRunAt,
                    LastSuccess = j.LastSuccess, LastResult = j.LastResult,
                    TotalRuns = j.TotalRuns, FailedRuns = j.FailedRuns
                }).ToList(),
                History = _history.ToList()
            };
        }
    }

    public bool Exists(string name)
    {
        lock (_lock) return _jobs.ContainsKey(name);
    }
}
