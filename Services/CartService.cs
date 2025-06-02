using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using TITFood_Backend.Data;
using TITFood_Backend.Entities;
using TITFood_Backend.Interfaces;
using TITFood_Backend.Models;

namespace TITFood_Backend.Services
{
    public class CartService : ICartService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CartService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<CartDto?> GetCartByUserIdAsync(string userId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                    .ThenInclude(ci => ci.Dish) // Eager load dish details for each cart item
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                // Optionally create a cart if it doesn't exist
                // cart = new Cart { UserId = userId };
                // _context.Carts.Add(cart);
                // await _context.SaveChangesAsync();
                // return _mapper.Map<CartDto>(cart); // Return empty cart
                return null; // Or return null if cart must exist
            }
            return _mapper.Map<CartDto>(cart);
        }

        public async Task<CartDto?> AddItemToCartAsync(string userId, AddToCartDto itemDto)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                _context.Carts.Add(cart);
                // Need to save here so cart.Id is generated if it's new
                await _context.SaveChangesAsync(); 
            }

            var dish = await _context.Dishes.FindAsync(itemDto.DishId);
            if (dish == null || !dish.IsAvailable)
            {
                throw new ArgumentException("Món ăn không tồn tại hoặc không có sẵn.");
            }

            var cartItem = cart.Items.FirstOrDefault(i => i.DishId == itemDto.DishId);
            if (cartItem != null)
            {
                // Item already in cart, update quantity
                cartItem.Quantity += itemDto.Quantity;
            }
            else
            {
                // New item for the cart
                cartItem = new CartItem
                {
                    CartId = cart.Id, // Assign CartId
                    DishId = itemDto.DishId,
                    Quantity = itemDto.Quantity
                };
                _context.CartItems.Add(cartItem); // Add to context's CartItems DbSet
            }
            cart.LastModified = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return await GetCartByUserIdAsync(userId); // Reload cart to reflect changes
        }

        public async Task<CartDto?> UpdateCartItemAsync(string userId, int cartItemId, UpdateCartItemDto itemDto)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null) return null; // Cart not found

            var cartItem = await _context.CartItems.FirstOrDefaultAsync(ci => ci.Id == cartItemId && ci.CartId == cart.Id);
            
            if (cartItem == null) return null; // Item not found in this user's cart

            cartItem.Quantity = itemDto.Quantity;
            cart.LastModified = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return await GetCartByUserIdAsync(userId);
        }

        public async Task<bool> RemoveItemFromCartAsync(string userId, int cartItemId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart == null) return false;

            var cartItem = await _context.CartItems.FirstOrDefaultAsync(ci => ci.Id == cartItemId && ci.CartId == cart.Id);
            if (cartItem == null) return false; // Item not found in this user's cart

            _context.CartItems.Remove(cartItem);
            cart.LastModified = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ClearCartAsync(string userId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart == null || !cart.Items.Any()) return false; // Cart not found or already empty

            _context.CartItems.RemoveRange(cart.Items);
            cart.Items.Clear(); // Also clear in-memory collection
            cart.LastModified = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}