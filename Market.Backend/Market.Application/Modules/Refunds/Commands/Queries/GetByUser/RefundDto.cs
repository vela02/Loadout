namespace Market.Application.Modules.Refunds.Queries.GetByUser;

public class RefundDto
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public string OrderTitle { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime RequestedAt { get; set; }
    public string? AdminResponse { get; set; }
}