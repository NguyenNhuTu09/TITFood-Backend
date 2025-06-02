using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TITFood_Backend.Interfaces;
using TITFood_Backend.Models;
using TITFood_Backend.Common; 

namespace TITFood_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RestaurantsController : ControllerBase
    {
        private readonly IRestaurantService _restaurantService;

        public RestaurantsController(IRestaurantService restaurantService)
        {
            _restaurantService = restaurantService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RestaurantDto>>> GetAllRestaurants([FromQuery] string? searchTerm)
        {
            var restaurants = await _restaurantService.GetAllAsync(searchTerm);
            return Ok(restaurants);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RestaurantDto>> GetRestaurantById(int id)
        {
            var restaurant = await _restaurantService.GetByIdAsync(id);
            if (restaurant == null) return NotFound();
            return Ok(restaurant);
        }

        [HttpPost]
        [Authorize(Roles = AppRole.Admin)] 
        public async Task<ActionResult<RestaurantDto>> CreateRestaurant([FromBody] CreateRestaurantDto createDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var createdRestaurant = await _restaurantService.CreateAsync(createDto);
            if (createdRestaurant == null) return BadRequest("Không thể tạo nhà hàng."); 
            return CreatedAtAction(nameof(GetRestaurantById), new { id = createdRestaurant.Id }, createdRestaurant);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = AppRole.Admin)] 
        public async Task<IActionResult> UpdateRestaurant(int id, [FromBody] UpdateRestaurantDto updateDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var success = await _restaurantService.UpdateAsync(id, updateDto);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = AppRole.Admin)] 
        public async Task<IActionResult> DeleteRestaurant(int id)
        {
            var success = await _restaurantService.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }

        // Menu Endpoints
        [HttpGet("{restaurantId}/menus")]
        public async Task<ActionResult<IEnumerable<MenuDto>>> GetMenusForRestaurant(int restaurantId)
        {
            var menus = await _restaurantService.GetMenusByRestaurantAsync(restaurantId);
            if (menus == null || !menus.Any()) return NotFound("Không tìm thấy thực đơn nào cho nhà hàng này.");
            return Ok(menus);
        }

        [HttpGet("menus/{menuId}")]
        public async Task<ActionResult<MenuDto>> GetMenu(int menuId)
        {
            var menu = await _restaurantService.GetMenuByIdAsync(menuId);
            if (menu == null) return NotFound();
            return Ok(menu);
        }

        [HttpPost("menus")]
        [Authorize(Roles = AppRole.Admin)]
        public async Task<ActionResult<MenuDto>> CreateMenu([FromBody] CreateMenuDto createMenuDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var createdMenu = await _restaurantService.CreateMenuAsync(createMenuDto);
            if (createdMenu == null) return BadRequest("Không thể tạo thực đơn, nhà hàng không tồn tại.");
            return CreatedAtAction(nameof(GetMenu), new { menuId = createdMenu.Id }, createdMenu);
        }

        [HttpPut("menus/{menuId}")]
        [Authorize(Roles = AppRole.Admin)]
        public async Task<IActionResult> UpdateMenu(int menuId, [FromBody] UpdateMenuDto updateMenuDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var success = await _restaurantService.UpdateMenuAsync(menuId, updateMenuDto);
            if (!success) return NotFound("Thực đơn không tồn tại hoặc nhà hàng không hợp lệ.");
            return NoContent();
        }

        [HttpDelete("menus/{menuId}")]
        [Authorize(Roles = AppRole.Admin)]
        public async Task<IActionResult> DeleteMenu(int menuId)
        {
            var success = await _restaurantService.DeleteMenuAsync(menuId);
            if (!success) return NotFound();
            return NoContent();
        }

        // Dish Endpoints 
        [HttpGet("menus/{menuId}/dishes")]
        public async Task<ActionResult<IEnumerable<DishDto>>> GetDishesForMenu(int menuId)
        {
            var dishes = await _restaurantService.GetDishesByMenuAsync(menuId);
             if (dishes == null || !dishes.Any()) return NotFound("Không tìm thấy món ăn nào cho thực đơn này.");
            return Ok(dishes);
        }

        [HttpGet("dishes/{dishId}")]
        public async Task<ActionResult<DishDto>> GetDish(int dishId)
        {
            var dish = await _restaurantService.GetDishByIdAsync(dishId);
            if (dish == null) return NotFound();
            return Ok(dish);
        }

        [HttpPost("dishes")]
        [Authorize(Roles = AppRole.Admin)]
        public async Task<ActionResult<DishDto>> CreateDish([FromBody] CreateDishDto createDishDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var createdDish = await _restaurantService.CreateDishAsync(createDishDto);
             if (createdDish == null) return BadRequest("Không thể tạo món ăn, thực đơn không tồn tại.");
            return CreatedAtAction(nameof(GetDish), new { dishId = createdDish.Id }, createdDish);
        }

        [HttpPut("dishes/{dishId}")]
        [Authorize(Roles = AppRole.Admin)]
        public async Task<IActionResult> UpdateDish(int dishId, [FromBody] UpdateDishDto updateDishDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var success = await _restaurantService.UpdateDishAsync(dishId, updateDishDto);
            if (!success) return NotFound("Món ăn không tồn tại hoặc thực đơn không hợp lệ.");
            return NoContent();
        }

        [HttpDelete("dishes/{dishId}")]
        [Authorize(Roles = AppRole.Admin)]
        public async Task<IActionResult> DeleteDish(int dishId)
        {
            var success = await _restaurantService.DeleteDishAsync(dishId);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}