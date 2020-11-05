using Edreams.OutlookMiddleware.DataTransferObjects.Api;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Edreams.OutlookMiddleware.BusinessLogic.Interfaces
{
    public interface ICategoryManager
    {
        /// <summary>
        /// Method to get the pending categories for a given user.
        /// </summary>
        /// <param name="user">The UserName for whom the pending categories needs to be fetched.</param>
        /// <returns></returns>
        Task<GetCategoryResponse> GetPendingCategories(string user);

        /// <summary>
        /// Method to set the processed categories for a given user.
        /// </summary>
        /// <param name="categories">The list of categories to be processed.</param>
        /// <param name="user">The UserName for whom the categories should be processed.</param>
        /// <returns></returns>
        Task<ProcessedCategoriesResponse> SetProcessedCategories(List<ProcessedCategoriesRequest> categories, string user);
    }
}
