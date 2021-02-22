using System;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Edreams.OutlookMiddleware.Common.Configuration.Interfaces;
using Edreams.Common.AzureServiceBus.Contracts;
using Edreams.Common.AzureServiceBus.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Edreams.OutlookMiddleware.DataTransferObjects;
using Edreams.OutlookMiddleware.Services.Upload.Engine.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Edreams.OutlookMiddleware.Services.Upload.Engine
{
    public class UploadEngineWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IServiceBusHandler _serviceBusHandler;
        private readonly IEdreamsConfiguration _configuration;
        private readonly ILogger<UploadEngineWorker> _logger;

        public UploadEngineWorker(
            IServiceScopeFactory serviceScopeFactory,
            IServiceBusHandler serviceBusHandler,
            IEdreamsConfiguration configuration,
            ILogger<UploadEngineWorker> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _serviceBusHandler = serviceBusHandler;
            _configuration = configuration;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Run continuously as long as the Windows Service is running. If the Windows Service
            // is stopped, the cancellation token will be cancelled and this loop will be stopped.
            while (!stoppingToken.IsCancellationRequested)
            {
                await _serviceBusHandler.ProcessMessagesAsync<TransactionMessage>(_configuration.ServiceBusQueueName,
                    _configuration.ServiceBusConnectionString, OnProcessing, OnError, stoppingToken);

                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }

        private async Task OnProcessing(ServiceBusMessage<TransactionMessage> message, CancellationToken cancellationToken)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            IUploadEngineProcessor uploadEngineProcessor = scope.ServiceProvider.GetService<IUploadEngineProcessor>();

            await uploadEngineProcessor.Process(message.Data);
        }



        private Task OnError(ServiceBusReceivedMessage serviceBusMessage, Exception ex, CancellationToken cancellationToken)
        {
            _logger.LogError($"MESSAGE {serviceBusMessage.CorrelationId} THREW AN EXCEPTION '{ex.Message}' !!!");

            return Task.CompletedTask;
        }
    }
}