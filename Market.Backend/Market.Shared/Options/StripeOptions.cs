namespace Market.Shared.Options;

/// <summary>
/// Stripe postavke. Ključevi se NE drže u appsettings.json (koji ide na git),
/// nego u User Secrets (Market.API -> Manage User Secrets).
/// </summary>
public sealed class StripeOptions
{
    public const string SectionName = "Stripe";

    public string SecretKey { get; set; } = "";       // sk_test_... (samo backend!)
    public string PublishableKey { get; set; } = "";  // pk_test_... (smije na frontend)
    public string Currency { get; set; } = "bam";     // konvertibilna marka (KM)
}
