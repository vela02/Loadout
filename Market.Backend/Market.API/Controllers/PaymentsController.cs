using Market.Application.Abstractions;
using Market.Application.Common.Exceptions;

namespace Market.API.Controllers
{
    /// <summary>
    /// Stripe naplata.
    ///   GET  api/Payments/config                  -> publishable key za Stripe.js
    ///   POST api/Payments/create-intent           -> otvara PaymentIntent za trenutnu korpu
    ///   POST api/Payments/confirm?paymentIntentId -> provjerava plaćanje i kreira narudžbe
    /// Korisnik se uzima iz JWT tokena (ne iz URL-a), pa niko ne može platiti/potvrditi tuđu korpu.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _payments;
        private readonly IAppCurrentUser _currentUser;

        public PaymentsController(IPaymentService payments, IAppCurrentUser currentUser)
        {
            _payments = payments;
            _currentUser = currentUser;
        }

        [HttpGet("config")]
        public IActionResult GetConfig()
        {
            return Ok(new { publishableKey = _payments.GetPublishableKey() });
        }

        [HttpPost("create-intent")]
        public async Task<IActionResult> CreateIntent()
        {
            if (_currentUser.UserId is not int userId) return Unauthorized();
            return await Handle(async () => Ok(await _payments.CreatePaymentIntentAsync(userId)));
        }

        [HttpPost("confirm")]
        public async Task<IActionResult> Confirm([FromQuery] string paymentIntentId)
        {
            if (_currentUser.UserId is not int userId) return Unauthorized();
            return await Handle(async () => Ok(await _payments.ConfirmPaymentAsync(userId, paymentIntentId)));
        }

        /// <summary>Pretvara poslovne i Stripe greške u čitljive HTTP odgovore za frontend.</summary>
        private async Task<IActionResult> Handle(Func<Task<IActionResult>> action)
        {
            try
            {
                return await action();
            }
            catch (MarketConflictException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (MarketBusinessRuleException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Stripe.StripeException ex)
            {
                return BadRequest(new { message = "Stripe: " + ex.Message });
            }
        }
    }
}
