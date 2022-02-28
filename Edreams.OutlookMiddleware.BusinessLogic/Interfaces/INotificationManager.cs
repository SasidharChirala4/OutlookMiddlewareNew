using System;
using System.Threading.Tasks;

namespace Edreams.OutlookMiddleware.BusinessLogic.Interfaces
{
    public interface INotificationManager
    {
        /// <summary>
        /// Creates email notification 
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns></returns>
        Task CreateNotification(Guid batchId);

        /// <summary>
        /// Process all the pending notifications
        /// </summary>
        Task ProcessNotifications();
    }
}
