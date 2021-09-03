namespace Edreams.OutlookMiddleware.Common.ServiceBus.Contracts
{
    public class ServiceBusQueueStatistics
    {
        public long ActiveMessageCount { get; set; }
        public long ScheduledMessageCount { get; set; }
        public long DeadLetterMessageCount { get; set; }
    }
}