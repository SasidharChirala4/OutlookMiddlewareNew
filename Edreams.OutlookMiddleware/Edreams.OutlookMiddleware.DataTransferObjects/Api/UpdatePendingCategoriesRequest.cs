using Edreams.OutlookMiddleware.DataTransferObjects.Api.Base;
using System.Collections.Generic;

namespace Edreams.OutlookMiddleware.DataTransferObjects.Api
{
    /// <summary>
    /// Request of UpdatePendingCategories
    /// </summary>
    public class UpdatePendingCategoriesRequest : Request
    {
        /// <summary>
        /// The list of categories to be processed
        /// </summary>
        public List<CategorizationRequest> CategorizationRequests { get; set; }

        /// <summary>
        /// The UserPricipalName for whom the categories should be processed
        /// </summary>
        public string UserPrincipalName { get; set; }
    }
}
