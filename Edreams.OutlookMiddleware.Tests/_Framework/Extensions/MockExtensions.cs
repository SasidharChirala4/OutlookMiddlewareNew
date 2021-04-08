using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Edreams.OutlookMiddleware.DataAccess.Repositories.Helpers;
using Edreams.OutlookMiddleware.DataAccess.Repositories.Interfaces;
using Edreams.OutlookMiddleware.Model.Base;
using Microsoft.EntityFrameworkCore;
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
        /// Automatically setup a mocked IRepository's Find method with include expression
        /// </summary>
        /// <typeparam name="T">The type of the entity used by the IRepository.</typeparam>
        /// <typeparam name="TProperty">The type of the property included in resultset</typeparam>
        /// <param name="mock">The mocked IRepository.</param>
        /// <param name="data">The data that will be used as a data-source for the mocked repository.</param>
        /// <param name="includes">The data that will be included in result set</param>
        /// <returns>An object that can be used by the Moq Fluent API.</returns>
        public static IReturnsResult<IRepository<T>> SetupRepositoryFind<T, TProperty>(this Mock<IRepository<T>> mock, IList<T> data, Expression<Func<T, TProperty>> includes) where T : ModelBase
        {
            // The find method takes a predicate (lambda expression), includes (lambda expression) that should be compiled
            // and executed on the data that acts as a data-source for the mocked repository.
            return mock.Setup(x => x.Find(It.IsAny<Expression<Func<T, bool>>>(), It.IsAny<Expression<Func<T, TProperty>>>()))
           .ReturnsAsync((Expression<Func<T, bool>> predicate, Expression<Func<T, TProperty>> includes) => data.AsQueryable().Include(includes).Where(predicate.Compile()).ToList());
        }

        /// <summary>
        /// Automatically setup a mocked IRepository's FindDescending method.
        /// </summary>
        /// <typeparam name="T">The type of the entity used by the IRepository.</typeparam>
        /// <typeparam name="TOrderKey">The type of the property used by the orderby (lambda expression).</typeparam>
        /// <param name="mock">The mocked IRepository.</param>
        /// <param name="data">The data that will be used as a data-source for the mocked repository.</param>
        /// <returns>An object that can be used by the Moq Fluent API.</returns>
        public static IReturnsResult<IRepository<T>> SetupRepositoryFindDescending<T, TOrderKey>(this Mock<IRepository<T>> mock, IList<T> data) where T : ModelBase =>
            // The FindDescending method takes  predicate (lambda expression),orderby caluse(lambda expression) ,limit that should be compiled
            // and executed on the data that acts as a data-source for the mocked repository.
            mock.Setup(x => x.FindDescending(It.IsAny<Expression<Func<T, bool>>>(), It.IsAny<Expression<Func<T, TOrderKey>>>(), It.IsAny<Limit>()))
                .ReturnsAsync((Expression<Func<T, bool>> predicate, Expression<Func<T, TOrderKey>> orderBy, Limit limit) => data.Where(predicate.Compile()).OrderByDescending(orderBy.Compile())
                        .Skip(limit.Skip).Take(limit.Take).ToList());

        /// <summary>
        /// Automatically setup a mocked IRepository's GetFirstAscending method.
        /// </summary>
        /// <typeparam name="T">The type of the entity used by the IRepository.</typeparam>
        /// <typeparam name="TOrderKey">The type of the property used in the orderby caluse(lambda expression).</typeparam>
        /// <param name="mock">The mocked IRepository.</param>
        /// <param name="data">The data that will be used as a data-source for the mocked repository.</param>
        /// <returns>An object that can be used by the Moq Fluent API.</returns>
        public static IReturnsResult<IRepository<T>> SetupRepositoryFindAscending<T, TOrderKey>(this Mock<IRepository<T>> mock, IList<T> data) where T : ModelBase =>
            // The GetFirstAscending method takes a predicate (lambda expression) ,orderBy (lambda expression) that should be compiled
            // and executed on the data that acts as a data-source for the mocked repository.
            mock.Setup(x => x.GetFirstAscending(It.IsAny<Expression<Func<T, bool>>>(), It.IsAny<Expression<Func<T, TOrderKey>>>()))
                .ReturnsAsync((Expression<Func<T, bool>> predicate, Expression<Func<T, TOrderKey>> orderBy) => data.OrderBy(orderBy.Compile()).FirstOrDefault(predicate.Compile()));

        /// <summary>
        /// Automatically setup a mocked IRepository's FindAndProject method.
        /// </summary>
        /// <typeparam name="T">The type of the entity used by the IRepository.</typeparam>
        /// <param name="mock">The mocked IRepository.</param>
        /// <param name="data">The data that will be used as a data-source for the mocked repository.</param>
        /// <returns>An object that can be used by the Moq Fluent API.</returns>
        public static IReturnsResult<IRepository<T>> SetupRepositoryFindAndProject<T, TResult>(this Mock<IRepository<T>> mock, IList<T> data) where T : ModelBase
        {

            // The FindAndProject method takes a predicate (lambda expression), projection (lambda expression) that should be compiled
            // and executed on the data that acts as a data-source for the mocked repository.
            return mock.Setup(x => x.FindAndProject(It.IsAny<Expression<Func<T, bool>>>(), It.IsAny<Expression<Func<T, TResult>>>()))
                  .ReturnsAsync((Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> projection) => data.Where(predicate.Compile()).Select(projection.Compile()).ToList());
        }

        /// <summary>
        /// Automatically setup a mocked IRepository's GetSingle method.
        /// </summary>
        /// <typeparam name="T">The type of the entity used by the IRepository.</typeparam>
        /// <param name="mock">The mocked IRepository.</param>
        /// <param name="data">The data that will be used as a data-source for the mocked repository.</param>
        /// <returns>An object that can be used by the Moq Fluent API.</returns>
        public static IReturnsResult<IRepository<T>> SetupRepositoryGetSingle<T>(this Mock<IRepository<T>> mock, IList<T> data) where T : ModelBase
        {
            // The GetSingle method takes a predicate (lambda expression) that should be compiled
            // and executed on the data that acts as a data-source for the mocked repository.
            return mock.Setup(x => x.GetSingle(It.IsAny<Expression<Func<T, bool>>>()))
                .ReturnsAsync((Expression<Func<T, bool>> predicate) => data.SingleOrDefault(predicate.Compile()));
        }

        /// <summary>
        /// Automatically setup a mocked IRepository's GetSingleAndProject method.
        /// </summary>
        /// <typeparam name="T">The type of the entity used by the IRepository.</typeparam>
        /// <param name="mock">The mocked IRepository.</param>
        /// <param name="data">The data that will be used as a data-source for the mocked repository.</param>
        /// <returns>An object that can be used by the Moq Fluent API.</returns>
        public static IReturnsResult<IRepository<T>> SetupRepositoryGetSingleAndProject<T, TResult>(this Mock<IRepository<T>> mock, IList<T> data) where T : ModelBase
        {
            // The GetSingleAndProject method takes a predicate (lambda expression),projetion (lambda expression) that should be compiled
            // and executed on the data that acts as a data-source for the mocked repository.
            return mock.Setup(x => x.GetSingleAndProject(It.IsAny<Expression<Func<T, bool>>>(), It.IsAny<Expression<Func<T, TResult>>>()))
                .ReturnsAsync((Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> projection) => data.Where(predicate.Compile()).Select(projection.Compile()).SingleOrDefault());
        }

        /// <summary>
        /// Automatically setup a mocked IRepository's Exists method.
        /// </summary>
        /// <typeparam name="T">The type of the entity used by the IRepository.</typeparam>
        /// <param name="mock">The mocked IRepository.</param>
        /// <param name="data">The data that will be used as a data-source for the mocked repository.</param>
        /// <returns>An object that can be used by the Moq Fluent API.</returns>
        public static IReturnsResult<IRepository<T>> SetupRepositoryExists<T>(this Mock<IRepository<T>> mock, IList<T> data) where T : ModelBase
        {
            // The find Exists takes a predicate (lambda expression) that should be compiled
            // and executed on the data that acts as a data-source for the mocked repository.
            return mock.Setup(x => x.Exists(It.IsAny<Expression<Func<T, bool>>>()))
                .ReturnsAsync((Expression<Func<T, bool>> predicate) => data.Any(predicate.Compile()));
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

        /// <summary>
        /// Automatically verify a mocked IRepository's Create method for single entity.
        /// </summary>
        /// <typeparam name="T">The type of the entity used by the IRepository.</typeparam>
        /// <param name="mock">The mocked IRepository.</param>
        /// <param name="times">The number of times the "Create" method is expected to be called.</param>
        public static void VerifyRepositoryCreateSingle<T>(this Mock<IRepository<T>> mock, Times times) where T : ModelBase
        {
            mock.Verify(x => x.Create(It.IsAny<T>()), times);
        }

        /// <summary>
        /// Automatically verify a mocked IRepository's Update method for single entity.
        /// </summary>
        /// <typeparam name="T">The type of the entity used by the IRepository.</typeparam>
        /// <param name="mock">The mocked IRepository.</param>
        /// <param name="times">The number of times the "Update" method is expected to be called.</param>
        public static void VerifyRepositoryUpdateSingle<T>(this Mock<IRepository<T>> mock, Times times) where T : ModelBase
        {
            mock.Verify(x => x.Update(It.IsAny<T>()), times);
        }

        /// <summary>
        /// Automatically setup a mocked IRepository's GetFirstDescending method.
        /// </summary>
        /// <typeparam name="T">The type of the entity used by the IRepository.</typeparam>
        /// <typeparam name="TOrderKey">The type of the property used by the order by clause</typeparam>
        /// <param name="mock">The mocked IRepository.</param>
        /// <param name="data">The data that will be used as a data-source for the mocked repository.</param>
        /// <returns>An object that can be used by the Moq Fluent API.</returns>
        public static IReturnsResult<IRepository<T>> SetupRepositoryGetFirstDescending<T, TOrderKey>(this Mock<IRepository<T>> mock, IList<T> data) where T : ModelBase =>
            // The find method takes a predicate (lambda expression) , orderBy clause(lambda expression) that should be compiled
            // and executed on the data that acts as a data-source for the mocked repository.
            mock.Setup(x => x.GetFirstDescending(It.IsAny<Expression<Func<T, bool>>>(), It.IsAny<Expression<Func<T, TOrderKey>>>()))
                .ReturnsAsync((Expression<Func<T, bool>> predicate, Expression<Func<T, TOrderKey>> orderBy) => data.OrderByDescending(orderBy.Compile()).FirstOrDefault(predicate.Compile()));

        /// <summary>
        /// Automatically setup a mocked IRepository's GetFirstAscending method.
        /// </summary>
        /// <typeparam name="T">The type of the entity used by the IRepository.</typeparam>
        /// <typeparam name="TOrderKey">The type of the property used by the order by clause</typeparam>
        /// <param name="mock">The mocked IRepository.</param>
        /// <param name="data">The data that will be used as a data-source for the mocked repository.</param>
        /// <returns>An object that can be used by the Moq Fluent API.</returns>
        public static IReturnsResult<IRepository<T>> SetupRepositoryGetFirstAscending<T, TOrderKey>(this Mock<IRepository<T>> mock, IList<T> data) where T : ModelBase =>
            // The GetFirstAscending method takes a predicate (lambda expression), orderBy (lambda expression) that should be compiled
            // and executed on the data that acts as a data-source for the mocked repository.
            mock.Setup(x => x.GetFirstAscending(It.IsAny<Expression<Func<T, bool>>>(), It.IsAny<Expression<Func<T, TOrderKey>>>()))
                .ReturnsAsync((Expression<Func<T, bool>> predicate, Expression<Func<T, TOrderKey>> orderBy) =>
                data.OrderBy(orderBy.Compile()).FirstOrDefault(predicate.Compile()));


    }
}