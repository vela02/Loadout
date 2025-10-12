namespace Market.Shared.Dtos;

public sealed class ErrorDto
{
    public required string Code { get; init; }     // npr. "internal.error"
    public required string Message { get; init; }  // kratka poruka
}