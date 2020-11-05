namespace Edreams.OutlookMiddleware.DataAccess.Repositories.Helpers
{
    /// <summary>
    /// Class representing a limit for a Repository query.
    /// </summary>
    public class Limit
    {
        /// <summary>
        /// Bypasses a specified number of elements in a sequence and then returns the remaining elements.
        /// </summary>
        public int Skip { get; }

        /// <summary>
        /// Returns a specified number of contiguous elements from the start of a sequence.
        /// </summary>
        public int Take { get; }

        /// <summary>
        /// Returns a null limit.
        /// </summary>
        public static Limit None => null;

        /// <summary>
        /// Prevents a default instance of the <see cref="Limit"/> class from being created.
        /// </summary>
        private Limit() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Limit"/> class.
        /// </summary>
        /// <param name="skip">A number of elements to bypass in a sequence.</param>
        /// <param name="take">A number of contiguous elements.</param>
        public Limit(int skip, int take)
        {
            Skip = skip;
            Take = take;
        }

        /// <summary>
        /// Checks if the specified limit is a null limit.
        /// </summary>
        /// <param name="limit">The limit to check.</param>
        /// <returns><c>True</c> if the specified limit is a null limit. <c>False</c> otherwise.
        /// </returns>
        public static bool HasNone(Limit limit)
        {
            return limit == Limit.None;
        }

        public override string ToString()
        {
            return $"{{Skip: {Skip}, Take: {Take}}}";
        }
    }
}