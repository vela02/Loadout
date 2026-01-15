using Market.Application.Modules.Sales.Orders.Commands.Status;

namespace Market.Application.Modules.Sales.Orders.Commands.ChangeStatus;

/// <summary>
/// Handler for changing order status
/// </summary>
public class ChangeOrderStatusCommandHandler(IAppDbContext db, IAppCurrentUser user) : IRequestHandler<ChangeOrderStatusCommand>
{
    public async Task Handle(ChangeOrderStatusCommand request, CancellationToken ct)
    {
        if (!user.IsAdmin)
        {
            throw new MarketBusinessRuleException("UNAUTHORIZED", "Samo admin može mijenjati status.");
        }

        var order = await db.Orders
            .FirstOrDefaultAsync(o => o.Id == request.Id, ct);

        if (order is null)
        {
            throw new MarketNotFoundException($"Narudžba {request.Id} nije pronađena.");
        }

        var statusExists = await db.OrderStatuses
            .AnyAsync(s => s.Id == request.NewStatusId, ct);

        if (!statusExists)
        {
            throw new MarketNotFoundException("Odabrani status ne postoji u sustavu.");
        }

        order.StatusId = request.NewStatusId;
        await db.SaveChangesAsync(ct);  
    }
}