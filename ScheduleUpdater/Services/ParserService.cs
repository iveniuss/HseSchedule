using System.Globalization;
using System.Text.RegularExpressions;
using NPOI.SS.UserModel;
using Shared;
using Shared.Models;

namespace ScheduleUpdater.Services;

public class ParserService(IServiceScopeFactory scopeFactory, IConfiguration configuration)
{
    private enum LineType
    {
        Name,
        Description,
        Link,
        Empty
    }
    
    private class ParsedTeacherInfo
    {
        public string? Teacher { get; set; }
        public string? Location { get; set; }
        public int? Subgroup { get; set; }
    }
    
    private static readonly Regex TeacherRegex = 
        new Regex(@"[А-ЯЁ][а-яё]+\s+[А-ЯЁ]\.\s*[А-ЯЁ]?\.", RegexOptions.Compiled);
    
    private static readonly Regex LocationRegex =
        new Regex(@"\(([\p{L}\p{N}]+\[\d+\])(?:,\s*(\d+))?\)", RegexOptions.Compiled);
    
    private static readonly Regex LinkRegex = 
        new Regex(@"https?://", RegexOptions.Compiled);
    
    private static readonly Regex DateRegex = new Regex(@"[А-Я][а-я]+\n\d{2}\.\d{2}\.\d{4}", RegexOptions.Compiled);

    
    public async Task<int> UpdateDatabase(IWorkbook workbook)
    {
        var added = 0;
        
        foreach (var sheet in workbook)
        {
            using var scope = scopeFactory.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<Repository>();
            
            var groupNames = new List<string>();
            var headerEnded = false;
            var updateTime = DateTime.UtcNow.AddTicks(
                -(DateTime.UtcNow.Ticks % TimeSpan.TicksPerSecond)
            );
            foreach (var row in sheet)
            {
                if (!headerEnded && row.Cells[0].StringCellValue == "дни") //Строка с названиями групп
                {
                    for (int i = 2; i < row.LastCellNum; i++)
                    {
                        if (string.IsNullOrWhiteSpace(row.Cells[i].StringCellValue))
                            break;
                        groupNames.Add(row.Cells[i].StringCellValue);
                    }

                    headerEnded = true;
                }
                else if (headerEnded && DateRegex.IsMatch(row.Cells[0].StringCellValue))
                {
                    var dates = ParseTime(row.Cells[0].StringCellValue, row.Cells[1].StringCellValue);
                    
                    var lessons = GetLessons(row.Cells.GetRange(2, row.LastCellNum-2), groupNames, dates);

                    foreach (var lesson in lessons)
                    {
                        lesson.LastUpdate = updateTime;
                        var lessonId = await repository.GetLessonId(lesson);

                        if (lessonId == -1)
                        {
                            await repository.Add(lesson);
                            added++;
                        }
                        else
                            await repository.Touch(lessonId, updateTime);
                        
                    }
                }
            }
        }

        return added;
    }

    private List<Lesson> GetLessons(IList<ICell> cells, List<string> groupNames, DateTime[] dates)
    {
        var lessons = new List<Lesson>();

        for (var i = 0; i < groupNames.Count; i++)
        {
            lessons.AddRange(ParseCell(cells[i].StringCellValue, groupNames[i], dates));
        }
        
        return lessons;
    }

    private List<Lesson> ParseCell(string cellValue, string groupName, DateTime[] dates)
    {
        if (string.IsNullOrWhiteSpace(cellValue))
        {
            return [];
        }
        
        var substrings = cellValue.Split("\n", StringSplitOptions.RemoveEmptyEntries);
        var lessons = new List<Lesson>();
        var currentLesson = Lesson.Empty;

        foreach (var substring in substrings)
        {
            var trimmedSubstring = substring.Trim();
            
            switch (GetLineType(trimmedSubstring))
            {
                case LineType.Name:
                    if (!string.IsNullOrEmpty(currentLesson.Name))
                    {
                        lessons.Add(currentLesson);
                        currentLesson = Lesson.Empty;
                    }
                    currentLesson.Name = trimmedSubstring;
                    break;
                case LineType.Description:
                    var descriptionInfo = ParseDescription(trimmedSubstring);
                    currentLesson.Location = descriptionInfo.Location;
                    currentLesson.Subgroup = descriptionInfo.Subgroup;
                    currentLesson.Teacher = descriptionInfo.Teacher;
                    break;
                case LineType.Link:
                    currentLesson.Link = trimmedSubstring;
                    break;
                case LineType.Empty:
                    break;
            }
        }

        lessons.Add(currentLesson);
        
        foreach (var lesson in lessons)
        {
            lesson.GroupName = groupName;
            lesson.StartTime = dates[0];
            lesson.EndTime = dates[1];
            if ((configuration.GetSection("Optionals").Get<List<string>>() ?? []).Any(x => lesson.Name.Contains(x)))
                lesson.IsOptional = true;
        }
        
        

        return lessons;
    }
    
    private static ParsedTeacherInfo ParseDescription(string line)
    {
        var result = new ParsedTeacherInfo();

        if (string.IsNullOrWhiteSpace(line))
            return result;

        line = line.Trim();
        
        var teacherMatch = TeacherRegex.Match(line);
        if (teacherMatch.Success)
        {
            result.Teacher = teacherMatch.Groups[0].Value.Trim();
        }
        
        var locationMatch = LocationRegex.Match(line);
        if (locationMatch.Success)
        {
            result.Location = locationMatch.Groups[1].Value;

            if (locationMatch.Groups[2].Success)
            {
                result.Subgroup = int.Parse(locationMatch.Groups[2].Value);
            }
        }

        return result;
    }
    
    private static LineType GetLineType(string line)
    {
        if (string.IsNullOrWhiteSpace(line))
            return LineType.Empty;
        
        if (LinkRegex.IsMatch(line))
            return LineType.Link;
        
        if (TeacherRegex.IsMatch(line) && LocationRegex.IsMatch(line))
            return LineType.Description;
        
        if (LocationRegex.IsMatch(line))
            return LineType.Description;
        
        return LineType.Name;
    }

    private DateTime[] ParseTime(string dayString, string timeString)
    {
        // Очистка строк от лишних пробелов и переносов
        var cleanedDay = Regex.Replace(dayString, @"\s+", " ").Trim();
        var cleanedTime = Regex.Replace(timeString, @"\s+", " ").Trim();

        // Извлечение даты из dayString (формат: "Вторник 10.03.2026")
        var dateMatch = Regex.Match(cleanedDay, @"(\d{2}\.\d{2}\.\d{4})");
        if (!dateMatch.Success)
            throw new FormatException($"Date not found in dayString: {dayString}");

        var dateString = dateMatch.Groups[1].Value;
        var date = DateTime.ParseExact(dateString, "dd.MM.yyyy", CultureInfo.InvariantCulture);

        // Извлечение временного диапазона из timeString (формат: "1 8:10-9:30")
        var timeMatch = Regex.Match(cleanedTime, @"(\d{1,2}):(\d{2})-(\d{1,2}):(\d{2})");
        if (!timeMatch.Success)
            throw new FormatException($"Time range not found in timeString: {timeString}");

        var startHour = int.Parse(timeMatch.Groups[1].Value);
        var startMinute = int.Parse(timeMatch.Groups[2].Value);
        var endHour = int.Parse(timeMatch.Groups[3].Value);
        var endMinute = int.Parse(timeMatch.Groups[4].Value);

        var startDateTime = new DateTime(date.Year, date.Month, date.Day, startHour, startMinute, 0);
        var endDateTime = new DateTime(date.Year, date.Month, date.Day, endHour, endMinute, 0);

        return [startDateTime, endDateTime];
    }
}