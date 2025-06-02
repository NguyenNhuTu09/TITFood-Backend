using System.Collections.Generic;
using System.Threading.Tasks;
using TITFood_Backend.Models;
using Microsoft.AspNetCore.Identity; // For IdentityResult

namespace TITFood_Backend.Interfaces
{
    public interface IUserService
    {
        Task<UserDto?> GetUserByIdAsync(string userId);
        Task<IEnumerable<UserDto>> GetAllUsersAsync(); // For Admin
        Task<IdentityResult> UpdateUserAsync(string userId, UpdateUserDto updateUserDto);
    }
}