namespace Market.Core.Exceptions;

/// <summary>
/// Predstavlja grešku koja nastaje kada se prekrši neko poslovno pravilo.
///
/// Ovakve greške ne označavaju sistemske probleme (npr. null reference),
/// nego situacije kada zahtjev ne može biti izvršen jer bi narušio
/// logiku poslovanja.
///
/// Primjer: pokušaj onemogućavanja kategorije koja još sadrži aktivne proizvode.
/// </summary>
public sealed class MarketBusinessRuleException : Exception
{
    public string Code { get; }

    public MarketBusinessRuleException(string code, string message)
        : base(message)
    {
        Code = code;
    }

    public MarketBusinessRuleException(string code, string message, Exception? innerException)
        : base(message, innerException)
    {
        Code = code;
    }
}
