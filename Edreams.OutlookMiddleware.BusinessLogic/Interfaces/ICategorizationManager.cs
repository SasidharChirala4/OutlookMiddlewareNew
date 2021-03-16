using Edreams.OutlookMiddleware.DataTransferObjects.Api;
using Edreams.OutlookMiddleware.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Edreams.OutlookMiddleware.BusinessLogic.Interfaces
{
    public interface ICategorizationManager
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

        /// <summary>
        /// Method to add a category.
        /// </summary>
        /// <param name="internetMessageId">The internet message Id of the Email.</param>
        /// <param name="recipientsList">The list of recipients.</param>
        /// <param name="type">The type of the CategorizationRequest</param>
        /// <returns></returns>
        Task AddCategorizationRequest(string internetMessageId, List<string> recipientsList, CategorizationRequestType type);
    }
}
