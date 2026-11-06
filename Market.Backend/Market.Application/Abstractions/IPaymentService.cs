using Market.Shared.Dtos;

namespace Market.Application.Abstractions
{
    public interface IPaymentService
    {
        /// <summary>Publishable key za Stripe.js na frontendu.</summary>
        string GetPublishableKey();

        /// <summary>Izračuna iznos korpe NA SERVERU i kreira Stripe PaymentIntent.</summary>
        Task<PaymentIntentDto> CreatePaymentIntentAsync(int userId);

        /// <summary>Provjeri kod Stripe-a da je plaćanje uspjelo, kreira narudžbe i spremi transakciju.</summary>
        Task<CheckoutResultDto> ConfirmPaymentAsync(int userId, string paymentIntentId);
    }
}
