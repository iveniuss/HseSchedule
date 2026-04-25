using System.Text;
using Microsoft.AspNetCore.Mvc;
using ScheduleAPI.Services;

namespace ScheduleAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class CalendarController(
    FilterService filter,
    CalendarService calendar) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] Dictionary<string, string> query)
    {
        var res = await filter.FilterLessons(query);
        var ical = calendar.GetIcal(res);

        return File(Encoding.UTF8.GetBytes(ical), "text/calendar", "schedule.ics");
    }
}  