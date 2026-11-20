using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Account.Models;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<User>(options)
{
    public DbSet<MainGroup> MainGroups { get; set; }
    public DbSet<Track> Tracks { get; set; }
    public DbSet<OptGroup> OptGroups { get; set; }
    public DbSet<UserOpt> UserOpts { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<UserOpt>()
            .HasKey(userOpt => new { userOpt.UserId, userOpt.OptGroupId });

        builder.Entity<User>()
            .HasOne(u => u.MainGroup)
            .WithMany(g => g.Users)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<User>()
            .HasOne(u => u.Track)
            .WithMany(t => t.Users)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<UserOpt>()
            .HasOne(uo => uo.User)
            .WithMany(u => u.UserOpts)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<UserOpt>()
            .HasOne(uo => uo.OptGroup)
            .WithMany(og => og.UserOpts)
            .OnDelete(DeleteBehavior.Cascade);

    }
}
