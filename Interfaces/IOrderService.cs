using System.Collections.Generic;
using System.Threading.Tasks;
using TITFood_Backend.Models;
using TITFood_Backend.Entities; // For OrderStatus

namespace TITFood_Backend.Interfaces
{
    public interface IOrderService
    {
        Task<OrderDto?> CreateOrderAsync(CreateOrderDto createOrderDto, string userId);
        Task<OrderDto?> GetOrderByIdAsync(int orderId, string userId); // User chỉ xem được đơn của mình
        Task<OrderDto?> GetOrderByIdForAdminAsync(int orderId); // Admin xem được mọi đơn
        Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(string userId);
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync(); // For admin
        Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus);
        // Task<bool> CancelOrderAsync(int orderId, string userId); // Optional
    }
}