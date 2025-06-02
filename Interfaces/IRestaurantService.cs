using System.Collections.Generic;
using System.Threading.Tasks;
using TITFood_Backend.Models;

namespace TITFood_Backend.Interfaces
{
    public interface IRestaurantService
    {
        Task<IEnumerable<RestaurantDto>> GetAllAsync();
        Task<RestaurantDto?> GetByIdAsync(int id);
        Task<RestaurantDto?> CreateAsync(CreateRestaurantDto createDto);
        Task<bool> UpdateAsync(int id, UpdateRestaurantDto updateDto);
        Task<bool> DeleteAsync(int id);

        // Menu related methods
        Task<IEnumerable<MenuDto>> GetMenusByRestaurantAsync(int restaurantId);
        Task<MenuDto?> GetMenuByIdAsync(int menuId);
        Task<MenuDto?> CreateMenuAsync(CreateMenuDto createMenuDto);
        Task<bool> UpdateMenuAsync(int menuId, UpdateMenuDto updateMenuDto);
        Task<bool> DeleteMenuAsync(int menuId);

        // Dish related methods
        Task<IEnumerable<DishDto>> GetDishesByMenuAsync(int menuId);
        Task<DishDto?> GetDishByIdAsync(int dishId);
        Task<DishDto?> CreateDishAsync(CreateDishDto createDishDto);
        Task<bool> UpdateDishAsync(int dishId, UpdateDishDto updateDishDto);
        Task<bool> DeleteDishAsync(int dishId);
    }
}