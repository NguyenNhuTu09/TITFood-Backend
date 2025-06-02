using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using TITFood_Backend.Interfaces;
using TITFood_Backend.Models;
using TITFood_Backend.Entities; // For OrderStatus
using TITFood_Backend.Common; // For AppRole

namespace TITFood_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Yêu cầu xác thực cho tất cả các action trong controller này
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] CreateOrderDto createOrderDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Không thể xác định người dùng.");
            }

            try
            {
                var createdOrder = await _orderService.CreateOrderAsync(createOrderDto, userId);
                if (createdOrder == null) return BadRequest("Không thể tạo đơn hàng.");
                // Trả về thông tin chi tiết của đơn hàng vừa tạo
                var detailedOrder = await _orderService.GetOrderByIdAsync(createdOrder.Id, userId);
                return CreatedAtAction(nameof(GetOrderById), new { orderId = detailedOrder!.Id }, detailedOrder);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("{orderId}")]
        public async Task<ActionResult<OrderDto>> GetOrderById(int orderId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
             if (string.IsNullOrEmpty(userId)) return Unauthorized();

            OrderDto? order;
            if (User.IsInRole(AppRole.Admin))
            {
                order = await _orderService.GetOrderByIdForAdminAsync(orderId);
            }
            else
            {
                order = await _orderService.GetOrderByIdAsync(orderId, userId);
            }
            
            if (order == null) return NotFound("Không tìm thấy đơn hàng.");
            return Ok(order);
        }

        [HttpGet("my-orders")]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetMyOrders()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var orders = await _orderService.GetOrdersByUserIdAsync(userId);
            return Ok(orders);
        }

        [HttpGet]
        [Authorize(Roles = AppRole.Admin)] // Chỉ Admin mới xem được tất cả đơn hàng
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        [HttpPut("{orderId}/status")]
        [Authorize(Roles = AppRole.Admin + "," + AppRole.RestaurantOwner)] // Admin hoặc chủ nhà hàng có thể cập nhật trạng thái
        public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromBody] UpdateOrderStatusDto statusDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Thêm logic kiểm tra quyền của RestaurantOwner nếu cần (ví dụ: chỉ được update đơn của nhà hàng mình)
            var success = await _orderService.UpdateOrderStatusAsync(orderId, statusDto.Status);
            if (!success) return NotFound("Không tìm thấy đơn hàng hoặc cập nhật thất bại.");
            return NoContent();
        }
    }
}