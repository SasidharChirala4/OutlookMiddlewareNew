using Edreams.OutlookMiddleware.DataTransferObjects.Api.Base;
using Edreams.OutlookMiddleware.Model.Enums;
using System.Collections.Generic;

namespace Edreams.OutlookMiddleware.DataTransferObjects.Api
{

    /// <summary>
    /// Response class for GetCategories
    /// </summary>
    public class GetCategoryResponse : Response
    {
        /// <summary>
        ///  List of Categories Request
        /// </summary>
        public List<CategorizationRequest> CategorizationRequests { get; set; }
    }
}
