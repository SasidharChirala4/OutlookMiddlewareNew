using System;
using System.Threading;
using System.Threading.Tasks;
using Edreams.Common.Logging.Interfaces;
using Microsoft.Extensions.Hosting;

namespace Edreams.OutlookMiddleware.Services.Notification
{
    public class Worker : BackgroundService
    {
        private readonly IEdreamsLogger<Worker> _logger;

        public Worker(IEdreamsLogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", new object[] { DateTimeOffset.Now });
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}