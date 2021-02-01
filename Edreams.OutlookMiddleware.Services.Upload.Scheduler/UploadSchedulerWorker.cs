using System;
using System.Threading;
using System.Threading.Tasks;
using Edreams.OutlookMiddleware.Common.Configuration.Interfaces;
using Edreams.OutlookMiddleware.Common.ServiceBus.Contracts;
using Edreams.OutlookMiddleware.Common.ServiceBus.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Edreams.OutlookMiddleware.Services.Upload.Scheduler
{
    public class UploadSchedulerWorker : BackgroundService
    {
        private readonly IServiceBusHandler _serviceBusHandler;
        private readonly IEdreamsConfiguration _configuration;
        private readonly ILogger<UploadSchedulerWorker> _logger;

        public UploadSchedulerWorker(
            IServiceBusHandler serviceBusHandler,
            IEdreamsConfiguration configuration,
            ILogger<UploadSchedulerWorker> logger)
        {
            _serviceBusHandler = serviceBusHandler;
            _configuration = configuration;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace current testing code with actual implementation.
            while (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(10000, cancellationToken);

                ServiceBusMessage<Guid> serviceBusMessage = new ServiceBusMessage<Guid>
                {
                    CorrelationId = Guid.NewGuid(),
                    Data = Guid.NewGuid(),
                    QueuedOn = DateTime.UtcNow
                };

                await _serviceBusHandler.PostMessage(_configuration.ServiceBusQueueName, serviceBusMessage, cancellationToken);
            }
        }
    }
}