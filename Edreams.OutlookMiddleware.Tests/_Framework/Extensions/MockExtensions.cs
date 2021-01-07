using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Edreams.OutlookMiddleware.DataAccess.Repositories.Interfaces;
using Edreams.OutlookMiddleware.Model.Base;
using Moq;
using Moq.Language.Flow;

// ReSharper disable once CheckNamespace
namespace Edreams.OutlookMiddleware.Tests.Framework.Extensions
{
    public static class MockExtensions
    {
        /// <summary>
        /// Automatically setup a mocked IRepository's Find method.
        /// </summary>
        /// <typeparam name="T">The type of the entity used by the IRepository.</typeparam>
        /// <param name="mock">The mocked IRepository.</param>
        /// <param name="data">The data that will be used as a data-source for the mocked repository.</param>
        /// <returns>An object that can be used by the Moq Fluent API.</returns>
        public static IReturnsResult<IRepository<T>> SetupRepositoryFind<T>(this Mock<IRepository<T>> mock, IList<T> data) where T : ModelBase
        {
            // The find method takes a predicate (lambda expression) that should be compiled
            // and executed on the data that acts as a data-source for the mocked repository.
            return mock.Setup(x => x.Find(It.IsAny<Expression<Func<T, bool>>>()))
                .ReturnsAsync((Expression<Func<T, bool>> predicate) => data.Where(predicate.Compile()).ToList());
        }

        /// <summary>
        /// Automatically verify a mocked IRepository's Update method.
        /// </summary>
        /// <typeparam name="T">The type of the entity used by the IRepository.</typeparam>
        /// <param name="mock">The mocked IRepository.</param>
        /// <param name="times">The number of times the "Update" method is expected to be called.</param>
        public static void VerifyRepositoryUpdate<T>(this Mock<IRepository<T>> mock, Times times) where T : ModelBase
        {
            mock.Verify(x => x.Update(It.IsAny<IList<T>>()), times);
        }
    }
}