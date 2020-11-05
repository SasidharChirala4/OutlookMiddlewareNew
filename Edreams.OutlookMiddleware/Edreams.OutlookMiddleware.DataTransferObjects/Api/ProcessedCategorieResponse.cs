using Edreams.OutlookMiddleware.DataTransferObjects.Api.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Edreams.OutlookMiddleware.DataTransferObjects.Api
{
    /// <summary>
    /// Response class for Categories
    /// </summary>
    public class ProcessedCategoriesResponse : Response
    {
        /// <summary>
        /// Response status of the set categeroies
        /// </summary>
        public bool Success { get; set; }
    }
}
