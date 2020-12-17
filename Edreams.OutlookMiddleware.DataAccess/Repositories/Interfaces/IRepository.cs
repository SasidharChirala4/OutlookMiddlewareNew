using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Edreams.OutlookMiddleware.DataAccess.Repositories.Helpers;
using Edreams.OutlookMiddleware.Model.Base;

namespace Edreams.OutlookMiddleware.DataAccess.Repositories.Interfaces
{
    /// <summary>
     /// Interface describing a generic repository for CRUD operations.
     /// </summary>
     /// <typeparam name="TModel">The entity type this generic repository is used for.</typeparam>
    public interface IRepository<TModel> : IDisposable where TModel : ModelBase
    {
        #region <| Exists |>

        /// <summary>
        /// Checks the existence of objects using this repository.
        /// </summary>
        /// <returns>True if objects are found, false otherwise.</returns>
        Task<bool> Exists();

        /// <summary>
        /// Checks the existence of objects using this repository.
        /// </summary>
        /// <param name="predicate">The predicate used to find objects.</param>
        /// <returns>True if objects are found, false otherwise.</returns>
        Task<bool> Exists(
            Expression<Func<TModel, bool>> predicate);

        #endregion

        #region <| Count |>

        /// <summary>
        /// Counts the objects using this repository.
        /// </summary>
        /// <returns>The number of objects.</returns>
        Task<int> Count();

        /// <summary>
        /// Counts the objects using this repository.
        /// </summary>
        /// <param name="predicate">The predicate used to find objects.</param>
        /// <returns>The number of objects.</returns>
        Task<int> Count(
            Expression<Func<TModel, bool>> predicate);

        #endregion

        #region <| GetAll |>

        /// <summary>
        /// Gets all objects from a data-source.
        /// </summary>
        /// <returns>A list of objects from a data-source.</returns>
        Task<IList<TModel>> GetAll();

        /// <summary>
        /// Gets all objects from a data-source.
        /// </summary>
        /// <typeparam name="TProperty">The type of the navigation property.</typeparam>
        /// <param name="include">The navigation property to include using a SQL JOIN.</param>
        /// <returns>A list of objects from a data-source.</returns>
        Task<IList<TModel>> GetAll<TProperty>(
            Expression<Func<TModel, TProperty>> include);

        /// <summary>
        /// Gets all objects from a data-source.
        /// </summary>
        /// <typeparam name="TProperty1">The first type of the navigation property.</typeparam>
        /// <typeparam name="TProperty2">The second type of the navigation property.</typeparam>
        /// <param name="include1">The first navigation property to include using a SQL JOIN.</param>
        /// <param name="include2">The second navigation property to include using a SQL JOIN.</param>
        /// <returns>A list of objects from a data-source.</returns>
        Task<IList<TModel>> GetAll<TProperty1, TProperty2>(
            Expression<Func<TModel, TProperty1>> include1,
            Expression<Func<TModel, TProperty2>> include2);

        /// <summary>
        /// Gets all objects from a data-source.
        /// </summary>
        /// <typeparam name="TProperty1">The first type of the navigation property.</typeparam>
        /// <typeparam name="TProperty2">The second type of the navigation property.</typeparam>
        /// <typeparam name="TProperty3">The second type of the navigation property.</typeparam>
        /// <param name="include1">The first navigation property to include using a SQL JOIN.</param>
        /// <param name="include2">The second navigation property to include using a SQL JOIN.</param>
        /// <param name="include3">The third navigation property to include using a SQL JOIN.</param>
        /// <returns>A list of objects from a data-source.</returns>
        Task<IList<TModel>> GetAll<TProperty1, TProperty2, TProperty3>(
            Expression<Func<TModel, TProperty1>> include1,
            Expression<Func<TModel, TProperty2>> include2,
            Expression<Func<TModel, TProperty3>> include3);

        /// <summary>
        /// Gets all objects from a data-source and project.
        /// </summary>
        /// <typeparam name="TResult">The type of the projected result.</typeparam>
        /// <param name="projection">The projection to apply.</param>
        /// <returns>A list of projected objects from a data-source.</returns>
        Task<IList<TResult>> GetAllAndProject<TResult>(
            Expression<Func<TModel, TResult>> projection);

        /// <summary>
        /// Gets all objects from a data-source and project.
        /// </summary>
        /// <typeparam name="TProperty">The type of the navigation property.</typeparam>
        /// <typeparam name="TResult">The type of the projected result.</typeparam>
        /// <param name="include">The navigation property to include using a SQL JOIN.</param>
        /// <param name="projection">The projection to apply.</param>
        /// <returns>A list of projected objects from a data-source.</returns>
        Task<IList<TResult>> GetAllAndProject<TProperty, TResult>(
            Expression<Func<TModel, TProperty>> include,
            Expression<Func<TModel, TResult>> projection);

        /// <summary>
        /// Gets all objects from a data-source and project.
        /// </summary>
        /// <typeparam name="TProperty1">The first type of the navigation property.</typeparam>
        /// <typeparam name="TProperty2">The second type of the navigation property.</typeparam>
        /// <typeparam name="TResult">The type of the projected result.</typeparam>
        /// <param name="include1">The first navigation property to include using a SQL JOIN.</param>
        /// <param name="include2">The second navigation property to include using a SQL JOIN.</param>
        /// <param name="projection">The projection to apply.</param>
        /// <returns>A list of projected objects from a data-source.</returns>
        Task<IList<TResult>> GetAllAndProject<TProperty1, TProperty2, TResult>(
            Expression<Func<TModel, TProperty1>> include1,
            Expression<Func<TModel, TProperty2>> include2,
            Expression<Func<TModel, TResult>> projection);

        /// <summary>
        /// Gets all objects from a data-source and project.
        /// </summary>
        /// <typeparam name="TProperty1">The first type of the navigation property.</typeparam>
        /// <typeparam name="TProperty2">The second type of the navigation property.</typeparam>
        /// <typeparam name="TProperty3">The second type of the navigation property.</typeparam>
        /// <typeparam name="TResult">The type of the projected result.</typeparam>
        /// <param name="include1">The first navigation property to include using a SQL JOIN.</param>
        /// <param name="include2">The second navigation property to include using a SQL JOIN.</param>
        /// <param name="include3">The third navigation property to include using a SQL JOIN.</param>
        /// <param name="projection">The projection to apply.</param>
        /// <returns>A list of projected objects from a data-source.</returns>
        Task<IList<TResult>> GetAllAndProject<TProperty1, TProperty2, TProperty3, TResult>(
            Expression<Func<TModel, TProperty1>> include1,
            Expression<Func<TModel, TProperty2>> include2,
            Expression<Func<TModel, TProperty3>> include3,
            Expression<Func<TModel, TResult>> projection);

        #endregion

        #region <| GetSingle |>

        /// <summary>
        /// Gets a single object from a data-source.
        /// </summary>
        /// <returns>A single object from a data-source.</returns>
        Task<TModel> GetSingle();

        /// <summary>
        /// Gets a single object from a data-source.
        /// </summary>
        /// <typeparam name="TProperty">The type of the navigation property.</typeparam>
        /// <param name="include">The navigation property to include using a SQL JOIN.</param>
        /// <returns>A single object from a data-source.</returns>
        Task<TModel> GetSingle<TProperty>(
            Expression<Func<TModel, TProperty>> include);

        /// <summary>
        /// Gets a single object from a data-source.
        /// </summary>
        /// <typeparam name="TProperty1">The first type of the navigation property.</typeparam>
        /// <typeparam name="TProperty2">The second type of the navigation property.</typeparam>
        /// <param name="include1">The first navigation property to include using a SQL JOIN.</param>
        /// <param name="include2">The second navigation property to include using a SQL JOIN.</param>
        /// <returns>A single object from a data-source.</returns>
        Task<TModel> GetSingle<TProperty1, TProperty2>(
            Expression<Func<TModel, TProperty1>> include1,
            Expression<Func<TModel, TProperty2>> include2);

        /// <summary>
        /// Gets a single object from a data-source.
        /// </summary>
        /// <typeparam name="TProperty1">The first type of the navigation property.</typeparam>
        /// <typeparam name="TProperty2">The second type of the navigation property.</typeparam>
        /// <typeparam name="TProperty3">The third type of the navigation property.</typeparam>
        /// <param name="include1">The first navigation property to include using a SQL JOIN.</param>
        /// <param name="include2">The second navigation property to include using a SQL JOIN.</param>
        /// <param name="include3">The third navigation property to include using a SQL JOIN.</param>
        /// <returns>A single object from a data-source.</returns>
        Task<TModel> GetSingle<TProperty1, TProperty2, TProperty3>(
            Expression<Func<TModel, TProperty1>> include1,
            Expression<Func<TModel, TProperty2>> include2,
            Expression<Func<TModel, TProperty3>> include3);

        /// <summary>
        /// Gets a single object from a data-source.
        /// </summary>
        /// <param name="predicate">The predicate used to find a single object.</param>
        /// <returns>A single object from a data-source.</returns>
        Task<TModel> GetSingle(
            Expression<Func<TModel, bool>> predicate);

        /// <summary>
        /// Gets a single object from a data-source.
        /// </summary>
        /// <typeparam name="TProperty">The type of the navigation property.</typeparam>
        /// <param name="predicate">The predicate used to find a single object.</param>
        /// <param name="include">The navigation property to include using a SQL JOIN.</param>
        /// <returns>A single object from a data-source.</returns>
        Task<TModel> GetSingle<TProperty>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TProperty>> include);

        /// <summary>
        /// Gets a single object from a data-source.
        /// </summary>
        /// <typeparam name="TProperty1">The first type of the navigation property.</typeparam>
        /// <typeparam name="TProperty2">The second type of the navigation property.</typeparam>
        /// <param name="predicate">The predicate used to find a single object.</param>
        /// <param name="include1">The first navigation property to include using a SQL JOIN.</param>
        /// <param name="include2">The second navigation property to include using a SQL JOIN.</param>
        /// <returns>A single object from a data-source.</returns>
        Task<TModel> GetSingle<TProperty1, TProperty2>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TProperty1>> include1,
            Expression<Func<TModel, TProperty2>> include2);

        /// <summary>
        /// Gets a single object from a data-source.
        /// </summary>
        /// <typeparam name="TProperty1">The first type of the navigation property.</typeparam>
        /// <typeparam name="TProperty2">The second type of the navigation property.</typeparam>
        /// <typeparam name="TProperty3">The third type of the navigation property.</typeparam>
        /// <param name="predicate">The predicate used to find a single object.</param>
        /// <param name="include1">The first navigation property to include using a SQL JOIN.</param>
        /// <param name="include2">The second navigation property to include using a SQL JOIN.</param>
        /// <param name="include3">The third navigation property to include using a SQL JOIN.</param>
        /// <returns>A single object from a data-source.</returns>
        Task<TModel> GetSingle<TProperty1, TProperty2, TProperty3>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TProperty1>> include1,
            Expression<Func<TModel, TProperty2>> include2,
            Expression<Func<TModel, TProperty3>> include3);

        /// <summary>
        /// Gets a single object from a data-source and project.
        /// </summary>
        /// <typeparam name="TResult">The type of the projected result.</typeparam>
        /// <param name="projection">The projection to apply.</param>
        /// <returns>A single projected object from a data-source.</returns>
        Task<TResult> GetSingleAndProject<TResult>(
            Expression<Func<TModel, TResult>> projection);

        /// <summary>
        /// Gets a single object from a data-source and project.
        /// </summary>
        /// <typeparam name="TProperty">The type of the navigation property.</typeparam>
        /// <typeparam name="TResult">The type of the projected result.</typeparam>
        /// <param name="include">The navigation property to include using a SQL JOIN.</param>
        /// <param name="projection">The projection to apply.</param>
        /// <returns>A single projected object from a data-source.</returns>
        Task<TResult> GetSingleAndProject<TProperty, TResult>(
            Expression<Func<TModel, TProperty>> include,
            Expression<Func<TModel, TResult>> projection);

        /// <summary>
        /// Gets a single object from a data-source and project.
        /// </summary>
        /// <typeparam name="TProperty1">The first type of the navigation property.</typeparam>
        /// <typeparam name="TProperty2">The second type of the navigation property.</typeparam>
        /// <typeparam name="TResult">The type of the projected result.</typeparam>
        /// <param name="include1">The first navigation property to include using a SQL JOIN.</param>
        /// <param name="include2">The second navigation property to include using a SQL JOIN.</param>
        /// <param name="projection">The projection to apply.</param>
        /// <returns>A single projected object from a data-source.</returns>
        Task<TResult> GetSingleAndProject<TProperty1, TProperty2, TResult>(
            Expression<Func<TModel, TProperty1>> include1,
            Expression<Func<TModel, TProperty2>> include2,
            Expression<Func<TModel, TResult>> projection);

        /// <summary>
        /// Gets a single object from a data-source and project.
        /// </summary>
        /// <typeparam name="TProperty1">The first type of the navigation property.</typeparam>
        /// <typeparam name="TProperty2">The second type of the navigation property.</typeparam>
        /// <typeparam name="TProperty3">The third type of the navigation property.</typeparam>
        /// <typeparam name="TResult">The type of the projected result.</typeparam>
        /// <param name="include1">The first navigation property to include using a SQL JOIN.</param>
        /// <param name="include2">The second navigation property to include using a SQL JOIN.</param>
        /// <param name="include3">The third navigation property to include using a SQL JOIN.</param>
        /// <param name="projection">The projection to apply.</param>
        /// <returns>A single projected object from a data-source.</returns>
        Task<TResult> GetSingleAndProject<TProperty1, TProperty2, TProperty3, TResult>(
            Expression<Func<TModel, TProperty1>> include1,
            Expression<Func<TModel, TProperty2>> include2,
            Expression<Func<TModel, TProperty3>> include3,
            Expression<Func<TModel, TResult>> projection);

        /// <summary>
        /// Gets a single object from a data-source and project.
        /// </summary>
        /// <typeparam name="TResult">The type of the projected result.</typeparam>
        /// <param name="predicate">The predicate used to find a single object.</param>
        /// <param name="projection">The projection to apply.</param>
        /// <returns>A single projected object from a data-source.</returns>
        Task<TResult> GetSingleAndProject<TResult>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TResult>> projection);

        /// <summary>
        /// Gets a single object from a data-source and project.
        /// </summary>
        /// <typeparam name="TProperty">The type of the navigation property.</typeparam>
        /// <typeparam name="TResult">The type of the projected result.</typeparam>
        /// <param name="predicate">The predicate used to find a single object.</param>
        /// <param name="include">The navigation property to include using a SQL JOIN.</param>
        /// <param name="projection">The projection to apply.</param>
        /// <returns>A single projected object from a data-source.</returns>
        Task<TResult> GetSingleAndProject<TProperty, TResult>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TProperty>> include,
            Expression<Func<TModel, TResult>> projection);

        /// <summary>
        /// Gets a single object from a data-source and project.
        /// </summary>
        /// <typeparam name="TProperty1">The first type of the navigation property.</typeparam>
        /// <typeparam name="TProperty2">The second type of the navigation property.</typeparam>
        /// <typeparam name="TResult">The type of the projected result.</typeparam>
        /// <param name="predicate">The predicate used to find a single object.</param>
        /// <param name="include1">The first navigation property to include using a SQL JOIN.</param>
        /// <param name="include2">The second navigation property to include using a SQL JOIN.</param>
        /// <param name="projection">The projection to apply.</param>
        /// <returns>A single projected object from a data-source.</returns>
        Task<TResult> GetSingleAndProject<TProperty1, TProperty2, TResult>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TProperty1>> include1,
            Expression<Func<TModel, TProperty2>> include2,
            Expression<Func<TModel, TResult>> projection);

        /// <summary>
        /// Gets a single object from a data-source and project.
        /// </summary>
        /// <typeparam name="TProperty1">The first type of the navigation property.</typeparam>
        /// <typeparam name="TProperty2">The second type of the navigation property.</typeparam>
        /// <typeparam name="TProperty3">The third type of the navigation property.</typeparam>
        /// <typeparam name="TResult">The type of the projected result.</typeparam>
        /// <param name="predicate">The predicate used to find a single object.</param>
        /// <param name="include1">The first navigation property to include using a SQL JOIN.</param>
        /// <param name="include2">The second navigation property to include using a SQL JOIN.</param>
        /// <param name="include3">The third navigation property to include using a SQL JOIN.</param>
        /// <param name="projection">The projection to apply.</param>
        /// <returns>A single projected object from a data-source.</returns>
        Task<TResult> GetSingleAndProject<TProperty1, TProperty2, TProperty3, TResult>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TProperty1>> include1,
            Expression<Func<TModel, TProperty2>> include2,
            Expression<Func<TModel, TProperty3>> include3,
            Expression<Func<TModel, TResult>> projection);

        #endregion

        #region <| GetFirst |>

        /// <summary>
        /// Gets the first object of a list of ordered object from a data-source.
        /// </summary>
        /// <param name="predicate">The predicate used to find objects.</param>
        /// <returns>
        /// The first object of a list of ordered objects from a data-source.
        /// </returns>
        Task<TModel> GetFirst(
            Expression<Func<TModel, bool>> predicate);

        /// <summary>
        /// Gets the first object of a list of ordered object from a data-source.
        /// </summary>
        /// <typeparam name="TProperty">The type of the navigation property.</typeparam>
        /// <param name="predicate">The predicate used to find objects.</param>
        /// <param name="include">The navigation property to include using a SQL JOIN.</param>
        /// <returns>
        /// The first object of a list of ordered objects from a data-source.
        /// </returns>
        Task<TModel> GetFirst<TProperty>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TProperty>> include);

        /// <summary>
        /// Gets the first object of a list of ordered object from a data-source.
        /// </summary>
        /// <typeparam name="TProperty1">The first type of the navigation property.</typeparam>
        /// <typeparam name="TProperty2">The second type of the navigation property.</typeparam>
        /// <param name="predicate">The predicate used to find objects.</param>
        /// <param name="include1">The first navigation property to include using a SQL JOIN.</param>
        /// <param name="include2">The second navigation property to include using a SQL JOIN.</param>
        /// <returns>
        /// The first object of a list of ordered objects from a data-source.
        /// </returns>
        Task<TModel> GetFirst<TProperty1, TProperty2>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TProperty1>> include1,
            Expression<Func<TModel, TProperty2>> include2);

        /// <summary>
        /// Gets the first object of a list of ordered object from a data-source.
        /// </summary>
        /// <typeparam name="TProperty1">The first type of the navigation property.</typeparam>
        /// <typeparam name="TProperty2">The second type of the navigation property.</typeparam>
        /// <typeparam name="TProperty3">The third type of the navigation property.</typeparam>
        /// <param name="predicate">The predicate used to find objects.</param>
        /// <param name="include1">The first navigation property to include using a SQL JOIN.</param>
        /// <param name="include2">The second navigation property to include using a SQL JOIN.</param>
        /// <param name="include3">The third navigation property to include using a SQL JOIN.</param>
        /// <returns>
        /// The first object of a list of ordered objects from a data-source.
        /// </returns>
        Task<TModel> GetFirst<TProperty1, TProperty2, TProperty3>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TProperty1>> include1,
            Expression<Func<TModel, TProperty2>> include2,
            Expression<Func<TModel, TProperty3>> include3);

        #endregion

        #region <| GetFirstAscending |>

        /// <summary>
        /// Gets the first object of a list of ordered object from a data-source.
        /// </summary>
        /// <typeparam name="TKey">The type of the order property.</typeparam>
        /// <param name="predicate">The predicate used to find objects.</param>
        /// <param name="orderBy">The order by.</param>
        /// <returns>
        /// The first object of a list of ordered objects from a data-source.
        /// </returns>
        Task<TModel> GetFirstAscending<TKey>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TKey>> orderBy);

        /// <summary>
        /// Gets the first object of a list of ordered object from a data-source.
        /// </summary>
        /// <param name="predicate">The predicate used to find objects.</param>
        /// <param name="orderBy">The order by information.</param>
        /// <returns>
        /// The first object of a list of ordered objects from a data-source.
        /// </returns>
        Task<TModel> GetFirstAscending<TKey1, TKey2>(
            Expression<Func<TModel, bool>> predicate,
            OrderBy<TModel, TKey1, TKey2> orderBy);

        /// <summary>
        /// Gets the first object of a list of ordered object from a data-source.
        /// </summary>
        /// <typeparam name="TKey">The type of the order property.</typeparam>
        /// <typeparam name="TProperty">The type of the navigation property.</typeparam>
        /// <param name="predicate">The predicate used to find objects.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="include">The navigation property to include using a SQL JOIN.</param>
        /// <returns>
        /// The first object of a list of ordered objects from a data-source.
        /// </returns>
        Task<TModel> GetFirstAscending<TKey, TProperty>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TKey>> orderBy,
            Expression<Func<TModel, TProperty>> include);

        /// <summary>
        /// Gets the first object of a list of ordered object from a data-source.
        /// </summary>
        /// <typeparam name="TKey">The type of the order property.</typeparam>
        /// <typeparam name="TProperty1">The first type of the navigation property.</typeparam>
        /// <typeparam name="TProperty2">The second type of the navigation property.</typeparam>
        /// <param name="predicate">The predicate used to find objects.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="include1">The first navigation property to include using a SQL JOIN.</param>
        /// <param name="include2">The second navigation property to include using a SQL JOIN.</param>
        /// <returns>
        /// The first object of a list of ordered objects from a data-source.
        /// </returns>
        Task<TModel> GetFirstAscending<TKey, TProperty1, TProperty2>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TKey>> orderBy,
            Expression<Func<TModel, TProperty1>> include1,
            Expression<Func<TModel, TProperty2>> include2);

        /// <summary>
        /// Gets the first object of a list of ordered object from a data-source.
        /// </summary>
        /// <typeparam name="TKey">The type of the order property.</typeparam>
        /// <typeparam name="TProperty1">The first type of the navigation property.</typeparam>
        /// <typeparam name="TProperty2">The second type of the navigation property.</typeparam>
        /// <typeparam name="TProperty3">The third type of the navigation property.</typeparam>
        /// <param name="predicate">The predicate used to find objects.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="include1">The first navigation property to include using a SQL JOIN.</param>
        /// <param name="include2">The second navigation property to include using a SQL JOIN.</param>
        /// <param name="include3">The third navigation property to include using a SQL JOIN.</param>
        /// <returns>
        /// The first object of a list of ordered objects from a data-source.
        /// </returns>
        Task<TModel> GetFirstAscending<TKey, TProperty1, TProperty2, TProperty3>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TKey>> orderBy,
            Expression<Func<TModel, TProperty1>> include1,
            Expression<Func<TModel, TProperty2>> include2,
            Expression<Func<TModel, TProperty3>> include3);

        #endregion

        #region <| GetFirstDescending |>

        /// <summary>
        /// Gets the first object of a list of descending ordered object from a data-source.
        /// </summary>
        /// <typeparam name="TKey">The type of the order property.</typeparam>
        /// <param name="predicate">The predicate used to find objects.</param>
        /// <param name="orderBy">The order by.</param>
        /// <returns>
        /// The first object of a list of ordered objects from a data-source.
        /// </returns>
        Task<TModel> GetFirstDescending<TKey>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TKey>> orderBy);

        /// <summary>
        /// Gets the first object of a list of descending ordered object from a data-source.
        /// </summary>
        /// <typeparam name="TKey">The type of the order property.</typeparam>
        /// <typeparam name="TProperty">The type of the navigation property.</typeparam>
        /// <param name="predicate">The predicate used to find objects.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="include">The navigation property to include using a SQL JOIN.</param>
        /// <returns>
        /// The first object of a list of ordered objects from a data-source.
        /// </returns>
        Task<TModel> GetFirstDescending<TKey, TProperty>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TKey>> orderBy,
            Expression<Func<TModel, TProperty>> include);

        /// <summary>
        /// Gets the first object of a list of descending ordered object from a data-source.
        /// </summary>
        /// <typeparam name="TKey">The type of the order property.</typeparam>
        /// <typeparam name="TProperty1">The first type of the navigation property.</typeparam>
        /// <typeparam name="TProperty2">The second type of the navigation property.</typeparam>
        /// <param name="predicate">The predicate used to find objects.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="include1">The first navigation property to include using a SQL JOIN.</param>
        /// <param name="include2">The second navigation property to include using a SQL JOIN.</param>
        /// <returns>
        /// The first object of a list of ordered objects from a data-source.
        /// </returns>
        Task<TModel> GetFirstDescending<TKey, TProperty1, TProperty2>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TKey>> orderBy,
            Expression<Func<TModel, TProperty1>> include1,
            Expression<Func<TModel, TProperty2>> include2);

        /// <summary>
        /// Gets the first object of a list of descending ordered object from a data-source.
        /// </summary>
        /// <typeparam name="TKey">The type of the order property.</typeparam>
        /// <typeparam name="TProperty1">The first type of the navigation property.</typeparam>
        /// <typeparam name="TProperty2">The second type of the navigation property.</typeparam>
        /// <typeparam name="TProperty3">The third type of the navigation property.</typeparam>
        /// <param name="predicate">The predicate used to find objects.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="include1">The first navigation property to include using a SQL JOIN.</param>
        /// <param name="include2">The second navigation property to include using a SQL JOIN.</param>
        /// <param name="include3">The third navigation property to include using a SQL JOIN.</param>
        /// <returns>
        /// The first object of a list of ordered objects from a data-source.
        /// </returns>
        Task<TModel> GetFirstDescending<TKey, TProperty1, TProperty2, TProperty3>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TKey>> orderBy,
            Expression<Func<TModel, TProperty1>> include1,
            Expression<Func<TModel, TProperty2>> include2,
            Expression<Func<TModel, TProperty3>> include3);

        #endregion

        #region <| Find |>

        /// <summary>
        /// Finds multiple objects from a data-source.
        /// </summary>
        /// <param name="predicate">The predicate used to find objects.</param>
        /// <returns>A list of objects from a data-source.</returns>
        Task<IList<TModel>> Find(
            Expression<Func<TModel, bool>> predicate);

        /// <summary>
        /// Finds multiple objects from a data-source.
        /// </summary>
        /// <typeparam name="TProperty">The type of the navigation property.</typeparam>
        /// <param name="predicate">The predicate used to find objects.</param>
        /// <param name="include">The navigation property to include using a SQL JOIN.</param>
        /// <returns>A list of objects from a data-source.</returns>
        Task<IList<TModel>> Find<TProperty>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TProperty>> include);

        /// <summary>
        /// Finds multiple objects from a data-source.
        /// </summary>
        /// <typeparam name="TProperty1">The first type of the navigation property.</typeparam>
        /// <typeparam name="TProperty2">The second type of the navigation property.</typeparam>
        /// <param name="predicate">The predicate used to find objects.</param>
        /// <param name="include1">The first navigation property to include using a SQL JOIN.</param>
        /// <param name="include2">The second navigation property to include using a SQL JOIN.</param>
        /// <returns>A list of objects from a data-source.</returns>
        Task<IList<TModel>> Find<TProperty1, TProperty2>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TProperty1>> include1,
            Expression<Func<TModel, TProperty2>> include2);

        /// <summary>
        /// Finds multiple objects from a data-source.
        /// </summary>
        /// <typeparam name="TProperty1">The first type of the navigation property.</typeparam>
        /// <typeparam name="TProperty2">The second type of the navigation property.</typeparam>
        /// <typeparam name="TProperty3">The second type of the navigation property.</typeparam>
        /// <param name="predicate">The predicate used to find objects.</param>
        /// <param name="include1">The first navigation property to include using a SQL JOIN.</param>
        /// <param name="include2">The second navigation property to include using a SQL JOIN.</param>
        /// <param name="include3">The third navigation property to include using a SQL JOIN.</param>
        /// <returns>A list of objects from a data-source.</returns>
        Task<IList<TModel>> Find<TProperty1, TProperty2, TProperty3>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TProperty1>> include1,
            Expression<Func<TModel, TProperty2>> include2,
            Expression<Func<TModel, TProperty3>> include3);

        /// <summary>
        /// Finds multiple objects from a data-source and project.
        /// </summary>
        /// <typeparam name="TResult">The type of the projected result.</typeparam>
        /// <param name="predicate">The predicate used to find objects.</param>
        /// <param name="projection">The projection to apply.</param>
        /// <returns>A list of projected objects from a data-source.</returns>
        Task<IList<TResult>> FindAndProject<TResult>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TResult>> projection);

        /// <summary>
        /// Finds multiple objects from a data-source and project.
        /// </summary>
        /// <typeparam name="TProperty">The type of the navigation property.</typeparam>
        /// <typeparam name="TResult">The type of the projected result.</typeparam>
        /// <param name="predicate">The predicate used to find objects.</param>
        /// <param name="include">The navigation property to include using a SQL JOIN.</param>
        /// <param name="projection">The projection to apply.</param>
        /// <returns>A list of projected objects from a data-source.</returns>
        Task<IList<TResult>> FindAndProject<TProperty, TResult>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TProperty>> include,
            Expression<Func<TModel, TResult>> projection);

        /// <summary>
        /// Finds multiple objects from a data-source and project.
        /// </summary>
        /// <typeparam name="TProperty1">The first type of the navigation property.</typeparam>
        /// <typeparam name="TProperty2">The second type of the navigation property.</typeparam>
        /// <typeparam name="TResult">The type of the projected result.</typeparam>
        /// <param name="predicate">The predicate used to find objects.</param>
        /// <param name="include1">The first navigation property to include using a SQL JOIN.</param>
        /// <param name="include2">The second navigation property to include using a SQL JOIN.</param>
        /// <param name="projection">The projection to apply.</param>
        /// <returns>A list of projected objects from a data-source.</returns>
        Task<IList<TResult>> FindAndProject<TProperty1, TProperty2, TResult>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TProperty1>> include1,
            Expression<Func<TModel, TProperty2>> include2,
            Expression<Func<TModel, TResult>> projection);

        /// <summary>
        /// Finds multiple objects from a data-source and project.
        /// </summary>
        /// <typeparam name="TProperty1">The first type of the navigation property.</typeparam>
        /// <typeparam name="TProperty2">The second type of the navigation property.</typeparam>
        /// <typeparam name="TProperty3">The second type of the navigation property.</typeparam>
        /// <typeparam name="TResult">The type of the projected result.</typeparam>
        /// <param name="predicate">The predicate used to find objects.</param>
        /// <param name="include1">The first navigation property to include using a SQL JOIN.</param>
        /// <param name="include2">The second navigation property to include using a SQL JOIN.</param>
        /// <param name="include3">The third navigation property to include using a SQL JOIN.</param>
        /// <param name="projection">The projection to apply.</param>
        /// <returns>A list of projected objects from a data-source.</returns>
        Task<IList<TResult>> FindAndProject<TProperty1, TProperty2, TProperty3, TResult>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TProperty1>> include1,
            Expression<Func<TModel, TProperty2>> include2,
            Expression<Func<TModel, TProperty3>> include3,
            Expression<Func<TModel, TResult>> projection);

        #endregion

        #region <| FindAscending |>

        /// <summary>
        /// Finds multiple objects from a data-source.
        /// </summary>
        /// <typeparam name="TOrderKey">The type of the order by property.</typeparam>
        /// <param name="predicate">The predicate used to find objects.</param>
        /// <param name="orderBy">The order by.</param>
        /// <returns>A list of objects from a data-source.</returns>
        Task<IList<TModel>> FindAscending<TOrderKey>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TOrderKey>> orderBy);

        /// <summary>
        /// Finds multiple objects from a data-source.
        /// </summary>
        /// <typeparam name="TProperty">The type of the navigation property.</typeparam>
        /// <typeparam name="TOrderKey">The type of the order by property.</typeparam>
        /// <param name="predicate">The predicate used to find objects.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="include">The navigation property to include using a SQL JOIN.</param>
        /// <returns>A list of objects from a data-source.</returns>
        Task<IList<TModel>> FindAscending<TProperty, TOrderKey>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TOrderKey>> orderBy,
            Expression<Func<TModel, TProperty>> include);

        /// <summary>
        /// Finds multiple objects from a data-source.
        /// </summary>
        /// <typeparam name="TProperty1">The first type of the navigation property.</typeparam>
        /// <typeparam name="TProperty2">The second type of the navigation property.</typeparam>
        /// <typeparam name="TOrderKey">The type of the order by property.</typeparam>
        /// <param name="predicate">The predicate used to find objects.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="include1">The first navigation property to include using a SQL JOIN.</param>
        /// <param name="include2">The second navigation property to include using a SQL JOIN.</param>
        /// <returns>A list of objects from a data-source.</returns>
        Task<IList<TModel>> FindAscending<TProperty1, TProperty2, TOrderKey>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TOrderKey>> orderBy,
            Expression<Func<TModel, TProperty1>> include1,
            Expression<Func<TModel, TProperty2>> include2);

        /// <summary>
        /// Finds multiple objects from a data-source.
        /// </summary>
        /// <typeparam name="TProperty1">The first type of the navigation property.</typeparam>
        /// <typeparam name="TProperty2">The second type of the navigation property.</typeparam>
        /// <typeparam name="TProperty3">The second type of the navigation property.</typeparam>
        /// <typeparam name="TOrderKey">The type of the order by property.</typeparam>
        /// <param name="predicate">The predicate used to find objects.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="include1">The first navigation property to include using a SQL JOIN.</param>
        /// <param name="include2">The second navigation property to include using a SQL JOIN.</param>
        /// <param name="include3">The third navigation property to include using a SQL JOIN.</param>
        /// <returns>A list of objects from a data-source.</returns>
        Task<IList<TModel>> FindAscending<TProperty1, TProperty2, TProperty3, TOrderKey>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TOrderKey>> orderBy,
            Expression<Func<TModel, TProperty1>> include1,
            Expression<Func<TModel, TProperty2>> include2,
            Expression<Func<TModel, TProperty3>> include3);

        #endregion

        #region <| FindDescending |>

        /// <summary>
        /// Finds multiple objects from a data-source.
        /// </summary>
        /// <typeparam name="TOrderKey">The type of the order by property.</typeparam>
        /// <param name="predicate">The predicate used to find objects.</param>
        /// <param name="orderBy">The order by.</param>
        /// <returns>A list of objects from a data-source.</returns>
        Task<IList<TModel>> FindDescending<TOrderKey>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TOrderKey>> orderBy);

        /// <summary>
        /// Finds multiple objects from a data-source.
        /// </summary>
        /// <typeparam name="TOrderKey">The type of the order by property.</typeparam>
        /// <param name="predicate">The predicate used to find objects.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="limit">Applies paging to this request.</param>
        /// <returns>A list of objects from a data-source.</returns>
        Task<IList<TModel>> FindDescending<TOrderKey>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TOrderKey>> orderBy, Limit limit);

        /// <summary>
        /// Finds multiple objects from a data-source.
        /// </summary>
        /// <typeparam name="TProperty">The type of the navigation property.</typeparam>
        /// <typeparam name="TOrderKey">The type of the order by property.</typeparam>
        /// <param name="predicate">The predicate used to find objects.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="include">The navigation property to include using a SQL JOIN.</param>
        /// <returns>A list of objects from a data-source.</returns>
        Task<IList<TModel>> FindDescending<TProperty, TOrderKey>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TOrderKey>> orderBy,
            Expression<Func<TModel, TProperty>> include);

        /// <summary>
        /// Finds multiple objects from a data-source.
        /// </summary>
        /// <typeparam name="TProperty1">The first type of the navigation property.</typeparam>
        /// <typeparam name="TProperty2">The second type of the navigation property.</typeparam>
        /// <typeparam name="TOrderKey">The type of the order by property.</typeparam>
        /// <param name="predicate">The predicate used to find objects.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="include1">The first navigation property to include using a SQL JOIN.</param>
        /// <param name="include2">The second navigation property to include using a SQL JOIN.</param>
        /// <returns>A list of objects from a data-source.</returns>
        Task<IList<TModel>> FindDescending<TProperty1, TProperty2, TOrderKey>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TOrderKey>> orderBy,
            Expression<Func<TModel, TProperty1>> include1,
            Expression<Func<TModel, TProperty2>> include2);

        /// <summary>
        /// Finds multiple objects from a data-source.
        /// </summary>
        /// <typeparam name="TProperty1">The first type of the navigation property.</typeparam>
        /// <typeparam name="TProperty2">The second type of the navigation property.</typeparam>
        /// <typeparam name="TProperty3">The second type of the navigation property.</typeparam>
        /// <typeparam name="TOrderKey">The type of the order by property.</typeparam>
        /// <param name="predicate">The predicate used to find objects.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="include1">The first navigation property to include using a SQL JOIN.</param>
        /// <param name="include2">The second navigation property to include using a SQL JOIN.</param>
        /// <param name="include3">The third navigation property to include using a SQL JOIN.</param>
        /// <returns>A list of objects from a data-source.</returns>
        Task<IList<TModel>> FindDescending<TProperty1, TProperty2, TProperty3, TOrderKey>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TOrderKey>> orderBy,
            Expression<Func<TModel, TProperty1>> include1,
            Expression<Func<TModel, TProperty2>> include2,
            Expression<Func<TModel, TProperty3>> include3);

        /// <summary>
        /// Finds multiple objects from a data-source.
        /// </summary>
        /// <typeparam name="TProperty1">The first type of the navigation property.</typeparam>
        /// <typeparam name="TOrderKey">The type of the order by property.</typeparam>
        /// <param name="predicate">The predicate used to find objects.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="include">The first navigation property to include using a SQL JOIN.</param>
        /// <param name="limit">Applies paging to this request.</param>
        /// <returns>A list of objects from a data-source.</returns>
        Task<IList<TModel>> FindDescending<TProperty1, TOrderKey>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TOrderKey>> orderBy,
            Expression<Func<TModel, TProperty1>> include,
            Limit limit);

        /// <summary>
        /// Finds multiple objects from a data-source.
        /// </summary>
        /// <typeparam name="TProperty1">The first type of the navigation property.</typeparam>
        /// <typeparam name="TProperty2">The second type of the navigation property.</typeparam>
        /// <typeparam name="TOrderKey">The type of the order by property.</typeparam>
        /// <param name="predicate">The predicate used to find objects.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="include1">The first navigation property to include using a SQL JOIN.</param>
        /// <param name="include2">The second navigation property to include using a SQL JOIN.</param>
        /// <param name="limit">Applies paging to this request.</param>
        /// <returns>A list of objects from a data-source.</returns>
        Task<IList<TModel>> FindDescending<TProperty1, TProperty2, TOrderKey>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TOrderKey>> orderBy,
            Expression<Func<TModel, TProperty1>> include1,
            Expression<Func<TModel, TProperty2>> include2,
            Limit limit);

        /// <summary>
        /// Finds multiple objects from a data-source.
        /// </summary>
        /// <typeparam name="TProperty1">The first type of the navigation property.</typeparam>
        /// <typeparam name="TProperty2">The second type of the navigation property.</typeparam>
        /// <typeparam name="TProperty3">The third type of the navigation property.</typeparam>
        /// <typeparam name="TOrderKey">The type of the order by property.</typeparam>
        /// <param name="predicate">The predicate used to find objects.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="include1">The first navigation property to include using a SQL JOIN.</param>
        /// <param name="include2">The second navigation property to include using a SQL JOIN.</param>
        /// <param name="include3">The third navigation property to include using a SQL JOIN.</param>
        /// <param name="limit">Applies paging to this request.</param>
        /// <returns>A list of objects from a data-source.</returns>
        Task<IList<TModel>> FindDescending<TProperty1, TProperty2, TProperty3, TOrderKey>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TOrderKey>> orderBy,
            Expression<Func<TModel, TProperty1>> include1,
            Expression<Func<TModel, TProperty2>> include2,
            Expression<Func<TModel, TProperty3>> include3,
            Limit limit);

        #endregion

        #region <| Create |>

        /// <summary>
        /// Creates the specified object in a data-source.
        /// </summary>
        /// <param name="entity">The object to create.</param>
        /// <returns>The created object from a data-source.</returns>
        Task<TModel> Create(TModel entity);

        /// <summary>
        /// Creates the specified object in a data-source. Will not override the InsertedBy and InsertedOn values.
        /// </summary>
        /// <param name="entity">The object to create.</param>
        /// <returns>The created object from a data-source.</returns>
        Task<TModel> CreateDirect(TModel entity);

        /// <summary>
        /// Creates the specified objects in a data-source.
        /// </summary>
        /// <param name="entities">The objects to create.</param>
        /// <returns>The created objects from a data-source.</returns>
        Task<IList<TModel>> Create(IList<TModel> entities);

        /// <summary>
        /// Creates the specified objects in a data-source. Will not override the InsertedBy and InsertedOn values.
        /// </summary>
        /// <param name="entities">The objects to create.</param>
        /// <returns>The created objects from a data-source.</returns>
        Task<IList<TModel>> CreateDirect(IList<TModel> entities);

        /// <summary>
        /// Creates the specified objects in a data-source.
        /// </summary>
        /// <param name="entities">The objects to create..</param>
        /// <returns>The created objects from a data-source.</returns>
        Task<IList<TModel>> BulkCreate(IList<TModel> entities);

        #endregion

        #region <| Update |>

        /// <summary>
        /// Updates the specified object in a data-source.
        /// </summary>
        /// <param name="entity">The object to update.</param>
        /// <returns>The updated object from a data-source.</returns>
        Task<TModel> Update(TModel entity);

        /// <summary>
        /// Updates the specified object in a data-source. Will not override the InsertedBy, InsertedOn, UpdatedBy and UpdatedOn values.
        /// </summary>
        /// <param name="entity">The object to update.</param>
        /// <returns>The updated object from a data-source.</returns>
        Task<TModel> UpdateDirect(TModel entity);

        /// <summary>
        /// Updates the specified objects in a data-source.
        /// </summary>
        /// <param name="entities">The objects to update.</param>
        /// <returns>The updated objects from a data-source.</returns>
        Task<IList<TModel>> Update(IList<TModel> entities);

        /// <summary>
        /// Updates the specified objects in a data-source. Will not override the InsertedBy, InsertedOn, UpdatedBy and UpdatedOn values.
        /// </summary>
        /// <param name="entities">The objects to update.</param>
        /// <returns>The updated objects from a data-source.</returns>
        Task<IList<TModel>> UpdateDirect(IList<TModel> entities);

        /// <summary>
        /// Updates only the specified property for the specified entities.
        /// </summary>
        /// <typeparam name="TKey">The type of the property to update.</typeparam>
        /// <param name="entities">The entities to update.</param>
        /// <param name="property">The property to update.</param>
        /// <param name="value">The value to update.</param>
        Task Update<TKey>(IList<TModel> entities, Expression<Func<TModel, TKey>> property, TKey value);

        /// <summary>
        /// Updates only the specified property for the specified entity ids.
        /// </summary>
        /// <typeparam name="TKey">The type of the property to update.</typeparam>
        /// <param name="ids">The entity ids to update.</param>
        /// <param name="property">The property to update.</param>
        /// <param name="value">The value to update.</param>
        Task Update<TKey>(IList<Guid> ids, Expression<Func<TModel, TKey>> property, TKey value);

        #endregion

        #region <| Delete |>

        /// <summary>
        /// Deletes the specified object from a data-source.
        /// </summary>
        /// <param name="entity">The object to delete.</param>
        /// <returns>True if successful, false otherwise.</returns>
        Task<bool> Delete(TModel entity);

        /// <summary>
        /// Deletes the specified objects from a data-source.
        /// </summary>
        /// <param name="entities">The objects to delete.</param>
        /// <returns>True if successful, false otherwise.</returns>
        Task<bool> Delete(IList<TModel> entities);

        /// <summary>
        /// Deletes an object with the specified unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the object to delete.</param>
        /// <returns>True if successful, false otherwise.</returns>
        Task<bool> Delete(Guid id);

        /// <summary>
        /// Deletes multiple objects with the specified unique identifiers.
        /// </summary>
        /// <param name="ids">The unique identifiers of the objects to delete.</param>
        /// <returns>True if successful, false otherwise.</returns>
        Task<bool> Delete(IList<Guid> ids);

        #endregion

        #region <| Truncate |>

        /// <summary>
        /// Deletes all entities from the corresponding database table.
        /// </summary>
        /// <returns>The number of affected rows.</returns>
        Task Truncate();

        #endregion

        #region <| Raw SQL |>

        /// <summary>
        /// Executes the specified raw SQL query against the database.
        /// </summary>
        /// <param name="sql">The SQL query to execute.</param>
        /// <param name="parameters">Values for parameters used in the raw SQL query.</param>
        /// <returns>The number of affected rows.</returns>
        Task<int> RawSql(string sql, params object[] parameters);

        Task<IList<TModel>> SelectRawSql(string sql, params object[] parameters);

        Task<IList<TModel>> SelectRawSql<TProperty>(string sql, Expression<Func<TModel, TProperty>> include, params object[] parameters);

        #endregion
    }
}