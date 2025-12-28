
namespace Market.Application.Modules.Refunds.Queries.GetByUser;

public sealed class GetUserRefundsHandler(
    IAppDbContext ctx,
    IAppCurrentUser currentUser) : IRequestHandler<GetUserRefundsQuery, List<RefundDto>>
{
    public async Task<List<RefundDto>> Handle(GetUserRefundsQuery request, CancellationToken ct)
    {
        var userId = currentUser.UserId;

        return await ctx.Refunds
            .Include(r => r.Order)
            .Include(r => r.Status)
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.RefundDate)
            .Select(r => new RefundDto 
            {
                Id = r.Id,
                OrderId = r.OrderId,
                OrderTitle = $"Order #{r.OrderId} ({r.Order.TotalAmount:C})",
                Reason = r.Reason,
                Status = r.Status.Name,
                RequestedAt = r.RefundDate,
                AdminResponse = r.AdminResponse
            })
            .ToListAsync(ct);
    }
}