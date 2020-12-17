using System.Collections.Generic;

namespace Edreams.OutlookMiddleware.Mapping.Interfaces
{
    /// <summary>
    /// Interface defining a generic data mapper.
    /// </summary>
    /// <typeparam name="T1">The type of the first object.</typeparam>
    /// <typeparam name="T2">The type of the second object.</typeparam>
    public interface IMapper<T1, T2>
    {
        /// <summary>
        /// Maps an object defined by the first type to an object defined by the second type.
        /// </summary>
        /// <param name="value">The source object defined by the first type.</param>
        /// <returns>A destination object defined by the second type.</returns>
        T2 Map(T1 value);

        /// <summary>
        /// Maps a list of objects defined by the first type to a list of objects defined by the second type.
        /// </summary>
        /// <param name="values">The list of source objects defined by the first type.</param>
        /// <returns>A list of destination objects defined by the second type.</returns>
        IList<T2> Map(IList<T1> values);
    }
}