using Edreams.OutlookMiddleware.DataTransferObjects.Api;
using System.Threading.Tasks;

namespace Edreams.OutlookMiddleware.BusinessLogic.Interfaces
{
    public interface ICategoryManager
    {
        /// <summary>
        /// Method to get the pending categories for a given user.
        /// </summary>
        /// <param name="userPrincipalName">The UserName for whom the pending categories needs to be fetched.</param>
        /// <returns></returns>
        Task<GetPendingCategoriesResponse> GetPendingCategories(string userPrincipalName);

        /// <summary>
        /// Method to update the pending categories for a given user.
        /// </summary>
        /// <param name="updatePendingCategoriesRequest"></param>
        /// <returns></returns>
        Task<UpdatePendingCategoriesResponse> UpdatePendingCategories(UpdatePendingCategoriesRequest updatePendingCategoriesRequest);
    }
}
