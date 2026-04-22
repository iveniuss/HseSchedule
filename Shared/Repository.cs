using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace Shared;

public class Repository
{
    private readonly ScheduleContext _dbContext;

    public Repository(ScheduleContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> Add(Lesson lesson)
    {
        try
        {
            await _dbContext.Schedule.AddAsync(lesson);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException)
        {
            return false;
        }
    }

    public async Task UpdateSchedule(IEnumerable<Lesson> newLessons)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM Schedule");
        await _dbContext.Schedule.AddRangeAsync(newLessons);
        await transaction.CommitAsync();
    }

    public async Task<List<Lesson>> GetGroupSchedule(string groupName)
    {
        return await _dbContext.Schedule.Where(r => r.GroupName == groupName).ToListAsync();
    }

    public async Task<int> GetLessonId(Lesson lesson)
    {
        var res =  await _dbContext.Schedule
            .FirstOrDefaultAsync(Lesson.IsDuplicateOf(lesson));
        
        return res?.Id ?? -1;
    }

    public async Task Touch(int id, DateTime updateTime)
    {
        await _dbContext.Schedule
            .Where(x => x.Id == id)
            .ExecuteUpdateAsync(s => s.SetProperty(p => p.LastUpdate, updateTime));
    }
    
    public async Task<int> ClearOldData()
    {
        DateTime threshold = DateTime.UtcNow.AddMinutes(-5);
        
        var res = await _dbContext.Schedule
            .Where(x => x.LastUpdate < threshold)
            .ExecuteDeleteAsync();

        return res;
    }
}