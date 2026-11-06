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

        // DELETE api/Order/cart/5/items/12 -> ukloni stavku iz korpe
        [HttpDelete("cart/{userId}/items/{cartItemId}")]
        [AllowAnonymous]
        public async Task<IActionResult> RemoveFromCart(int userId, int cartItemId)
        {
            var ok = await _orderService.RemoveFromCartAsync(userId, cartItemId);
            return ok ? NoContent() : NotFound();
        }

        // PUT api/Order/cart/5/items/12/save-for-later?saved=true -> premjesti u "za kasnije" (ili nazad)
        [HttpPut("cart/{userId}/items/{cartItemId}/save-for-later")]
        [AllowAnonymous]
        public async Task<IActionResult> SaveForLater(int userId, int cartItemId, [FromQuery] bool saved = true)
        {
            var ok = await _orderService.SetSavedForLaterAsync(userId, cartItemId, saved);
            return ok ? NoContent() : NotFound();
        }

        // POST api/Order/save-for-later?userId=5&gameId=3 -> sačuvaj igru za kasnije sa stranice igre
        [HttpPost("save-for-later")]
        [AllowAnonymous]
        public async Task<IActionResult> SaveGameForLater(int userId, int gameId)
        {
            await _orderService.SaveGameForLaterAsync(userId, gameId);
            return Ok();
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
                OrderIds = result.OrderIds,
                Message = result.Message,
                IsPreOrder = result.IsPreOrder,
                TotalAmount = result.TotalAmount
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
        [HttpGet("sales_report-admin")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<SalesReportDto>> GetReport([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            if (from == default) from = DateTime.UtcNow.AddDays(-30);
            if (to == default) to = DateTime.UtcNow;

            var report = await _orderService.GetSalesReportAsync(from, to);

            return Ok(report);
        }

    }
}
