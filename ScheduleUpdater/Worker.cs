using System.Runtime.CompilerServices;
using ScheduleUpdater.Services;
using Shared;

namespace ScheduleUpdater;

public class Worker(
    ILogger<Worker> logger,
    ParserService parser,
    ExcelDownloader downloader,
    IServiceScopeFactory scopeFactory
    ) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (logger.IsEnabled(LogLevel.Information))
                    logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                using var scope = scopeFactory.CreateScope();
                var repository = scope.ServiceProvider.GetRequiredService<Repository>();

                var workbooks = await downloader.DownloadWorkbook();
                if (logger.IsEnabled(LogLevel.Information))
                    logger.LogInformation("Workbooks downloaded");

                var lessonsAdded = 0;
                foreach (var book in workbooks)
                    lessonsAdded += await parser.UpdateDatabase(book);


                if (logger.IsEnabled(LogLevel.Information))
                    logger.LogInformation("{lessonsAdded} lessons added", lessonsAdded);

                var deleted = await repository.ClearOldData();
                if (logger.IsEnabled(LogLevel.Information))
                    logger.LogInformation("{deleted} lessons deleted", deleted);

                if (logger.IsEnabled(LogLevel.Information))
                    logger.LogInformation("Database updated");
            }
            catch (Exception e)
            {
                if (logger.IsEnabled(LogLevel.Error))
                    logger.LogError(e, "Error during update: {}", e.Message);
            }

            await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
        }
    }
}