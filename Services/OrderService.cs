using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TITFood_Backend.Data;
using TITFood_Backend.Entities;
using TITFood_Backend.Interfaces;
using TITFood_Backend.Models;
using TITFood_Backend.Common;


namespace TITFood_Backend.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager; // For checking roles


        public OrderService(ApplicationDbContext context, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<OrderDto?> CreateOrderAsync(CreateOrderDto createOrderDto, string userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) throw new ArgumentException("Người dùng không tồn tại.");

            var restaurant = await _context.Restaurants.FindAsync(createOrderDto.RestaurantId);
            if (restaurant == null) throw new ArgumentException("Nhà hàng không tồn tại.");

            var cart = await _context.Carts
                                 .Include(c => c.Items)
                                 .ThenInclude(ci => ci.Dish)
                                 .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.Items.Any())
            {
                throw new ArgumentException("Giỏ hàng trống hoặc không tìm thấy.");
            }
            
            // Use items from CreateOrderDto if provided and not empty, otherwise use cart items
            var itemsToProcess = createOrderDto.OrderItems.Any() ? createOrderDto.OrderItems : null;

            if (itemsToProcess == null || !itemsToProcess.Any())
            {
                 // If no items in DTO, use cart items
                if (cart == null || !cart.Items.Any())
                {
                    throw new ArgumentException("Không có sản phẩm nào để đặt hàng (giỏ hàng trống và không có sản phẩm nào được cung cấp).");
                }
                // Map cart items to CreateOrderItemDto structure for consistency
                itemsToProcess = cart.Items.Select(ci => new CreateOrderItemDto { DishId = ci.DishId, Quantity = ci.Quantity }).ToList();
            }


            var order = new Order
            {
                UserId = userId,
                RestaurantId = createOrderDto.RestaurantId, // Assume all items in cart are from this restaurant for now
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                ShippingAddress = createOrderDto.ShippingAddress ?? user.Address, // Use user's address if not provided
                Notes = createOrderDto.Notes
            };
            
            decimal totalAmount = 0;
            foreach (var itemDto in itemsToProcess)
            {
                var dish = await _context.Dishes.FindAsync(itemDto.DishId);
                if (dish == null || !dish.IsAvailable)
                {
                    throw new ArgumentException($"Món ăn với ID {itemDto.DishId} không hợp lệ hoặc không có sẵn.");
                }
                // Check if dish belongs to the specified restaurant
                var menu = await _context.Menus.FindAsync(dish.MenuId);
                if (menu == null || menu.RestaurantId != createOrderDto.RestaurantId)
                {
                     throw new ArgumentException($"Món ăn '{dish.Name}' (ID: {dish.Id}) không thuộc nhà hàng đã chọn (ID: {createOrderDto.RestaurantId}).");
                }

                var orderItem = new OrderItem
                {
                    DishId = itemDto.DishId,
                    Quantity = itemDto.Quantity,
                    UnitPrice = dish.Price 
                };
                order.OrderItems.Add(orderItem);
                totalAmount += orderItem.Quantity * orderItem.UnitPrice;
            }
            order.TotalAmount = totalAmount;

            _context.Orders.Add(order);
            
            // Clear the cart after successful order creation
            if (cart != null && createOrderDto.OrderItems.Any()) // Only clear if DTO items were used, implying checkout from cart
            {
                _context.CartItems.RemoveRange(cart.Items);
                cart.Items.Clear(); // Also clear in-memory collection
                cart.LastModified = DateTime.UtcNow;
                _context.Carts.Update(cart);
            }

            await _context.SaveChangesAsync();
            
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

        public async Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus, string currentUserId)
        {
            var order = await _context.Orders.Include(o => o.Restaurant).FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null) return false;

            var currentUser = await _userManager.FindByIdAsync(currentUserId);
            if (currentUser == null) return false; // Should not happen if user is authenticated

            bool isAdmin = await _userManager.IsInRoleAsync(currentUser, AppRole.Admin);
            // bool isOwner = order.Restaurant?.OwnerId == currentUserId; // Assuming Restaurant has OwnerId

            // For now, only Admin can change status. Add RestaurantOwner logic later if needed.
            if (!isAdmin)
            {
                // If user is not admin, check if they are the customer and the status change is valid (e.g., cancelling a pending order)
                if (order.UserId == currentUserId) {
                    if (newStatus == OrderStatus.Cancelled && order.Status == OrderStatus.Pending) {
                        // Customer can cancel their own pending order
                    } else {
                        return false; // Customer cannot change to other statuses or cancel non-pending orders
                    }
                } else {
                     return false; // Not admin and not the customer who placed the order
                }
            }
            
            // Add more complex status transition logic if needed
            // e.g., cannot go from Delivered back to Processing

            order.Status = newStatus;
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
