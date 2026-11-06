namespace Market.Domain.Models;

public partial class CartItem
{
    public int Id { get; set; }

    public int? CartId { get; set; }

    public int? GameId { get; set; }

    public int Quantity { get; set; }

    public DateTime AddedAt { get; set; }

    // "Save for later": stavka ostaje kod korisnika, ali se NE naplaćuje pri checkoutu
    public bool IsSavedForLater { get; set; }

    public virtual Cart? Cart { get; set; }

    public virtual Game? Game { get; set; }
}
