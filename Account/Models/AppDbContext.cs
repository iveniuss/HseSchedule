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
    }
}