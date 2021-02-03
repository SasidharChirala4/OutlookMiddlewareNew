using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Edreams.OutlookMiddleware.BusinessLogic.Interfaces;
using Edreams.OutlookMiddleware.Common.Configuration.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Edreams.OutlookMiddleware.Services.Cleanup.Workers
{
    public class PreloadedFilesCleanupWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IEdreamsConfiguration _configuration;
        private readonly ILogger<PreloadedFilesCleanupWorker> _logger;

        public PreloadedFilesCleanupWorker(
            IServiceScopeFactory serviceScopeFactory,
            IEdreamsConfiguration configuration,
            ILogger<PreloadedFilesCleanupWorker> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _configuration = configuration;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("PreloadedFiles Expiration Worker STARTED");
            while (!cancellationToken.IsCancellationRequested)
            {
                //Cleanup worker needs to be executed in non-working hours only, 
                //Start and End Time for the worker are configured
                if (!ShouldProcess())
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
                    _logger.LogInformation($"PreloadedFilesExpirationWorker: {workDone} records are expired!{DateTime.Now}");
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
            _logger.LogInformation("PreloadedFiles Expiration Worker STOPPED");
        }

        /// <summary>
        /// Method to know if the worker should process or not based on configured start and end Time
        /// </summary>
        /// <returns></returns>
        private bool ShouldProcess()
        {
            TimeSpan startTimeSpan = _configuration.PreloadedFilesWorkerScheduleStartTime;
            TimeSpan stopTimeSpan = _configuration.PreloadedFilesWorkerScheduleEndTime;

            if (startTimeSpan == stopTimeSpan)
            {
                return false;
            }

            DateTime now = DateTime.UtcNow;
            DateTime start;
            DateTime stop;

            // 21:00 - 06:00
            if (startTimeSpan > stopTimeSpan)
            {
                // <22:00>
                if (now.TimeOfDay >= stopTimeSpan)
                {
                    start = new DateTime(
                        now.Year, now.Month, now.Day,
                        startTimeSpan.Hours, startTimeSpan.Minutes, startTimeSpan.Seconds);
                    stop = new DateTime(
                        now.Year, now.Month, now.Day,
                        stopTimeSpan.Hours, stopTimeSpan.Minutes, stopTimeSpan.Seconds).AddDays(1);
                }
                // <01:00>
                else
                {
                    start = new DateTime(
                        now.Year, now.Month, now.Day,
                        startTimeSpan.Hours, startTimeSpan.Minutes, startTimeSpan.Seconds).AddDays(-1);

                    stop = new DateTime(
                        now.Year, now.Month, now.Day,
                        stopTimeSpan.Hours, stopTimeSpan.Minutes, stopTimeSpan.Seconds);
                }
            }
            // 06:00 - 21:00
            else
            {
                start = new DateTime(
                    now.Year, now.Month, now.Day,
                    startTimeSpan.Hours, startTimeSpan.Minutes, startTimeSpan.Seconds);

                stop = new DateTime(
                    now.Year, now.Month, now.Day,
                    stopTimeSpan.Hours, stopTimeSpan.Minutes, stopTimeSpan.Seconds);
            }

            return now >= start && now <= stop;
        }
    }
}