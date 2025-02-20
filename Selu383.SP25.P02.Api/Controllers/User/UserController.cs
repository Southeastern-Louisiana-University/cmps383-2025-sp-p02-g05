using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Selu383.SP25.P02.Api.Features.Roles;
using Selu383.SP25.P02.Api.Features.Users;

namespace Selu383.SP25.P02.Api.Controllers
{
    [Route("api/users")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;

        public UsersController(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserDto createUserDto)
        {
            
            if (createUserDto == null || string.IsNullOrEmpty(createUserDto.Username) ||
                string.IsNullOrEmpty(createUserDto.Password) || createUserDto.Roles == null || !createUserDto.Roles.Any())
            {
                return BadRequest("All fields are required, and at least one role must be provided.");
            }

           
            var existingUser = await _userManager.FindByNameAsync(createUserDto.Username);
            if (existingUser != null)
            {
                return BadRequest("Username already exists.");
            }

            
            foreach (var role in createUserDto.Roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    return BadRequest($"Role '{role}' does not exist.");
                }
            }

           
            var user = new User { UserName = createUserDto.Username };
            var result = await _userManager.CreateAsync(user, createUserDto.Password);

            
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.Select(e => e.Description));
            }

            
            await _userManager.AddToRolesAsync(user, createUserDto.Roles);

            
            return Ok(new UserDto
            {
                Id = user.Id,
                Username = user.UserName,
                Roles = createUserDto.Roles
            });
        }
    }
}