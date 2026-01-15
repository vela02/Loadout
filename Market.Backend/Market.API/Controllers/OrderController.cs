using Market.Application.Abstractions;
using Market.Shared.Dtos;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Market.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        [HttpPost("add-to-cart")]
        [AllowAnonymous]
        public async Task<IActionResult> AddToCart(int userId, int gameId,int quantity=1)
        {
            var success = await _orderService.AddToCartAsync(userId, gameId, quantity);
            if(!success) return BadRequest("Failed to add item to cart.");

            return Ok("Item added to cart successfully.");
        }
        [HttpGet("cart/{userId}")]
        [AllowAnonymous]
        public async Task<ActionResult<List<CartItemDto>>> GetCart(int userId)
        {
            var cart= await _orderService.GetCartAsync(userId);
            return Ok(cart);
        }

        [HttpPost("checkout/{userId}")]
        [AllowAnonymous]
        public async Task<IActionResult> Checkout(int userId)
        {
            var result = await _orderService.CheckoutAsync(userId);

            if (result == null || result.OrderId == 0)
                return BadRequest("Korpa je prazna ili je došlo do greške pri obradi.");

            // Vraćamo tačnu poruku koju je servis generisao
            return Ok(new
            {
                OrderId = result.OrderId,
                Message = result.Message
            });
        }


        [HttpGet("history/{userId}")]
        [AllowAnonymous]
        public async Task<ActionResult<List<OrderHistoryDto>>> GetHistory(int userId)
        {
            var history= await _orderService.GetOrderHistoryAsync(userId);
            return Ok(history);
        }
        [HttpPost("cancel-preorder/{preorderId}")]
        [AllowAnonymous]
        public async Task<IActionResult> CancelPreorder(int preorderId)
        {
            var success= await _orderService.CancelPreOrderAsync(preorderId);

            if(!success) return BadRequest("Failed to cancel pre-order.");
            return Ok("Pre-order cancelled successfully.");
        }

        //  Admin report
        [HttpGet("report")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<SalesReportDto>> GetReport([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            if (from == default) from = DateTime.UtcNow.AddDays(-30);
            if (to == default) to = DateTime.UtcNow;

            var report = await _orderService.GetSalesReportAsync(from, to);

            return Ok(report);
        }

    }
}
