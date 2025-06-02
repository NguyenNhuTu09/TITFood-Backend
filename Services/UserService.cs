using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TITFood_Backend.Entities;
using TITFood_Backend.Interfaces;
using TITFood_Backend.Models;

namespace TITFood_Backend.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public UserService(UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<UserDto?> GetUserByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return null;

            var userDto = _mapper.Map<UserDto>(user);
            userDto.Roles = await _userManager.GetRolesAsync(user);
            return userDto;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            var userDtos = new List<UserDto>();
            foreach (var user in users)
            {
                var userDto = _mapper.Map<UserDto>(user);
                userDto.Roles = await _userManager.GetRolesAsync(user);
                userDtos.Add(userDto);
            }
            return userDtos;
        }
        
        public async Task<IdentityResult> UpdateUserAsync(string userId, UpdateUserDto updateUserDto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });
            }

            // Map only non-null properties from DTO to user entity
            user.FullName = updateUserDto.FullName ?? user.FullName;
            user.PhoneNumber = updateUserDto.PhoneNumber ?? user.PhoneNumber;
            user.Address = updateUserDto.Address ?? user.Address;
            // Add other properties if needed

            var result = await _userManager.UpdateAsync(user);
            return result;
        }
    }
}
