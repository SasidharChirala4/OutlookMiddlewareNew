using Edreams.OutlookMiddleware.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Edreams.OutlookMiddleware.DataTransferObjects
{
    /// <summary>
    /// Contract that represents MetaDeta Details
    /// </summary>
    public class MetaDataDto
    {
        /// <summary>
        /// MetaData Key
        /// </summary>
        public string PropertyName { get; set; }
        /// <summary>
        /// MetaData Value 
        /// </summary>
        public string PropertyValue { get; set; }

    }
}
