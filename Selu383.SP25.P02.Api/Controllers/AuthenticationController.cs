using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Threading.Tasks;
using Selu383.SP25.P02.Api.Models;
using Selu383.SP25.P02.Api.DTOs;

namespace Selu383.SP25.P02.Api.Controllers { }

[Route("api/authentication")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AuthenticationController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var user = await _userManager.FindByNameAsync(loginDto.UserName);
        if (user == null)
            return BadRequest("Invalid username or password.");

        var result = await _signInManager.PasswordSignInAsync(user, loginDto.Password, true, false);
        if (!result.Succeeded)
            return BadRequest("Invalid username or password.");

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties { IsPersistent = true };

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

        return Ok(new { Message = "Login successful." });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Ok(new { Message = "Logged out successfully." });
    }
}
