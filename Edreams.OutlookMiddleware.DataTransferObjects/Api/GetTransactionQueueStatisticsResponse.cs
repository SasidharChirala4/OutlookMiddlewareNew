using Edreams.OutlookMiddleware.DataTransferObjects.Api.Base;

namespace Edreams.OutlookMiddleware.DataTransferObjects.Api
{
    public class GetTransactionQueueStatisticsResponse : Response
    {
        public long ActiveMessageCount { get; set; }
        public long ScheduledMessageCount { get; set; }
        public long DeadLetterMessageCount { get; set; }
    }
}