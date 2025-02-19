using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Selu383.SP25.P02.Api.DTOs;
using Selu383.SP25.P02.Api.Models;
using Selu383.SP25.P02.Test.Dtos;
using System.Linq;
using System.Threading.Tasks;

namespace Selu383.SP25.P02.Api.Controllers
{
    [Route("api/users")]
    [ApiController]
    [Authorize(Roles = "Admin")] // ✅ Only Admins can create users
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<Role> _roleManager;

        public UsersController(UserManager<ApplicationUser> userManager, RoleManager<Role> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto createUserDto)
        {
            // ✅ Return 400 if request is invalid
            if (createUserDto == null || string.IsNullOrEmpty(createUserDto.UserName) ||
                string.IsNullOrEmpty(createUserDto.Password) || createUserDto.Roles == null || !createUserDto.Roles.Any())
            {
                return BadRequest("All fields are required, and at least one role must be provided.");
            }

            // ✅ Check if user already exists
            var existingUser = await _userManager.FindByNameAsync(createUserDto.UserName);
            if (existingUser != null)
            {
                return BadRequest("Username already exists.");
            }

            // ✅ Ensure all roles exist
            foreach (var role in createUserDto.Roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    return BadRequest($"Role '{role}' does not exist.");
                }
            }

            // ✅ Create new user
            var user = new ApplicationUser { UserName = createUserDto.UserName };
            var result = await _userManager.CreateAsync(user, createUserDto.Password);

            // ✅ Return 400 if user creation failed
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.Select(e => e.Description));
            }

            // ✅ Assign roles
            await _userManager.AddToRolesAsync(user, createUserDto.Roles);

            // ✅ Return 200 with user details
            return Ok(new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Roles = createUserDto.Roles
            });
        }
    }
}
