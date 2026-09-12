using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using ScheduleAPI.Models;
using Shared;

namespace SсheduleAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class ScheduleController(Repository repository) : ControllerBase
{
    [HttpGet]
    [EnableRateLimiting("calendar")]
    public async Task<IActionResult> Get([FromQuery] Dictionary<string, string> query)
    {
        var lessons = await repository.GetGroupSchedule(groupName);
        
        return Ok(lessons.Select(LessonDto.FromLesson));
    }
}