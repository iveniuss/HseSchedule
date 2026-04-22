using Microsoft.EntityFrameworkCore;

namespace Shared.Models;

public class ScheduleContext(DbContextOptions<ScheduleContext> options) : DbContext(options)
{
    public DbSet<Lesson> Schedule { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Lesson>()
            .Property(x => x.Id)
            .ValueGeneratedOnAdd();
        
        modelBuilder.Entity<Lesson>()
            .HasIndex(r => new { r.Name,
                r.Location,
                r.Teacher,
                r.GroupName,
                r.Subgroup,
                r.IsOptional,
                r.StartTime,
                r.EndTime,
                r.Link
            })
            .IsUnique();
    }
}