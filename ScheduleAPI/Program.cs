using Shared;
using Shared.Models;
using Microsoft.EntityFrameworkCore;
using ScheduleAPI.Services;

namespace ScheduleAPI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddDbContext<Shared.Models.ScheduleContext>(options =>
            options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddScoped<Repository>();
        builder.Services.AddScoped<FilterService>();
        builder.Services.AddScoped<CalendarService>();
        builder.Services.AddControllers();

        var app = builder.Build();

        // Ensure database is created and migrated
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<Shared.Models.ScheduleContext>();
            db.Database.Migrate();
        }

        app.MapControllers();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.Run();

    }
}