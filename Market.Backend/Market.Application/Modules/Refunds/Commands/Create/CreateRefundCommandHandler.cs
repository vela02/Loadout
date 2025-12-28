using Market.Domain.Models;


namespace Market.Application.Modules.Refunds.Commands.Create;

public sealed class CreateRefundCommandHandler(
    IAppDbContext ctx,
    IAppCurrentUser currentUser) : IRequestHandler<CreateRefundCommand, bool>
{
    public async Task<bool> Handle(CreateRefundCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId;
        var orderId = request.Dto.OrderId;

        var order = await ctx.Orders.FirstOrDefaultAsync(o => o.Id == orderId, ct);

        if (order == null) return false;

        // validations
        if (order.UserId != userId) throw new UnauthorizedAccessException("You can only refund your own orders.");
        if (order.StatusId != 3) throw new InvalidOperationException("Cannot refund an order that is not completed.");

        var daysSincePurchase = (DateTime.UtcNow - order.Date).TotalDays;
        if (daysSincePurchase > 14) throw new InvalidOperationException("Refund period expired.");

        var alreadyRequested = await ctx.Refunds.AnyAsync(r => r.OrderId == orderId && r.StatusId == 1, ct);
        if (alreadyRequested) throw new InvalidOperationException("Request already pending.");

        
        var refund = new Refund
        {
            OrderId = orderId,
            UserId = (int)userId,
            Reason = request.Dto.Reason,
            StatusId = 1,
            RefundDate = DateTime.UtcNow,
            Amount = order.TotalAmount,
            AdminResponse = null
        };

        ctx.Refunds.Add(refund);
        await ctx.SaveChangesAsync(ct);

        return true;
    }
}