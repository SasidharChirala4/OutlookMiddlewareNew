using System.Collections.Generic;
using Edreams.Common.Web.Contracts;

namespace Edreams.OutlookMiddleware.DataTransferObjects.Api
{

    /// <summary>
    /// Response class for GetCategories
    /// </summary>
    public class GetPendingCategoriesResponse : Response
    {
        /// <summary>
        ///  List of Categories Request
        /// </summary>
        public List<CategorizationRequest> CategorizationRequests { get; set; }
    }
}