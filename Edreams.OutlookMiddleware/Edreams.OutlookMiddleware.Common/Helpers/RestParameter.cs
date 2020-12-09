using RestSharp;

namespace Edreams.OutlookMiddleware.Common.Helpers
{
    /// <summary>
    /// RestSharp Parameter Object
    /// </summary>
    public class RestParameter
    {
        /// <summary>
        /// Name of the parameter
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Value of the parameter
        /// </summary>
        public object Value { get; set; }
        /// <summary>
        /// Type of the parameter
        /// </summary>
        public ParameterType Type { get; set; }
        /// <summary>
        /// Body parameter data type
        /// </summary>
        public DataFormat DataFormat { get; set; }
    }
}
