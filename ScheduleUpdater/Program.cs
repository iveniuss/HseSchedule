using Microsoft.EntityFrameworkCore;
using ScheduleUpdater.Services;
using Shared.Models;

namespace ScheduleUpdater;

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