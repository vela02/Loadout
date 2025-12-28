
namespace Market.Application.Modules.Refunds.Queries.GetByUser;

public record GetUserRefundsQuery : IRequest<List<RefundDto>>;