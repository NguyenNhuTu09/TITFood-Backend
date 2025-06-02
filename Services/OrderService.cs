using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TITFood_Backend.Data;
using TITFood_Backend.Entities;
using TITFood_Backend.Interfaces;
using TITFood_Backend.Models;

namespace TITFood_Backend.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public OrderService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<OrderDto?> CreateOrderAsync(CreateOrderDto createOrderDto, string userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) throw new ArgumentException("Người dùng không tồn tại.");

            var restaurant = await _context.Restaurants.FindAsync(createOrderDto.RestaurantId);
            if (restaurant == null) throw new ArgumentException("Nhà hàng không tồn tại.");

            var order = _mapper.Map<Order>(createOrderDto);
            order.UserId = userId;
            order.OrderDate = DateTime.UtcNow;
            order.Status = OrderStatus.Pending;

            decimal totalAmount = 0;
            foreach (var itemDto in createOrderDto.OrderItems)
            {
                var dish = await _context.Dishes.FindAsync(itemDto.DishId);
                if (dish == null || !dish.IsAvailable)
                {
                    // Xử lý trường hợp món ăn không tồn tại hoặc không có sẵn
                    // Có thể throw exception hoặc bỏ qua món này
                    throw new ArgumentException($"Món ăn với ID {itemDto.DishId} không hợp lệ hoặc không có sẵn.");
                }
                var orderItem = _mapper.Map<OrderItem>(itemDto);
                orderItem.UnitPrice = dish.Price; // Lấy giá hiện tại của món ăn
                order.OrderItems.Add(orderItem);
                totalAmount += orderItem.Quantity * orderItem.UnitPrice;
            }
            order.TotalAmount = totalAmount;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Nạp các thông tin liên quan để trả về DTO đầy đủ
            var createdOrder = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Restaurant)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Dish)
                .FirstOrDefaultAsync(o => o.Id == order.Id);
                
            return _mapper.Map<OrderDto>(createdOrder);
        }

        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
        {
            var orders = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Restaurant)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Dish)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<OrderDto?> GetOrderByIdAsync(int orderId, string userId)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Restaurant)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Dish)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);
            return _mapper.Map<OrderDto>(order);
        }
         public async Task<OrderDto?> GetOrderByIdForAdminAsync(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Restaurant)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Dish)
                .FirstOrDefaultAsync(o => o.Id == orderId);
            return _mapper.Map<OrderDto>(order);
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(string userId)
        {
            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.User)
                .Include(o => o.Restaurant)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Dish)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) return false;

            order.Status = newStatus;
            // Thêm logic nghiệp vụ nếu cần (ví dụ: gửi thông báo, kiểm tra quyền,...)
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}