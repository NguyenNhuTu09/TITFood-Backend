using System.Collections.Generic;
using System.Threading.Tasks;
using TITFood_Backend.Models;
using TITFood_Backend.Entities; 

namespace TITFood_Backend.Interfaces
{
    public interface IOrderService
    {
        Task<OrderDto?> CreateOrderAsync(CreateOrderDto createOrderDto, string userId);
        Task<OrderDto?> GetOrderByIdAsync(int orderId, string userId); 
        Task<OrderDto?> GetOrderByIdForAdminAsync(int orderId); 
        Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(string userId);
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync(); 
        Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus, string currentUserId);
    }
}