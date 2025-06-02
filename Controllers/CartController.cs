using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using TITFood_Backend.Interfaces;
using TITFood_Backend.Models;

namespace TITFood_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // All cart operations require authentication
    public class CartsController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartsController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet("my-cart")]
        public async Task<ActionResult<CartDto>> GetMyCart()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var cart = await _cartService.GetCartByUserIdAsync(userId);
            if (cart == null) return Ok(new CartDto { UserId = userId }); // Return empty cart representation if none exists
            return Ok(cart);
        }

        [HttpPost("items")]
        public async Task<ActionResult<CartDto>> AddItemToMyCart([FromBody] AddToCartDto itemDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            try
            {
                var updatedCart = await _cartService.AddItemToCartAsync(userId, itemDto);
                if (updatedCart == null) return BadRequest("Không thể thêm vào giỏ hàng.");
                return Ok(updatedCart);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPut("items/{cartItemId}")]
        public async Task<ActionResult<CartDto>> UpdateCartItem(int cartItemId, [FromBody] UpdateCartItemDto itemDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var updatedCart = await _cartService.UpdateCartItemAsync(userId, cartItemId, itemDto);
            if (updatedCart == null) return NotFound("Không tìm thấy sản phẩm trong giỏ hàng hoặc giỏ hàng không tồn tại.");
            return Ok(updatedCart);
        }

        [HttpDelete("items/{cartItemId}")]
        public async Task<IActionResult> RemoveItemFromMyCart(int cartItemId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var success = await _cartService.RemoveItemFromCartAsync(userId, cartItemId);
            if (!success) return NotFound("Không tìm thấy sản phẩm trong giỏ hàng hoặc giỏ hàng không tồn tại.");
            return Ok(new { Message = "Sản phẩm đã được xóa khỏi giỏ hàng."});
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> ClearMyCart()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var success = await _cartService.ClearCartAsync(userId);
            if (!success) return BadRequest("Giỏ hàng không tồn tại hoặc đã trống."); // Or NotFound
            return Ok(new { Message = "Giỏ hàng đã được làm trống."});
        }
    }
}
