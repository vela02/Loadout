namespace Market.Shared.Dtos
{
    /// <summary>Odgovor na "create-intent": frontend s ClientSecret-om potvrđuje plaćanje kod Stripe-a.</summary>
    public class PaymentIntentDto
    {
        public string ClientSecret { get; set; } = null!;
        public string PaymentIntentId { get; set; } = null!;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = null!;
    }
}
