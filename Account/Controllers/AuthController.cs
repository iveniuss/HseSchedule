using Account.DTOs;
using Account.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Account.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class AuthController(
    UserManager<User> userManager,
    SignInManager<User> signInManager) : ControllerBase
{
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<LoginResponse>(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await signInManager.PasswordSignInAsync(
            request.Login,
            request.Password,
            request.RememberMe,
            lockoutOnFailure: true);

        if (!result.Succeeded)
        {
            if (result.IsLockedOut)
                return Unauthorized(new LoginResponse(LoginError.UserIsLockedOut));

            return Unauthorized(new LoginResponse(LoginError.WrongLoginOrPassword));
        }

        return Ok();
    }
}

