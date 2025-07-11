using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TITFood_Backend.Interfaces;
using TITFood_Backend.Models;
using TITFood_Backend.Common; 

namespace TITFood_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (result, user) = await _authService.RegisterAsync(model, AppRole.Customer); 
            if (result.Succeeded && user != null)
            {
                return Ok(new { Message = "Đăng ký thành công!", UserId = user.Id });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.Code, error.Description);
            }
            return BadRequest(ModelState);
        }
        
        [HttpPost("register-admin")] 
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterModel model)
        {
            // In a real app, this endpoint should be protected (e.g., by an existing admin or a secret key)
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (result, user) = await _authService.RegisterAsync(model, AppRole.Admin);
            if (result.Succeeded && user != null)
            {
                return Ok(new { Message = "Tài khoản Admin đăng ký thành công!", UserId = user.Id });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.Code, error.Description);
            }
            return BadRequest(ModelState);
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var authResponse = await _authService.LoginAsync(model);
            if (authResponse == null)
                return Unauthorized(new { Message = "Tên đăng nhập hoặc mật khẩu không chính xác." });

            return Ok(authResponse);
        }
    }
}