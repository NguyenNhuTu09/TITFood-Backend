using TITFood_Backend.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TITFood_Backend.Entities;
using TITFood_Backend.Helpers;
using TITFood_Backend.Interfaces;
using TITFood_Backend.Models;
using AutoMapper;


namespace TITFood_Backend.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context; // For creating cart

        public AuthService(UserManager<ApplicationUser> userManager,
                           RoleManager<IdentityRole> roleManager,
                           IConfiguration configuration,
                           IMapper mapper,
                           ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _mapper = mapper;
            _context = context;
        }

        public async Task<(IdentityResult, ApplicationUser?)> RegisterAsync(RegisterModel model, string roleName)
        {
            var userExists = await _userManager.FindByNameAsync(model.Username);
            if (userExists != null)
            {
                return (IdentityResult.Failed(new IdentityError { Description = "Tên đăng nhập đã tồn tại." }), null);
            }

            userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists != null)
            {
                 return (IdentityResult.Failed(new IdentityError { Description = "Email đã tồn tại." }), null);
            }

            var user = _mapper.Map<ApplicationUser>(model);
            user.SecurityStamp = Guid.NewGuid().ToString(); 

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                }
                await _userManager.AddToRoleAsync(user, roleName);

                // Create an empty cart for the new user
                var cart = new Cart { UserId = user.Id };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync(); // Save cart after user creation

                return (result, user);
            }
            return (result, null);
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginModel model)
        {
            ApplicationUser? user = null;
            if (!string.IsNullOrEmpty(model.LoginIdentifier))
            {
                user = await _userManager.FindByNameAsync(model.LoginIdentifier);
            }
            if (user == null && !string.IsNullOrEmpty(model.LoginIdentifier) && model.LoginIdentifier.Contains("@"))
            {
                user = await _userManager.FindByEmailAsync(model.LoginIdentifier);
            }

            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserName ?? string.Empty), 
                    new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), 
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }
                
                var jwtSettings = _configuration.GetSection("JwtSettings").Get<JwtSettings>();
                if (jwtSettings == null || string.IsNullOrEmpty(jwtSettings.Key)) {
                    throw new InvalidOperationException("JWT settings are not configured properly.");
                }

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key));

                var token = new JwtSecurityToken(
                    issuer: jwtSettings.Issuer,
                    audience: jwtSettings.Audience,
                    expires: DateTime.UtcNow.AddMinutes(jwtSettings.DurationInMinutes),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

                return new AuthResponseDto
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    Expiration = token.ValidTo,
                    UserId = user.Id,
                    Username = user.UserName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    Roles = userRoles
                };
            }
            return null; 
        }
    }
}