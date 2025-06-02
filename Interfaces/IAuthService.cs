using System.Threading.Tasks;
using TITFood_Backend.Models;
using Microsoft.AspNetCore.Identity;

namespace TITFood_Backend.Interfaces
{
    public interface IAuthService
    {
        Task<(IdentityResult, ApplicationUser?)> RegisterAsync(RegisterModel model, string role);
        Task<AuthResponseDto?> LoginAsync(LoginModel model);
        Task<UserDto?> GetUserByIdAsync(string userId);
    }
}