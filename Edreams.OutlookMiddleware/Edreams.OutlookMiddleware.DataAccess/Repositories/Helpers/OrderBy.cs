using System;
using System.Linq.Expressions;

namespace Edreams.OutlookMiddleware.DataAccess.Repositories.Helpers
{
    public class OrderBy<TModel, TKey>
    {
        public Expression<Func<TModel, TKey>> By { get; set; }

        public override string ToString()
        {
            return $"{{By: {By}}}";
        }
    }

    public class OrderBy<TModel, TKey1, TKey2>
    {
        public Expression<Func<TModel, TKey1>> By1 { get; set; }
        public Expression<Func<TModel, TKey2>> By2 { get; set; }

        public override string ToString()
        {
            return $"{{By1: {By1}, By2: {By2}}}";
        }
    }
}