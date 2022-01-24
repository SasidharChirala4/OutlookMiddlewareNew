namespace Edreams.OutlookMiddleware.DataTransferObjects
{
    /// <summary>
    /// Contract that represents MetaDeta Details
    /// </summary>
    public class MetadataDto
    {
        /// <summary>
        /// Metadata Key
        /// </summary>
        public string PropertyName { get; set; }
        /// <summary>
        /// Metadata Value 
        /// </summary>
        public string PropertyValue { get; set; }
    }
}