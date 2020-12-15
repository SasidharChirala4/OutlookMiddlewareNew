using System.Threading;
using System.Threading.Tasks;
using Edreams.OutlookMiddleware.BusinessLogic.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Edreams.OutlookMiddleware.Services.Cleanup.Workers
{
    public class ExpirationWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<ExpirationWorker> _logger;

        public ExpirationWorker(
            IServiceScopeFactory serviceScopeFactory,
            ILogger<ExpirationWorker> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using IServiceScope scope = _serviceScopeFactory.CreateScope();

                ICleanupManager cleanupLogic = scope.ServiceProvider.GetService<ICleanupManager>();
                int workDone = await cleanupLogic.VerifyExpiration();

                _logger.LogInformation($"ExpirationWorker: {workDone} records are expired!");

                await Task.Delay(10 * 1000, stoppingToken);
            }
        }
    }
}