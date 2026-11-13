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
        var url =
            $"https://edu.hse.ru/calendar/export_execute.php?userid={userId}&authtoken={authToken}&preset_what=all&preset_time=recentupcoming";

        try
        {
            return Ok(await parser.GetDeadlinesAsync(url, cancellationToken));
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }
}