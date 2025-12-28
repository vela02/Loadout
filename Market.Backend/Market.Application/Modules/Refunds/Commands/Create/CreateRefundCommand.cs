namespace Market.Application.Modules.Refunds.Commands.Create;

public record CreateRefundCommand(CreateRefundDto Dto) : IRequest<bool>;