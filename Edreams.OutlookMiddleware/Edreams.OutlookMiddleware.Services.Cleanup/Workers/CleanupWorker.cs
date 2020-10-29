using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Edreams.OutlookMiddleware.BusinessLogic.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Edreams.OutlookMiddleware.Services.Cleanup.Workers
{
    public class CleanupWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<CleanupWorker> _logger;

        public CleanupWorker(
            IServiceScopeFactory serviceScopeFactory,
            ILogger<CleanupWorker> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(5 * 1000, stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                using IServiceScope scope = _serviceScopeFactory.CreateScope();

                ICleanupManager cleanupLogic = scope.ServiceProvider.GetService<ICleanupManager>();

                Stopwatch sw = Stopwatch.StartNew();
                int workDone = await cleanupLogic.Cleanup();
                sw.Stop();

                _logger.LogInformation($"CleanupWorker: {workDone} records are cleaned in {sw.ElapsedMilliseconds}ms!");

                await Task.Delay(10 * 1000, stoppingToken);
            }
        }
    }
}