using System.Threading.Tasks;
using TITFood_Backend.Models;

namespace TITFood_Backend.Interfaces
{
    public interface ICartService
    {
        Task<CartDto?> GetCartByUserIdAsync(string userId);
        Task<CartDto?> AddItemToCartAsync(string userId, AddToCartDto itemDto);
        Task<CartDto?> UpdateCartItemAsync(string userId, int cartItemId, UpdateCartItemDto itemDto);
        Task<bool> RemoveItemFromCartAsync(string userId, int cartItemId);
        Task<bool> ClearCartAsync(string userId);
    }
}