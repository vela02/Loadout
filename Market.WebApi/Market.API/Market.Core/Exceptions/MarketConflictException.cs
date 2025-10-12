namespace Market.Core.Exceptions;

public sealed class MarketConflictException : Exception
{
    public MarketConflictException(string message) : base(message) { }
}
