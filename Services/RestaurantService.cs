using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TITFood_Backend.Data;
using TITFood_Backend.Entities;
using TITFood_Backend.Interfaces;
using TITFood_Backend.Models;

namespace TITFood_Backend.Services
{
    public class RestaurantService : IRestaurantService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public RestaurantService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // Restaurant methods
        public async Task<RestaurantDto?> CreateAsync(CreateRestaurantDto createDto)
        {
            var restaurant = _mapper.Map<Restaurant>(createDto);
            _context.Restaurants.Add(restaurant);
            await _context.SaveChangesAsync();
            return _mapper.Map<RestaurantDto>(restaurant);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var restaurant = await _context.Restaurants.FindAsync(id);
            if (restaurant == null) return false;

            _context.Restaurants.Remove(restaurant);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<RestaurantDto>> GetAllAsync()
        {
            var restaurants = await _context.Restaurants
                                        .Include(r => r.Menus)
                                        .ThenInclude(m => m.Dishes)
                                        .ToListAsync();
            return _mapper.Map<IEnumerable<RestaurantDto>>(restaurants);
        }

        public async Task<RestaurantDto?> GetByIdAsync(int id)
        {
            var restaurant = await _context.Restaurants
                                        .Include(r => r.Menus)
                                        .ThenInclude(m => m.Dishes)
                                        .FirstOrDefaultAsync(r => r.Id == id);
            return _mapper.Map<RestaurantDto>(restaurant);
        }

        public async Task<bool> UpdateAsync(int id, UpdateRestaurantDto updateDto)
        {
            var restaurant = await _context.Restaurants.FindAsync(id);
            if (restaurant == null) return false;

            _mapper.Map(updateDto, restaurant);
            _context.Restaurants.Update(restaurant);
            await _context.SaveChangesAsync();
            return true;
        }

        // Menu methods
        public async Task<MenuDto?> CreateMenuAsync(CreateMenuDto createMenuDto)
        {
            var restaurantExists = await _context.Restaurants.AnyAsync(r => r.Id == createMenuDto.RestaurantId);
            if (!restaurantExists) return null; // Hoặc throw exception

            var menu = _mapper.Map<Menu>(createMenuDto);
            _context.Menus.Add(menu);
            await _context.SaveChangesAsync();
            return _mapper.Map<MenuDto>(menu);
        }

        public async Task<bool> DeleteMenuAsync(int menuId)
        {
            var menu = await _context.Menus.FindAsync(menuId);
            if (menu == null) return false;

            _context.Menus.Remove(menu);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<MenuDto?> GetMenuByIdAsync(int menuId)
        {
            var menu = await _context.Menus
                                .Include(m => m.Dishes)
                                .FirstOrDefaultAsync(m => m.Id == menuId);
            return _mapper.Map<MenuDto>(menu);
        }

        public async Task<IEnumerable<MenuDto>> GetMenusByRestaurantAsync(int restaurantId)
        {
            var menus = await _context.Menus
                                .Where(m => m.RestaurantId == restaurantId)
                                .Include(m => m.Dishes)
                                .ToListAsync();
            return _mapper.Map<IEnumerable<MenuDto>>(menus);
        }

        public async Task<bool> UpdateMenuAsync(int menuId, UpdateMenuDto updateMenuDto)
        {
            var menu = await _context.Menus.FindAsync(menuId);
            if (menu == null) return false;

             var restaurantExists = await _context.Restaurants.AnyAsync(r => r.Id == updateMenuDto.RestaurantId);
            if (!restaurantExists) return false; // Nhà hàng phải tồn tại

            _mapper.Map(updateMenuDto, menu);
            _context.Menus.Update(menu);
            await _context.SaveChangesAsync();
            return true;
        }


        // Dish methods
        public async Task<DishDto?> CreateDishAsync(CreateDishDto createDishDto)
        {
            var menuExists = await _context.Menus.AnyAsync(m => m.Id == createDishDto.MenuId);
            if (!menuExists) return null; // Hoặc throw exception

            var dish = _mapper.Map<Dish>(createDishDto);
            _context.Dishes.Add(dish);
            await _context.SaveChangesAsync();
            return _mapper.Map<DishDto>(dish);
        }

        public async Task<bool> DeleteDishAsync(int dishId)
        {
            var dish = await _context.Dishes.FindAsync(dishId);
            if (dish == null) return false;

            _context.Dishes.Remove(dish);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<DishDto?> GetDishByIdAsync(int dishId)
        {
            var dish = await _context.Dishes.FindAsync(dishId);
            return _mapper.Map<DishDto>(dish);
        }

        public async Task<IEnumerable<DishDto>> GetDishesByMenuAsync(int menuId)
        {
            var dishes = await _context.Dishes
                                .Where(d => d.MenuId == menuId)
                                .ToListAsync();
            return _mapper.Map<IEnumerable<DishDto>>(dishes);
        }

        public async Task<bool> UpdateDishAsync(int dishId, UpdateDishDto updateDishDto)
        {
            var dish = await _context.Dishes.FindAsync(dishId);
            if (dish == null) return false;

            var menuExists = await _context.Menus.AnyAsync(m => m.Id == updateDishDto.MenuId);
            if (!menuExists) return false; // Menu phải tồn tại

            _mapper.Map(updateDishDto, dish);
            _context.Dishes.Update(dish);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}