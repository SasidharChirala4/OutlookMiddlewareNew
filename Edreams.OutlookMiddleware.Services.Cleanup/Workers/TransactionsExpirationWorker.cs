using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Edreams.OutlookMiddleware.BusinessLogic.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Edreams.OutlookMiddleware.Common.Configuration.Interfaces;
using Edreams.Common.Logging.Interfaces;

namespace Edreams.OutlookMiddleware.Services.Cleanup.Workers
{
    public class TransactionsExpirationWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IEdreamsLogger<TransactionsExpirationWorker> _logger;
        private readonly IEdreamsConfiguration _configuration;

        public TransactionsExpirationWorker(
            IServiceScopeFactory serviceScopeFactory,
            IEdreamsConfiguration configuration,
            IEdreamsLogger<TransactionsExpirationWorker> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _configuration = configuration;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Transactions-ExpirationWorker STARTED");
            // Run continuously as long as the Windows Service is running. If the Windows Service
            // is stopped, the cancellation token will be cancelled and this loop will be stopped.
            while (!stoppingToken.IsCancellationRequested)
            {
                // Get the scheduling interval in seconds from the application
                // configuration and convert to milliseconds.
                int schedulingInterval = _configuration.ExpirationWorkerIntervalInSeconds * 1000;

                // Start a stopwatch for future reference when calculating the time we need to delay.
                Stopwatch stopwatch = Stopwatch.StartNew();

                try
                {
                    using IServiceScope scope = _serviceScopeFactory.CreateScope();
                    ICleanupManager cleanupLogic = scope.ServiceProvider.GetService<ICleanupManager>();                    
                    int workDone = await cleanupLogic.ExpireTransactions();
                    
                    _logger.LogInformation($"TransactionsExpirationWorker: {workDone} records are expired!");
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
            _logger.LogInformation("Transactions-ExpirationWorker STOPPED");
        }
    }
}