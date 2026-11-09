using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Schedule.API.Models;
using Schedule.API.Services;
using Schedule.Shared;
using Schedule.Shared.Models;

namespace Schedule.API.Controllers;

[ApiController]
[Route("[controller]")]
public class ScheduleController(Repository repository, FilterService filter) : ControllerBase
{
    [HttpGet]
    [EnableRateLimiting("calendar")]
    public async Task<IActionResult> Get([FromQuery] Dictionary<string, string> query)
    {
        var res = await filter.FilterLessons(query);

        return Ok(res.Select(LessonDto.FromLesson));
    }
}