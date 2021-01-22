using System;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Edreams.OutlookMiddleware.Common.Configuration.Interfaces;
using Edreams.OutlookMiddleware.Common.ServiceBus.Contracts;
using Edreams.OutlookMiddleware.Common.ServiceBus.Interfaces;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.Extensions.Logging;

namespace Edreams.OutlookMiddleware.Common.ServiceBus
{
    public class ServiceBusHandler : IServiceBusHandler
    {
        private readonly IEdreamsConfiguration _configuration;
        private readonly ILogger<ServiceBusHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceBusHandler" /> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="logger">The logger.</param>
        public ServiceBusHandler(
            IEdreamsConfiguration configuration,
            ILogger<ServiceBusHandler> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Posts the message to the specified Azure ServiceBus queue.
        /// </summary>
        /// <typeparam name="T">The type of data that needs to be serialized into the Azure ServiceBus message.</typeparam>
        /// <param name="queueName">Name of the Azure ServiceBus queue to post a message to.</param>
        /// <param name="message">The Azure ServiceBus message that needs to be posted.</param>
        /// <param name="cancellationToken">A cancellation token that can cancel the request to Azure ServiceBus.</param>
        public async Task PostMessage<T>(string queueName, ServiceBusMessage<T> message, CancellationToken cancellationToken = default)
        {
            await using ServiceBusClient client = new ServiceBusClient(_configuration.ServiceBusConnectionString);
            await using ServiceBusSender sender = client.CreateSender(queueName);

            ServiceBusMessage serviceBusMessage = new ServiceBusMessage
            {
                Body = BinaryData.FromObjectAsJson(message),
                CorrelationId = $"{message.CorrelationId}",
            };

            await sender.SendMessageAsync(serviceBusMessage, cancellationToken);
        }

        /// <summary>
        /// Processes incoming messages from the specified Azure ServiceBus queue.
        /// </summary>
        /// <typeparam name="T">The type of data that needs to be deserialized from the Azure ServiceBus message.</typeparam>
        /// <param name="queueName">Name of the Azure ServiceBus queue to process the incoming message from.</param>
        /// <param name="onProcessing">An asynchronous method delegate that can process incoming messages.</param>
        /// <param name="onError">An optional asynchronous method delegate that can handle errors if they occur.</param>
        /// <param name="cancellationToken">A cancellation token that can cancel the request to Azure ServiceBus.</param>
        public async Task ProcessMessagesAsync<T>(string queueName,
            Func<ServiceBusMessage<T>, CancellationToken, Task> onProcessing,
            Func<ServiceBusReceivedMessage, Exception, CancellationToken, Task> onError = default,
            CancellationToken cancellationToken = default)
        {
            await using ServiceBusClient client = new ServiceBusClient(_configuration.ServiceBusConnectionString);
            await using ServiceBusReceiver receiver = client.CreateReceiver(queueName);

            // This foreach keeps an asynchronous stream alive and will wait on incoming Azure ServiceBus messages.
            await foreach (ServiceBusReceivedMessage serviceBusMessage in receiver.ReceiveMessagesAsync(cancellationToken))
            {
                try
                {
                    // Deserialize the message data from the Azure ServiceBus 
                    ServiceBusMessage<T> message = serviceBusMessage.Body.ToObjectFromJson<ServiceBusMessage<T>>();
                    
                    // Call the asynchronous method delegate to process the deserialized message, but
                    // don't await its result right now. We need to keep an eye on its locking timeout.
                    Task processingTask = onProcessing(message, cancellationToken);

                    // While processing is still ongoing...
                    while (!processingTask.IsCompleted)
                    {
                        // Take a short pause.
                        await Task.Delay(100, cancellationToken);

                        // Renew the Azure ServiceBus message lock if the remaining time
                        // on the timeout is less than a minute.
                        if ((serviceBusMessage.LockedUntil - DateTime.UtcNow).TotalMinutes < 1)
                        {
                            await receiver.RenewMessageLockAsync(serviceBusMessage, cancellationToken);
                        }
                    }

                    // If the while loop has passed and the processing was finished: the Azure ServiceBus
                    // message can be completed, releasing the lock and removing the message from the queue.
                    await receiver.CompleteMessageAsync(serviceBusMessage, cancellationToken);
                }
                catch (Exception ex)
                {
                    // If an Exception occurred and there is a method delegate registered, it should
                    // be called with the raw Azure ServiceBus message and Exception details.
                    if (onError != null)
                    {
                        await onError(serviceBusMessage, ex, cancellationToken);
                    }

                    // The Azure ServiceBus message should be abandoned to release its lock and make it available for a retry.
                    await receiver.AbandonMessageAsync(serviceBusMessage, null, cancellationToken);
                }
            }
        }

        /// <summary>
        /// Gets some messaging statistics for the specified Azure ServiceBus Queue.
        /// </summary>
        /// <param name="queueName">Name of the Azure Service Bus queue to get the statistics for.</param>
        /// <param name="cancellationToken">A cancellation token that can cancel the request to Azure ServiceBus.</param>
        /// <returns>A <see cref="ServiceBusQueueStatistics" /> object containing messaging statistics.</returns>
        public async Task<ServiceBusQueueStatistics> GetQueueStatistics(string queueName, CancellationToken cancellationToken = default)
        {
            ManagementClient managementClient = new ManagementClient(_configuration.ServiceBusConnectionString);
            QueueRuntimeInfo queueRuntimeInfo = await managementClient.GetQueueRuntimeInfoAsync(queueName, cancellationToken);
            return new ServiceBusQueueStatistics
            {
                ActiveMessageCount = queueRuntimeInfo.MessageCountDetails.ActiveMessageCount,
                ScheduledMessageCount = queueRuntimeInfo.MessageCountDetails.ScheduledMessageCount,
                DeadLetterMessageCount = queueRuntimeInfo.MessageCountDetails.DeadLetterMessageCount
            };
        }
    }
}