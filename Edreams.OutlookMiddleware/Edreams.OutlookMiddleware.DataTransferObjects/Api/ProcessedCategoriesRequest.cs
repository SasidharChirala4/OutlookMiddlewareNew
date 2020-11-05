using Edreams.OutlookMiddleware.Model.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Edreams.OutlookMiddleware.DataTransferObjects.Api
{
    /// <summary>
    /// Request for Process Categories
    /// </summary>
    public class ProcessedCategoriesRequest
    {
        /// <summary>
        /// Message Id of the Internet
        /// </summary>
        public string InternetMessageId { get; set; }
        /// <summary>
        /// IsCompose
        /// </summary>
        public bool IsCompose { get; set; }
        /// <summary>
        /// Request type of the Catergorie
        /// </summary>
        public CategorizationRequestType CategorizationRequestType { get; set; }
    }
}
