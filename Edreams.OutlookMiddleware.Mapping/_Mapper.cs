using System.Collections.Generic;
using System.Linq;
using Edreams.OutlookMiddleware.Mapping.Interfaces;

namespace Edreams.OutlookMiddleware.Mapping
{
    /// <summary>
    /// Abstract class Mapper to map 1 object to another and/or back
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public abstract class Mapper<T1, T2> : IMapper<T1, T2>
    {
        /// <summary>
        /// Maps an object defined by the first type to an object defined by the second type.
        /// </summary>
        /// <param name="value">The source object defined by the first type.</param>
        /// <returns>A destination object defined by the second type.</returns>
        public abstract T2 Map(T1 value);

        /// <summary>
        /// Maps a list of objects defined by the first type to a list of objects defined by the second type.
        /// </summary>
        /// <param name="values">The list of source objects defined by the first type.</param>
        /// <returns>A list of destination objects defined by the second type.</returns>
        public IList<T2> Map(IList<T1> values)
        {
            return values?.Select(Map).ToList();
        }
    }
}