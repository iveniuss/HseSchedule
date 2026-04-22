using Microsoft.AspNetCore.Mvc;
using ScheduleAPI.Models;
using Shared;

namespace SсheduleAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class ScheduleController(Repository repository) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(string groupName)
    {
        var lessons = await repository.GetGroupSchedule(groupName);
        
        return Ok(lessons.Select(LessonDto.FromLesson));
    }
}