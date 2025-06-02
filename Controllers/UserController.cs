using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using TITFood_Backend.Interfaces;
using TITFood_Backend.Models;
using TITFood_Backend.Common;

namespace TITFood_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // All actions require authentication
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("me")] // Get current logged-in user's details
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound("Không tìm thấy thông tin người dùng.");
            }
            return Ok(user);
        }
        
        [HttpPut("me")] // Update current logged-in user's details
        public async Task<IActionResult> UpdateCurrentUser([FromBody] UpdateUserDto updateUserDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var result = await _userService.UpdateUserAsync(userId, updateUserDto);
            if (result.Succeeded)
            {
                return Ok(new { Message = "Thông tin người dùng được cập nhật thành công." });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.Code, error.Description);
            }
            return BadRequest(ModelState);
        }


        [HttpGet("{id}")]
        [Authorize(Roles = AppRole.Admin)] // Only Admin can get user by any ID
        public async Task<ActionResult<UserDto>> GetUserById(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound("Không tìm thấy người dùng.");
            }
            return Ok(user);
        }

        [HttpGet]
        [Authorize(Roles = AppRole.Admin)] // Only Admin can get all users
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        // Add other endpoints like DeleteUser, AssignRole etc. for Admin if needed
    }
}
