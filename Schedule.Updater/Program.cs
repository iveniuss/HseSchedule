using Microsoft.EntityFrameworkCore;
using Schedule.Updater.Services;
using Schedule.Shared.Models;

namespace Schedule.Updater;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        
        builder.Services.AddDbContext<ScheduleContext>(options =>
            options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
        
        builder.Services.AddHostedService<Worker>();
        builder.Services.AddSingleton<ExcelDownloader>();
        builder.Services.AddSingleton<ParserService>();
        builder.Services.AddSingleton<HttpClient>();
        builder.Services.AddScoped<Shared.Repository>();

        var host = builder.Build();

        host.Run();
    }
}