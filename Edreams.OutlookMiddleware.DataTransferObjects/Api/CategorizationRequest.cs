using Edreams.Common.Web.Contracts;
using Edreams.OutlookMiddleware.Enums;

namespace Edreams.OutlookMiddleware.DataTransferObjects.Api
{
    /// <summary>
    /// Request class of Categorization
    /// </summary>
    public class CategorizationRequest : Request
    {
        /// <summary>
        /// Internet MessageId
        /// </summary>
        public string InternetMessageId { get; set; }
        /// <summary>
        /// IsCompose
        /// </summary>
        public bool IsCompose { get; set; }

        /// <summary>
        /// Request type of the Categorization
        /// </summary>
        public CategorizationRequestType CategorizationRequestType { get; set; }
    }
}