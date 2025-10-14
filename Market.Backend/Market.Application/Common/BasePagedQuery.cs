namespace Market.Application.Common;

/// <summary>
/// Bazna klasa za list upite sa paginacijom, pretragom i sortiranjem.
/// </summary>
public abstract class BasePagedQuery<TItem> : IRequest<PageResult<TItem>>
{
    /// <summary>Parametri paginacije (stranica i veličina stranice).</summary>
    public required PageRequest Paging { get; init; } = new();

}
