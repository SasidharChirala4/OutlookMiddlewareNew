using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Edreams.Common.AzureServiceBus.Interfaces;
using Edreams.Common.Exceptions;
using Edreams.Common.Logging.Interfaces;
using Edreams.OutlookMiddleware.BusinessLogic.Interfaces;
using Edreams.OutlookMiddleware.Common.Configuration.Interfaces;
using Edreams.OutlookMiddleware.Common.Helpers.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Edreams.OutlookMiddleware.Services.Notification
{
    public class NotificationWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ITimeHelper _timeHelper;
        private readonly IEdreamsConfiguration _configuration;
        private readonly IEdreamsLogger<NotificationWorker> _logger;

        public NotificationWorker(
            IServiceScopeFactory serviceScopeFactory,
            ITimeHelper timeHelper,
            IEdreamsConfiguration configuration,
            IEdreamsLogger<NotificationWorker> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _configuration = configuration;
            _timeHelper = timeHelper;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Notification Service STARTED");
            // Run continuously as long as the Windows Service is running. If the Windows Service
            // is stopped, the cancellation token will be cancelled and this loop will be stopped.
            while (!stoppingToken.IsCancellationRequested)
            {
                // Get the scheduling interval in seconds from the application
                // configuration and convert to milliseconds.
                int schedulingInterval = _configuration.TransactionSchedulingIntervalInSeconds * 1000;

                // Start a stopwatch for future reference when calculating the time we need to delay.
                Stopwatch stopwatch = Stopwatch.StartNew();

                try
                {
                    using IServiceScope scope = _serviceScopeFactory.CreateScope();
                    INotificationManager notificationLogic = scope.ServiceProvider.GetService<INotificationManager>();
                    await notificationLogic.ProcessNotification();

                    _logger.LogInformation($"All pending Notifications processed in {stopwatch.ElapsedMilliseconds}ms!");
                }
                catch (EdreamsException ex)
                {
                    _logger.LogError(ex.Message);
                }

                // Stop the stopwatch and subtract the number of milliseconds it recorded from the scheduling
                // interval. This will make sure the time between scheduling transactions is always consistent
                // and independent from the time it takes to process scheduling a transaction.
                stopwatch.Stop();
                schedulingInterval -= (int)stopwatch.ElapsedMilliseconds;
                if (schedulingInterval > 0)
                {
                    await Task.Delay(schedulingInterval, stoppingToken);
                }
            }
            _logger.LogInformation("Notification Service STOPPED");
        }

    }
}