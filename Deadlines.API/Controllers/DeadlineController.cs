using Deadlines.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Deadlines.API.Controllers;

[ApiController]
[Route("[controller]")]
[EnableRateLimiting("deadlines")]
public class DeadlineController(DeadlineParser parser) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(string userId, string authToken, CancellationToken cancellationToken = default)
    {
        try
        {
            return Ok(await parser.GetDeadlinesAsync(userId, authToken, cancellationToken));
        }
        catch (Exception ex)
        {
            return StatusCode(400);
        }
    }
}