using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Edreams.OutlookMiddleware.BusinessLogic.Interfaces;
using Edreams.OutlookMiddleware.Common.Configuration.Interfaces;
using Edreams.OutlookMiddleware.Common.Helpers.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Edreams.Common.Logging.Interfaces;
namespace Edreams.OutlookMiddleware.Services.Cleanup.Workers
{
    public class PreloadedFilesCleanupWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IEdreamsConfiguration _configuration;
        private readonly ITimeHelper _timeHelper;
        private readonly IEdreamsLogger<PreloadedFilesCleanupWorker> _logger;

        public PreloadedFilesCleanupWorker(
            IServiceScopeFactory serviceScopeFactory,
            IEdreamsConfiguration configuration,
            ITimeHelper timeHelper,
            IEdreamsLogger<PreloadedFilesCleanupWorker> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _configuration = configuration;
            _timeHelper = timeHelper;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            TimeSpan startTime = _configuration.PreloadedFilesWorkerScheduleStartTime;
            TimeSpan stopTime = _configuration.PreloadedFilesWorkerScheduleStopTime;

            // Get the scheduling interval in seconds from the application
            // configuration and convert to milliseconds.
            int schedulingInterval = _configuration.CleanupWorkerIntervalInSeconds * 1000;

            _logger.LogInformation("PreloadedFilesCleanupWorker STARTED");
            while (!stoppingToken.IsCancellationRequested)
            {
                // Start a stopwatch for future reference when calculating the time we need to delay.
                Stopwatch stopwatch = Stopwatch.StartNew();
             
                try
                {

                    // Cleanup worker needs to be executed in non-working hours only, 
                    // This is based on the configured timespan and the current execution time
                    if (_timeHelper.IsGivenTimeWithinTimeSpan(DateTime.UtcNow, startTime, stopTime))
                    {
                        using IServiceScope scope = _serviceScopeFactory.CreateScope();
                        ICleanupManager cleanupLogic = scope.ServiceProvider.GetService<ICleanupManager>(); 
                        int workDone = await cleanupLogic.CleanupPreloadedFiles();

                       _logger.LogInformation($"PreloadedFilesCleanupWorker: {workDone} records are cleaned in {stopwatch.ElapsedMilliseconds}ms!");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }
                finally
                {
                    // Stop the stopwatch and subtract the number of milliseconds it recorded from the scheduling interval.
                    stopwatch.Stop();
                    schedulingInterval -= (int)stopwatch.ElapsedMilliseconds;
                    if (schedulingInterval > 0)
                    {
                        await Task.Delay(schedulingInterval, stoppingToken);
                    }
                }
            }
            _logger.LogInformation("PreloadedFilesCleanupWorker STOPPED");
        }
    }
}
