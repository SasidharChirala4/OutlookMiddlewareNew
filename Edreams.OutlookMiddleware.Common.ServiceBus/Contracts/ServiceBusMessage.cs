using System;

namespace Edreams.OutlookMiddleware.Common.ServiceBus.Contracts
{
    public class ServiceBusMessage<T>
    {
        public Guid CorrelationId { get; set; }
        public DateTime QueuedOn { get; set; }
        public T Data { get; set; }
    }
}