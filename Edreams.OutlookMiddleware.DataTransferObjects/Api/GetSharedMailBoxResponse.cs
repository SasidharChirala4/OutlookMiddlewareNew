using Edreams.Common.Web.Contracts;

namespace Edreams.OutlookMiddleware.DataTransferObjects.Api
{
    /// <summary>
    /// Response object for the GetSharedMailBox endpoint.
    /// </summary>
    public class GetSharedMailBoxResponse : Response
    {
        /// <summary>
        /// SharedMailBox EMail
        /// </summary>
        public string EmailAddress { get; set; }
    }
}