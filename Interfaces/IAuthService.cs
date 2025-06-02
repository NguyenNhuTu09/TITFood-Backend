using System.Threading.Tasks;
using TITFood_Backend.Models;
using Microsoft.AspNetCore.Identity;
using TITFood_Backend.Entities; // For ApplicationUser

namespace TITFood_Backend.Interfaces
{
    public interface IAuthService
    {
        Task<(IdentityResult, ApplicationUser?)> RegisterAsync(RegisterModel model, string role);
        Task<AuthResponseDto?> LoginAsync(LoginModel model);
    }
}