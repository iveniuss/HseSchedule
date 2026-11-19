using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Account.DTOs;
using Account.Models;

namespace Account.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class ProfileController(AppDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new ProfileResponse(
            User.FindFirstValue(ClaimTypes.NameIdentifier),
            User.Identity?.Name));
    }
    
    [HttpPost("[action]")]
    public async Task<IActionResult> SelectGroup(string groupId)
    {
        var user = await dbContext.Users.FindAsync(
            User.FindFirstValue(ClaimTypes.NameIdentifier));

        if (user == null)
            return Unauthorized();

        if (await dbContext.MainGroups.FindAsync(groupId) == null)
            return BadRequest();
        
        user.MainGroupId = groupId;
        
        try
        {
            var res = await dbContext.SaveChangesAsync();
            return res > 0 ? Ok() : StatusCode(StatusCodes.Status500InternalServerError);
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}