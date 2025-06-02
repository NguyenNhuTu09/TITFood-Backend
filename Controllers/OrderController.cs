using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using TITFood_Backend.Interfaces;
using TITFood_Backend.Models;
using TITFood_Backend.Entities; 
using TITFood_Backend.Common; 

namespace TITFood_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
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
        [Authorize(Roles = AppRole.Admin)] 
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        [HttpPut("{orderId}/status")]
        [Authorize(Roles = AppRole.Admin + "," + AppRole.Customer)] // Admin or Customer (for cancellation)
        public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromBody] UpdateOrderStatusDto statusDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(currentUserId)) return Unauthorized();


            var success = await _orderService.UpdateOrderStatusAsync(orderId, statusDto.Status, currentUserId);
            if (!success) return Forbid("Không thể cập nhật trạng thái đơn hàng hoặc không tìm thấy đơn hàng.");
            return Ok(new { Message = "Trạng thái đơn hàng đã được cập nhật."});
        }
    }
}