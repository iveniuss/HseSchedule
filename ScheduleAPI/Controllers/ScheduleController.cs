using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using ScheduleAPI.Models;
using ScheduleAPI.Services;
using Shared;
using Shared.Models;

namespace ScheduleAPI.Controllers;

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