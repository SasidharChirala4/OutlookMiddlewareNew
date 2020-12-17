using Edreams.OutlookMiddleware.DataTransferObjects.Api.Base;

namespace Edreams.OutlookMiddleware.DataTransferObjects.Api
{
    /// <summary>
    /// Response class for Categories
    /// </summary>
    public class UpdatePendingCategoriesResponse : Response
    {
        /// <summary>
        /// Response status of the set categeroies
        /// </summary>
        public bool Success { get; set; }
    }
}
