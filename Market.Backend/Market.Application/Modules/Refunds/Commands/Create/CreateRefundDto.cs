namespace Market.Application.Modules.Refunds.Commands.Create;

public class CreateRefundDto
{
    public int OrderId { get; set; }
    public string Reason { get; set; } = string.Empty;
}