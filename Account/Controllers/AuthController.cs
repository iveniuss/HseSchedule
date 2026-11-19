using Account.DTOs;
using Account.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Account.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class AuthController(
    UserManager<User> userManager,
    SignInManager<User> signInManager) : ControllerBase
{
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var user = new User
        {
            UserName = request.Login
        };

        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.Code, error.Description);
            }

            return ValidationProblem(ModelState);
        }

        await signInManager.SignInAsync(user, isPersistent: request.RememberMe);
        return Ok();
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await signInManager.PasswordSignInAsync(
            request.Login,
            request.Password,
            request.RememberMe,
            lockoutOnFailure: true);

        return result.Succeeded ? Ok() : Unauthorized();
    }
}

