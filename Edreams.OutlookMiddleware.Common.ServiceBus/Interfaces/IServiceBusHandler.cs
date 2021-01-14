using System;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Edreams.OutlookMiddleware.Common.ServiceBus.Contracts;

namespace Edreams.OutlookMiddleware.Common.ServiceBus.Interfaces
{
    public interface IServiceBusHandler
    {
        /// <summary>
        /// Posts the message to the specified Azure ServiceBus queue.
        /// </summary>
        /// <typeparam name="T">The type of data that needs to be serialized into the Azure ServiceBus message.</typeparam>
        /// <param name="queueName">Name of the Azure ServiceBus queue to post a message to.</param>
        /// <param name="message">The Azure ServiceBus message that needs to be posted.</param>
        /// <param name="cancellationToken">A cancellation token that can cancel the request to Azure ServiceBus.</param>
        Task PostMessage<T>(string queueName, ServiceBusMessage<T> message, CancellationToken cancellationToken = default);

        /// <summary>
        /// Processes incoming messages from the specified Azure ServiceBus queue.
        /// </summary>
        /// <typeparam name="T">The type of data that needs to be deserialized from the Azure ServiceBus message.</typeparam>
        /// <param name="queueName">Name of the Azure ServiceBus queue to process the incoming message from.</param>
        /// <param name="onProcessing">An asynchronous method delegate that can process incoming messages.</param>
        /// <param name="onError">An optional asynchronous method delegate that can handle errors if they occur.</param>
        /// <param name="cancellationToken">A cancellation token that can cancel the request to Azure ServiceBus.</param>
        Task ProcessMessagesAsync<T>(string queueName, Func<ServiceBusMessage<T>, CancellationToken, Task> onProcessing, Func<ServiceBusReceivedMessage, Exception, CancellationToken, Task> onError = default, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets some messaging statistics for the specified Azure ServiceBus Queue.
        /// </summary>
        /// <param name="queueName">Name of the Azure Service Bus queue to get the statistics for.</param>
        /// <param name="cancellationToken">A cancellation token that can cancel the request to Azure ServiceBus.</param>
        /// <returns>A <see cref="ServiceBusQueueStatistics" /> object containing messaging statistics.</returns>
        Task<ServiceBusQueueStatistics> GetQueueStatistics(string queueName, CancellationToken cancellationToken = default);
    }
}