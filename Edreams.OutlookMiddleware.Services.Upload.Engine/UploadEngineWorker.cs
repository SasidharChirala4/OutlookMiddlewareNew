using System;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Edreams.OutlookMiddleware.Common.Configuration.Interfaces;
using Edreams.OutlookMiddleware.Common.ServiceBus.Contracts;
using Edreams.OutlookMiddleware.Common.ServiceBus.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Edreams.OutlookMiddleware.Services.Upload.Engine
{
    public class UploadEngineWorker : BackgroundService
    {
        private readonly IServiceBusHandler _serviceBusHandler;
        private readonly IEdreamsConfiguration _configuration;
        private readonly ILogger<UploadEngineWorker> _logger;

        public UploadEngineWorker(
            IServiceBusHandler serviceBusHandler,
            IEdreamsConfiguration configuration,
            ILogger<UploadEngineWorker> logger)
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
                await _serviceBusHandler.ProcessMessagesAsync<Guid>(
                    _configuration.ServiceBusQueueName, OnProcessing, OnError, cancellationToken);

                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, cancellationToken);
            }
        }

        private async Task OnProcessing(ServiceBusMessage<Guid> message, CancellationToken cancellationToken)
        {
            _logger.LogInformation("WORKING FOR 6 MINUTES ...");
            await Task.Delay(1000 * 60 * 6, cancellationToken);
            _logger.LogInformation($"MESSAGE {message.CorrelationId} SUCCESSFULLY RECEIVED !!!");
        }

        private Task OnError(ServiceBusReceivedMessage serviceBusMessage, Exception ex, CancellationToken cancellationToken)
        {
            _logger.LogError($"MESSAGE {serviceBusMessage.CorrelationId} THREW AN EXCEPTION '{ex.Message}' !!!");

            return Task.CompletedTask;
        }
    }
}