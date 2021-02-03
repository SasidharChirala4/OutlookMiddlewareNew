using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Edreams.OutlookMiddleware.BusinessLogic.Interfaces;
using Edreams.OutlookMiddleware.Common.Configuration.Interfaces;
using Edreams.OutlookMiddleware.Common.Helpers.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Edreams.OutlookMiddleware.Services.Cleanup.Workers
{
    public class PreloadedFilesCleanupWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IEdreamsConfiguration _configuration;
        private readonly ITimeHelper _timeHelper;
        private readonly ILogger<PreloadedFilesCleanupWorker> _logger;

        public PreloadedFilesCleanupWorker(
            IServiceScopeFactory serviceScopeFactory,
            IEdreamsConfiguration configuration,
            ITimeHelper timeHelper,
            ILogger<PreloadedFilesCleanupWorker> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _configuration = configuration;
            _timeHelper = timeHelper;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("PreloadedFiles-CleanupWorker STARTED");
            while (!cancellationToken.IsCancellationRequested)
            {
                // Cleanup worker needs to be executed in non-working hours only, 
                // Start and End Time for the worker needs to be taken from the configuration
                TimeSpan startTime = _configuration.PreloadedFilesWorkerScheduleStartTime;
                TimeSpan stopTime = _configuration.PreloadedFilesWorkerScheduleEndTime;
                if (!_timeHelper.IsGivenTimeWithinTimeSpan(DateTime.UtcNow, startTime, stopTime))
                {
                    continue;
                }
                // Get the scheduling interval in seconds from the application
                // configuration and convert to milliseconds.
                int schedulingInterval = _configuration.CleanupWorkerIntervalInSeconds * 1000;

                // Start a stopwatch for future reference when calculating the time we need to delay.
                Stopwatch stopwatch = Stopwatch.StartNew();
                try
                {
                    using IServiceScope scope = _serviceScopeFactory.CreateScope();
                    ICleanupManager cleanupLogic = scope.ServiceProvider.GetService<ICleanupManager>();

                    int workDone = await cleanupLogic.CleanupPreloadedFiles();
                   _logger.LogInformation($"PreloadedFilesExpirationWorker: {workDone} records are cleaned in {stopwatch.ElapsedMilliseconds}ms!");
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
                        await Task.Delay(schedulingInterval, cancellationToken);
                    }
                }
            }
            _logger.LogInformation("PreloadedFiles-CleanupWorker STOPPED");
        }
    }
}