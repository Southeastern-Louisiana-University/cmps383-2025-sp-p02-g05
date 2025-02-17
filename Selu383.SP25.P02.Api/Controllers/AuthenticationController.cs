using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Selu383.SP25.P02.Api.DTOs;
using System.Threading.Tasks;
using Selu383.SP25.P02.Api.Models;

namespace Selu383.SP25.P02.Api.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<Role> _roleManager;

        public AuthenticationController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<Role> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        /// <summary>
        /// ✅ Get Current Logged-In User (Me)
        /// </summary>
        [HttpGet("me")] // 🔹 Follows "List" position (Getting user details)
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Roles = (string[])roles
            });
        }

        /// <summary>
        /// ✅ Login (Creates a new authentication session)
        /// </summary>
        [HttpPost("login")] // 🔹 Follows "Create" position (Creating a session)
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.UserName);
            if (user == null)
            {
                return BadRequest("Invalid username or password.");
            }

            var result = await _signInManager.PasswordSignInAsync(user, loginDto.Password, true, false);
            if (!result.Succeeded)
            {
                return BadRequest("Invalid username or password.");
            }

            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Roles = (string[])roles
            });
        }

        /// <summary>
        /// ✅ Logout (Deletes authentication session)
        /// </summary>
        [HttpPost("logout")] // 🔹 Follows "Delete" position (Deleting session)
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok();
        }
    }
}
