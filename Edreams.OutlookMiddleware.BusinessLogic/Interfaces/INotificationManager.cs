using System.Threading.Tasks;

namespace Edreams.OutlookMiddleware.BusinessLogic.Interfaces
{
    public interface INotificationManager
    {
        /// <summary>
        /// Process all the pending notifications
        /// </summary>
        Task ProcessNotification();
    }
}
