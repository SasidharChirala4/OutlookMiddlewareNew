using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Edreams.Common.Exceptions;
using Edreams.OutlookMiddleware.Common.Extensions;
using Edreams.OutlookMiddleware.Common.Security.Interfaces;
using Edreams.OutlookMiddleware.DataAccess.Exceptions;
using Edreams.OutlookMiddleware.DataAccess.Repositories.Helpers;
using Edreams.OutlookMiddleware.DataAccess.Repositories.Interfaces;
using Edreams.OutlookMiddleware.Model.Base;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;

namespace Edreams.OutlookMiddleware.DataAccess.Repositories
{
    /// <summary>
    /// Base class implementing a generic repository for CRUD operations using Entity Framework Core.
    /// </summary>
    /// <typeparam name="TModel">The entity type this generic repository is used for.</typeparam>
    public class Repository<TModel> : IRepository<TModel> where TModel : ModelBase
    {
        #region <| Private Members |>

        private readonly DbContext _dbContext;
        private readonly DbSet<TModel> _dbSet;
        private readonly ISecurityContext _securityContext;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        #endregion

        #region <| Construction |>

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TModel}" /> class.
        /// </summary>
        /// <param name="dbContext">The Entity Framework Core <see cref="DbContext" />.</param>
        /// <param name="dbSet">The Entity Framework Core <see cref="DbSet{TModel}" /> this repository is used for.</param>
        /// <param name="securityContext">The security context.</param>
        protected Repository(
            DbContext dbContext,
            DbSet<TModel> dbSet,
            ISecurityContext securityContext)
        {
            _dbContext = dbContext;
            _dbSet = dbSet;
            _securityContext = securityContext;
        }

        #endregion

        #region <| Exists |>

        /// <summary>
        /// Checks the existence of objects using this repository.
        /// </summary>
        /// <returns>True if objects are found, false otherwise.</returns>
        public async Task<bool> Exists()
        {
            return await Lock(
                async () => await _dbSet.AnyAsync());
        }

        /// <summary>
        /// Checks the existence of objects using this repository.
        /// </summary>
        /// <param name="predicate">The predicate used to find objects.</param>
        /// <returns>True if objects are found, false otherwise.</returns>
        public async Task<bool> Exists(
            Expression<Func<TModel, bool>> predicate)
        {
            return await Lock(
                async () => await _dbSet.AnyAsync(predicate));
        }

        #endregion

        #region <| Count |>

        /// <summary>
        /// Counts the objects using this repository.
        /// </summary>
        /// <returns>The number of objects.</returns>
        public async Task<int> Count()
        {
            return await Lock(
                async () => await _dbSet.CountAsync());
        }

        /// <summary>
        /// Counts the objects using this repository.
        /// </summary>
        /// <param name="predicate">The predicate used to find objects.</param>
        /// <returns>The number of objects.</returns>
        public async Task<int> Count(
            Expression<Func<TModel, bool>> predicate)
        {
            return await Lock(
                async () => await _dbSet.CountAsync(predicate));
        }

        #endregion

        #region <| GetAll |>

        /// <summary>
        /// Gets all objects from a data-source.
        /// </summary>
        /// <returns>A list of objects from a data-source.</returns>
        public async Task<IList<TModel>> GetAll()
        {
            return await Lock(
                async () => await _dbSet.AsNoTracking().ToListAsync());
        }

        /// <summary>
        /// Gets all objects from a data-source.
        /// </summary>
        /// <typeparam name="TProperty">The type of the navigation property.</typeparam>
        /// <param name="include">The navigation property to include using a SQL JOIN.</param>
        /// <returns>A list of objects from a data-source.</returns>
        public async Task<IList<TModel>> GetAll<TProperty>(Expression<Func<TModel, TProperty>> include)
        {
            return await Lock(
                async () => await _dbSet.Include(include).AsNoTracking().ToListAsync());
        }

        /// <summary>
        /// Gets all objects from a data-source.
        /// </summary>
        /// <typeparam name="TProperty1">The first type of the navigation property.</typeparam>
        /// <typeparam name="TProperty2">The second type of the navigation property.</typeparam>
        /// <param name="include1">The first navigation property to include using a SQL JOIN.</param>
        /// <param name="include2">The second navigation property to include using a SQL JOIN.</param>
        /// <returns>A list of objects from a data-source.</returns>
        public async Task<IList<TModel>> GetAll<TProperty1, TProperty2>(
            Expression<Func<TModel, TProperty1>> include1, Expression<Func<TModel, TProperty2>> include2)
        {
            return await Lock(
                async () => await _dbSet.Include(include1).Include(include2).AsNoTracking().ToListAsync());
        }

        /// <summary>
        /// Gets all objects from a data-source.
        /// </summary>
        /// <typeparam name="TProperty1">The first type of the navigation property.</typeparam>
        /// <typeparam name="TProperty2">The second type of the navigation property.</typeparam>
        /// <typeparam name="TProperty3">The third type of the navigation property.</typeparam>
        /// <param name="include1">The first navigation property to include using a SQL JOIN.</param>
        /// <param name="include2">The second navigation property to include using a SQL JOIN.</param>
        /// <param name="include3">The third navigation property to include using a SQL JOIN.</param>
        /// <returns>A list of objects from a data-source.</returns>
        public async Task<IList<TModel>> GetAll<TProperty1, TProperty2, TProperty3>(
            Expression<Func<TModel, TProperty1>> include1,
            Expression<Func<TModel, TProperty2>> include2,
            Expression<Func<TModel, TProperty3>> include3)
        {
            return await Lock(
                async () => await _dbSet.Include(include1).Include(include2).Include(include3).AsNoTracking().ToListAsync());
        }

        /// <summary>
        /// Gets all objects from a data-source and project.
        /// </summary>
        /// <typeparam name="TResult">The type of the projected result.</typeparam>
        /// <param name="projection">The projection to apply.</param>
        /// <returns>A list of projected objects from a data-source.</returns>
        public async Task<IList<TResult>> GetAllAndProject<TResult>(
            Expression<Func<TModel, TResult>> projection)
        {
            return await Lock(
                async () => await _dbSet.AsNoTracking().Select(projection).ToListAsync());
        }

        /// <summary>
        /// Gets all objects from a data-source and project.
        /// </summary>
        /// <typeparam name="TProperty">The type of the navigation property.</typeparam>
        /// <typeparam name="TResult">The type of the projected result.</typeparam>
        /// <param name="include">The navigation property to include using a SQL JOIN.</param>
        /// <param name="projection">The projection to apply.</param>
        /// <returns>A list of projected objects from a data-source.</returns>
        public async Task<IList<TResult>> GetAllAndProject<TProperty, TResult>(
            Expression<Func<TModel, TProperty>> include,
            Expression<Func<TModel, TResult>> projection)
        {
            return await Lock(
                async () => await _dbSet.Include(include).AsNoTracking().Select(projection).ToListAsync());
        }

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
        public async Task<IList<TResult>> GetAllAndProject<TProperty1, TProperty2, TResult>(
            Expression<Func<TModel, TProperty1>> include1,
            Expression<Func<TModel, TProperty2>> include2,
            Expression<Func<TModel, TResult>> projection)
        {
            return await Lock(
                async () => await _dbSet.Include(include1).Include(include2).AsNoTracking().Select(projection).ToListAsync());
        }

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
        public async Task<IList<TResult>> GetAllAndProject<TProperty1, TProperty2, TProperty3, TResult>(
            Expression<Func<TModel, TProperty1>> include1,
            Expression<Func<TModel, TProperty2>> include2,
            Expression<Func<TModel, TProperty3>> include3,
            Expression<Func<TModel, TResult>> projection)
        {
            return await Lock(
                async () => await _dbSet.Include(include1).Include(include2).Include(include3).AsNoTracking().Select(projection).ToListAsync());
        }

        #endregion

        #region <| GetSingle |>

        /// <summary>
        /// Gets a single object from a data-source.
        /// </summary>
        /// <returns>A single object from a data-source.</returns>
        public async Task<TModel> GetSingle()
        {
            try
            {
                return await Lock(async () => await _dbSet.AsNoTracking().SingleOrDefaultAsync());
            }
            catch (InvalidOperationException ex)
            {
                throw new EdreamsDataAccessException(ex);
            }
        }

        /// <summary>
        /// Gets a single object from a data-source.
        /// </summary>
        /// <typeparam name="TProperty">The type of the navigation property.</typeparam>
        /// <param name="include">The navigation property to include using a SQL JOIN.</param>
        /// <returns>A single object from a data-source.</returns>
        public async Task<TModel> GetSingle<TProperty>(
            Expression<Func<TModel, TProperty>> include)
        {
            try
            {
                return await Lock(async () => await _dbSet.AsNoTracking().Include(include).SingleOrDefaultAsync());
            }
            catch (InvalidOperationException ex)
            {
                throw new EdreamsDataAccessException(ex);
            }
        }

        /// <summary>
        /// Gets a single object from a data-source.
        /// </summary>
        /// <typeparam name="TProperty1">The first type of the navigation property.</typeparam>
        /// <typeparam name="TProperty2">The second type of the navigation property.</typeparam>
        /// <param name="include1">The first navigation property to include using a SQL JOIN.</param>
        /// <param name="include2">The second navigation property to include using a SQL JOIN.</param>
        /// <returns>A single object from a data-source.</returns>
        public async Task<TModel> GetSingle<TProperty1, TProperty2>(
            Expression<Func<TModel, TProperty1>> include1,
            Expression<Func<TModel, TProperty2>> include2)
        {
            try
            {
                return await Lock(
                    async () => await _dbSet.AsNoTracking().Include(include1).Include(include2).SingleOrDefaultAsync());
            }
            catch (InvalidOperationException ex)
            {
                throw new EdreamsDataAccessException(ex);
            }
        }

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
        public async Task<TModel> GetSingle<TProperty1, TProperty2, TProperty3>(
            Expression<Func<TModel, TProperty1>> include1,
            Expression<Func<TModel, TProperty2>> include2,
            Expression<Func<TModel, TProperty3>> include3)
        {
            try
            {
                return await Lock(
                    async () => await _dbSet.AsNoTracking().Include(include1).Include(include2).Include(include3).SingleOrDefaultAsync());
            }
            catch (InvalidOperationException ex)
            {
                throw new EdreamsDataAccessException(ex);
            }
        }

        /// <summary>
        /// Gets a single object from a data-source.
        /// </summary>
        /// <param name="predicate">The predicate used to find a single object.</param>
        /// <returns>A single object from a data-source.</returns>
        public async Task<TModel> GetSingle(
            Expression<Func<TModel, bool>> predicate)
        {
            try
            {
                return await Lock(async () => await _dbSet.AsNoTracking().SingleOrDefaultAsync(predicate));
            }
            catch (InvalidOperationException ex)
            {
                throw new EdreamsDataAccessException(ex);
            }
        }

        /// <summary>
        /// Gets a single object from a data-source.
        /// </summary>
        /// <typeparam name="TProperty">The type of the navigation property.</typeparam>
        /// <param name="predicate">The predicate used to find a single object.</param>
        /// <param name="include">The navigation property to include using a SQL JOIN.</param>
        /// <returns>A single object from a data-source.</returns>
        public async Task<TModel> GetSingle<TProperty>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TProperty>> include)
        {
            try
            {
                return await Lock(async () => await _dbSet.AsNoTracking().Include(include).SingleOrDefaultAsync(predicate));
            }
            catch (InvalidOperationException ex)
            {
                throw new EdreamsDataAccessException(ex);
            }
        }

        /// <summary>
        /// Gets a single object from a data-source.
        /// </summary>
        /// <typeparam name="TProperty1">The first type of the navigation property.</typeparam>
        /// <typeparam name="TProperty2">The second type of the navigation property.</typeparam>
        /// <param name="predicate">The predicate used to find a single object.</param>
        /// <param name="include1">The first navigation property to include using a SQL JOIN.</param>
        /// <param name="include2">The second navigation property to include using a SQL JOIN.</param>
        /// <returns>A single object from a data-source.</returns>
        public async Task<TModel> GetSingle<TProperty1, TProperty2>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TProperty1>> include1,
            Expression<Func<TModel, TProperty2>> include2)
        {
            try
            {
                return await Lock(
                    async () => await _dbSet.AsNoTracking().Include(include1).Include(include2).SingleOrDefaultAsync(predicate));
            }
            catch (InvalidOperationException ex)
            {
                throw new EdreamsDataAccessException(ex);
            }
        }

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
        public async Task<TModel> GetSingle<TProperty1, TProperty2, TProperty3>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TProperty1>> include1,
            Expression<Func<TModel, TProperty2>> include2,
            Expression<Func<TModel, TProperty3>> include3)
        {
            try
            {
                return await Lock(
                    async () => await _dbSet.AsNoTracking().Include(include1).Include(include2).Include(include3).SingleOrDefaultAsync(predicate));
            }
            catch (InvalidOperationException ex)
            {
                throw new EdreamsDataAccessException(ex);
            }
        }

        /// <summary>
        /// Gets a single object from a data-source and project.
        /// </summary>
        /// <typeparam name="TResult">The type of the projected result.</typeparam>
        /// <param name="projection">The projection to apply.</param>
        /// <returns>A single projected object from a data-source.</returns>
        public async Task<TResult> GetSingleAndProject<TResult>(
            Expression<Func<TModel, TResult>> projection)
        {
            try
            {
                return await Lock(
                    async () => await _dbSet.AsNoTracking().Select(projection).SingleOrDefaultAsync());
            }
            catch (InvalidOperationException ex)
            {
                throw new EdreamsDataAccessException(ex);
            }
        }

        /// <summary>
        /// Gets a single object from a data-source and project.
        /// </summary>
        /// <typeparam name="TProperty">The type of the navigation property.</typeparam>
        /// <typeparam name="TResult">The type of the projected result.</typeparam>
        /// <param name="include">The navigation property to include using a SQL JOIN.</param>
        /// <param name="projection">The projection to apply.</param>
        /// <returns>A single projected object from a data-source.</returns>
        public async Task<TResult> GetSingleAndProject<TProperty, TResult>(
            Expression<Func<TModel, TProperty>> include,
            Expression<Func<TModel, TResult>> projection)
        {
            try
            {
                return await Lock(
                    async () => await _dbSet.AsNoTracking().Include(include).Select(projection).SingleOrDefaultAsync());
            }
            catch (InvalidOperationException ex)
            {
                throw new EdreamsDataAccessException(ex);
            }
        }

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
        public async Task<TResult> GetSingleAndProject<TProperty1, TProperty2, TResult>(
            Expression<Func<TModel, TProperty1>> include1,
            Expression<Func<TModel, TProperty2>> include2,
            Expression<Func<TModel, TResult>> projection)
        {
            try
            {
                return await Lock(
                    async () => await _dbSet.AsNoTracking().Include(include1).Include(include2).Select(projection).SingleOrDefaultAsync());
            }
            catch (InvalidOperationException ex)
            {
                throw new EdreamsDataAccessException(ex);
            }
        }

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
        public async Task<TResult> GetSingleAndProject<TProperty1, TProperty2, TProperty3, TResult>(
            Expression<Func<TModel, TProperty1>> include1,
            Expression<Func<TModel, TProperty2>> include2,
            Expression<Func<TModel, TProperty3>> include3,
            Expression<Func<TModel, TResult>> projection)
        {
            try
            {
                return await Lock(
                    async () => await _dbSet.AsNoTracking().Include(include1).Include(include2).Include(include3).Select(projection).SingleOrDefaultAsync());
            }
            catch (InvalidOperationException ex)
            {
                throw new EdreamsDataAccessException(ex);
            }
        }

        /// <summary>
        /// Gets a single object from a data-source and project.
        /// </summary>
        /// <typeparam name="TResult">The type of the projected result.</typeparam>
        /// <param name="predicate">The predicate used to find a single object.</param>
        /// <param name="projection">The projection to apply.</param>
        /// <returns>A single projected object from a data-source.</returns>
        public async Task<TResult> GetSingleAndProject<TResult>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TResult>> projection)
        {
            try
            {
                return await Lock(
                    async () => await _dbSet.AsNoTracking().Where(predicate).Select(projection).SingleOrDefaultAsync());
            }
            catch (InvalidOperationException ex)
            {
                throw new EdreamsDataAccessException(ex);
            }
        }

        /// <summary>
        /// Gets a single object from a data-source and project.
        /// </summary>
        /// <typeparam name="TProperty">The type of the navigation property.</typeparam>
        /// <typeparam name="TResult">The type of the projected result.</typeparam>
        /// <param name="predicate">The predicate used to find a single object.</param>
        /// <param name="include">The navigation property to include using a SQL JOIN.</param>
        /// <param name="projection">The projection to apply.</param>
        /// <returns>A single projected object from a data-source.</returns>
        public async Task<TResult> GetSingleAndProject<TProperty, TResult>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TProperty>> include,
            Expression<Func<TModel, TResult>> projection)
        {
            try
            {
                return await Lock(
                    async () => await _dbSet.AsNoTracking().Where(predicate).Include(include).Select(projection).SingleOrDefaultAsync());
            }
            catch (InvalidOperationException ex)
            {
                throw new EdreamsDataAccessException(ex);
            }
        }

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
        public async Task<TResult> GetSingleAndProject<TProperty1, TProperty2, TResult>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TProperty1>> include1,
            Expression<Func<TModel, TProperty2>> include2,
            Expression<Func<TModel, TResult>> projection)
        {
            try
            {
                return await Lock(
                    async () => await _dbSet.AsNoTracking().Where(predicate).Include(include1).Include(include2).Select(projection).SingleOrDefaultAsync());
            }
            catch (InvalidOperationException ex)
            {
                throw new EdreamsDataAccessException(ex);
            }
        }

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
        public async Task<TResult> GetSingleAndProject<TProperty1, TProperty2, TProperty3, TResult>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TProperty1>> include1,
            Expression<Func<TModel, TProperty2>> include2,
            Expression<Func<TModel, TProperty3>> include3,
            Expression<Func<TModel, TResult>> projection)
        {
            try
            {
                return await Lock(
                    async () => await _dbSet.AsNoTracking().Where(predicate).Include(include1).Include(include2).Include(include3).Select(projection).SingleOrDefaultAsync());
            }
            catch (InvalidOperationException ex)
            {
                throw new EdreamsDataAccessException(ex);
            }
        }

        #endregion

        #region <| GetFirst |>

        /// <summary>
        /// Gets the first object of a list of ordered object from a data-source.
        /// </summary>
        /// <param name="predicate">The predicate used to find objects.</param>
        /// <returns>
        /// The first object of a list of ordered objects from a data-source.
        /// </returns>
        public async Task<TModel> GetFirst(
            Expression<Func<TModel, bool>> predicate)
        {
            return await Lock(async () => await _dbSet.FirstOrDefaultAsync(predicate));
        }

        /// <summary>
        /// Gets the first object of a list of ordered object from a data-source.
        /// </summary>
        /// <typeparam name="TProperty">The type of the navigation property.</typeparam>
        /// <param name="predicate">The predicate used to find objects.</param>
        /// <param name="include">The navigation property to include using a SQL JOIN.</param>
        /// <returns>
        /// The first object of a list of ordered objects from a data-source.
        /// </returns>
        public async Task<TModel> GetFirst<TProperty>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TProperty>> include)
        {
            return await Lock(
                async () => await _dbSet.Include(include).FirstOrDefaultAsync(predicate));
        }

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
        public async Task<TModel> GetFirst<TProperty1, TProperty2>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TProperty1>> include1,
            Expression<Func<TModel, TProperty2>> include2)
        {
            return await Lock(
                async () => await _dbSet.Include(include1).Include(include2).FirstOrDefaultAsync(predicate));
        }

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
        public async Task<TModel> GetFirst<TProperty1, TProperty2, TProperty3>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TProperty1>> include1,
            Expression<Func<TModel, TProperty2>> include2,
            Expression<Func<TModel, TProperty3>> include3)
        {
            return await Lock(
                async () => await _dbSet.Include(include1).Include(include2).Include(include3).FirstOrDefaultAsync(predicate));
        }

        #endregion

        #region <| GetFirstAscending |>

        /// <summary>
        /// Gets the first object of a list of ordered object from a data-source.
        /// </summary>
        /// <param name="predicate">The predicate used to find objects.</param>
        /// <param name="orderBy">The order by.</param>
        /// <returns>
        /// The first object of a list of ordered objects from a data-source.
        /// </returns>
        public async Task<TModel> GetFirstAscending<TKey>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TKey>> orderBy)
        {
            return await Lock(
                async () => await _dbSet.OrderBy(orderBy).FirstOrDefaultAsync(predicate));
        }

        /// <summary>
        /// Gets the first object of a list of ordered object from a data-source.
        /// </summary>
        /// <param name="predicate">The predicate used to find objects.</param>
        /// <param name="orderBy">The order by information.</param>
        /// <returns>
        /// The first object of a list of ordered objects from a data-source.
        /// </returns>
        public async Task<TModel> GetFirstAscending<TKey1, TKey2>(
            Expression<Func<TModel, bool>> predicate,
            OrderBy<TModel, TKey1, TKey2> orderBy)
        {
            return await Lock(
                async () => await _dbSet.OrderBy(orderBy.By1).ThenBy(orderBy.By2).FirstOrDefaultAsync(predicate));
        }

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
        public async Task<TModel> GetFirstAscending<TKey, TProperty>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TKey>> orderBy,
            Expression<Func<TModel, TProperty>> include)
        {
            return await Lock(
                async () => await _dbSet.Include(include).OrderBy(orderBy).FirstOrDefaultAsync(predicate));
        }

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
        public async Task<TModel> GetFirstAscending<TKey, TProperty1, TProperty2>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TKey>> orderBy,
            Expression<Func<TModel, TProperty1>> include1,
            Expression<Func<TModel, TProperty2>> include2)
        {
            return await Lock(
                async () =>
                    await _dbSet.Include(include1).Include(include2).OrderBy(orderBy).FirstOrDefaultAsync(predicate));
        }

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
        public async Task<TModel> GetFirstAscending<TKey, TProperty1, TProperty2, TProperty3>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TKey>> orderBy,
            Expression<Func<TModel, TProperty1>> include1,
            Expression<Func<TModel, TProperty2>> include2,
            Expression<Func<TModel, TProperty3>> include3)
        {
            return await Lock(
                async () =>
                    await _dbSet.Include(include1).Include(include2).Include(include3).OrderBy(orderBy).FirstOrDefaultAsync(predicate));
        }

        #endregion

        #region <| GetFirstDescending |>

        /// <summary>
        /// Gets the first object of a list of descending ordered object from a data-source.
        /// </summary>
        /// <param name="predicate">The predicate used to find objects.</param>
        /// <param name="orderBy">The order by.</param>
        /// <returns>
        /// The first object of a list of ordered objects from a data-source.
        /// </returns>
        public async Task<TModel> GetFirstDescending<TKey>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TKey>> orderBy)
        {
            return await Lock(
                async () => await _dbSet.OrderByDescending(orderBy).FirstOrDefaultAsync(predicate));
        }

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
        public async Task<TModel> GetFirstDescending<TKey, TProperty>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TKey>> orderBy,
            Expression<Func<TModel, TProperty>> include)
        {
            return await Lock(
                async () => await _dbSet.Include(include).OrderByDescending(orderBy).FirstOrDefaultAsync(predicate));
        }

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
        public async Task<TModel> GetFirstDescending<TKey, TProperty1, TProperty2>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TKey>> orderBy,
            Expression<Func<TModel, TProperty1>> include1,
            Expression<Func<TModel, TProperty2>> include2)
        {
            return await Lock(
                async () => await _dbSet.Include(include1).Include(include2).OrderByDescending(orderBy).FirstOrDefaultAsync(predicate));
        }

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
        public async Task<TModel> GetFirstDescending<TKey, TProperty1, TProperty2, TProperty3>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TKey>> orderBy,
            Expression<Func<TModel, TProperty1>> include1,
            Expression<Func<TModel, TProperty2>> include2,
            Expression<Func<TModel, TProperty3>> include3)
        {
            return await Lock(
                async () => await _dbSet.Include(include1).Include(include2).Include(include3).OrderByDescending(orderBy).FirstOrDefaultAsync(predicate));
        }

        #endregion

        #region <| Find |>

        /// <summary>
        /// Finds multiple objects from a data-source.
        /// </summary>
        /// <param name="predicate">The predicate used to find objects.</param>
        /// <returns>A list of objects from a data-source.</returns>
        public async Task<IList<TModel>> Find(
            Expression<Func<TModel, bool>> predicate)
        {
            return await Lock(
                async () => await _dbSet.Where(predicate).AsNoTracking().ToListAsync());
        }

        /// <summary>
        /// Finds multiple objects from a data-source.
        /// </summary>
        /// <typeparam name="TProperty">The type of the navigation property.</typeparam>
        /// <param name="predicate">The predicate used to find objects.</param>
        /// <param name="include">The navigation property to include using a SQL JOIN.</param>
        /// <returns>A list of objects from a data-source.</returns>
        public async Task<IList<TModel>> Find<TProperty>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TProperty>> include)
        {
            return await Lock(async () =>
                await _dbSet.Include(include).Where(predicate).AsNoTracking().ToListAsync());
        }

        /// <summary>
        /// Finds multiple objects from a data-source.
        /// </summary>
        /// <typeparam name="TProperty1">The first type of the navigation property.</typeparam>
        /// <typeparam name="TProperty2">The second type of the navigation property.</typeparam>
        /// <param name="predicate">The predicate used to find objects.</param>
        /// <param name="include1">The first navigation property to include using a SQL JOIN.</param>
        /// <param name="include2">The second navigation property to include using a SQL JOIN.</param>
        /// <returns>A list of objects from a data-source.</returns>
        public async Task<IList<TModel>> Find<TProperty1, TProperty2>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TProperty1>> include1,
            Expression<Func<TModel, TProperty2>> include2)
        {
            return await Lock(async () =>
                await _dbSet.Include(include1).Include(include2).Where(predicate).AsNoTracking().ToListAsync());
        }

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
        public async Task<IList<TModel>> Find<TProperty1, TProperty2, TProperty3>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TProperty1>> include1,
            Expression<Func<TModel, TProperty2>> include2,
            Expression<Func<TModel, TProperty3>> include3)
        {
            return await Lock(async () =>
                await _dbSet.Include(include1).Include(include2).Include(include3).Where(predicate).AsNoTracking().ToListAsync());
        }

        /// <summary>
        /// Finds multiple objects from a data-source and project.
        /// </summary>
        /// <typeparam name="TResult">The type of the projected result.</typeparam>
        /// <param name="predicate">The predicate used to find objects.</param>
        /// <param name="projection">The projection to apply.</param>
        /// <returns>A list of projected objects from a data-source.</returns>
        public async Task<IList<TResult>> FindAndProject<TResult>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TResult>> projection)
        {
            return await Lock(async () =>
                await _dbSet.Where(predicate).AsNoTracking().Select(projection).ToListAsync());
        }

        /// <summary>
        /// Finds multiple objects from a data-source and project.
        /// </summary>
        /// <typeparam name="TProperty">The type of the navigation property.</typeparam>
        /// <typeparam name="TResult">The type of the projected result.</typeparam>
        /// <param name="predicate">The predicate used to find objects.</param>
        /// <param name="include">The navigation property to include using a SQL JOIN.</param>
        /// <param name="projection">The projection to apply.</param>
        /// <returns>A list of projected objects from a data-source.</returns>
        public async Task<IList<TResult>> FindAndProject<TProperty, TResult>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TProperty>> include,
            Expression<Func<TModel, TResult>> projection)
        {
            return await Lock(async () =>
                await _dbSet.Include(include).Where(predicate).AsNoTracking().Select(projection).ToListAsync());
        }

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
        public async Task<IList<TResult>> FindAndProject<TProperty1, TProperty2, TResult>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TProperty1>> include1,
            Expression<Func<TModel, TProperty2>> include2,
            Expression<Func<TModel, TResult>> projection)
        {
            return await Lock(async () =>
                await _dbSet.Include(include1).Include(include2).Where(predicate).AsNoTracking().Select(projection).ToListAsync());
        }

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
        public async Task<IList<TResult>> FindAndProject<TProperty1, TProperty2, TProperty3, TResult>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TProperty1>> include1,
            Expression<Func<TModel, TProperty2>> include2,
            Expression<Func<TModel, TProperty3>> include3,
            Expression<Func<TModel, TResult>> projection)
        {
            return await Lock(async () =>
                await _dbSet.Include(include1).Include(include2).Include(include3).Where(predicate).AsNoTracking().Select(projection).ToListAsync());
        }

        #endregion

        #region <| FindAscending |>

        /// <summary>
        /// Finds multiple objects from a data-source.
        /// </summary>
        /// <typeparam name="TOrderKey">The type of the order by property.</typeparam>
        /// <param name="predicate">The predicate used to find objects.</param>
        /// <param name="orderBy">The order by.</param>
        /// <returns>A list of objects from a data-source.</returns>
        public async Task<IList<TModel>> FindAscending<TOrderKey>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TOrderKey>> orderBy)
        {
            return await Lock(
                async () => await _dbSet.Where(predicate).OrderBy(orderBy).ToListAsync());
        }

        /// <summary>
        /// Finds multiple objects from a data-source.
        /// </summary>
        /// <typeparam name="TProperty">The type of the navigation property.</typeparam>
        /// <typeparam name="TOrderKey">The type of the order by property.</typeparam>
        /// <param name="predicate">The predicate used to find objects.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="include">The navigation property to include using a SQL JOIN.</param>
        /// <returns>A list of objects from a data-source.</returns>
        public async Task<IList<TModel>> FindAscending<TProperty, TOrderKey>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TOrderKey>> orderBy,
            Expression<Func<TModel, TProperty>> include)
        {
            return await Lock(
                async () => await _dbSet.Where(predicate).OrderBy(orderBy).Include(include).AsNoTracking().ToListAsync());
        }

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
        public async Task<IList<TModel>> FindAscending<TProperty1, TProperty2, TOrderKey>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TOrderKey>> orderBy,
            Expression<Func<TModel, TProperty1>> include1,
            Expression<Func<TModel, TProperty2>> include2)
        {
            return await Lock(
                async () => await _dbSet.Where(predicate).OrderBy(orderBy).Include(include1).Include(include2).AsNoTracking().ToListAsync());
        }

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
        public async Task<IList<TModel>> FindAscending<TProperty1, TProperty2, TProperty3, TOrderKey>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TOrderKey>> orderBy,
            Expression<Func<TModel, TProperty1>> include1,
            Expression<Func<TModel, TProperty2>> include2,
            Expression<Func<TModel, TProperty3>> include3)
        {
            return await Lock(
                async () => await _dbSet.Where(predicate).OrderBy(orderBy).Include(include1).Include(include2).Include(include3).AsNoTracking().ToListAsync());
        }

        #endregion

        #region <| FindDescending |>

        /// <summary>
        /// Finds multiple objects from a data-source.
        /// </summary>
        /// <typeparam name="TOrderKey">The type of the order by property.</typeparam>
        /// <param name="predicate">The predicate used to find objects.</param>
        /// <param name="orderBy">The order by.</param>
        /// <returns>A list of objects from a data-source.</returns>
        public async Task<IList<TModel>> FindDescending<TOrderKey>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TOrderKey>> orderBy)
        {
            return await Lock(
                async () => await _dbSet.Where(predicate).OrderByDescending(orderBy).AsNoTracking().ToListAsync());
        }

        /// <summary>
        /// Finds multiple objects from a data-source.
        /// </summary>
        /// <typeparam name="TOrderKey">The type of the order by property.</typeparam>
        /// <param name="predicate">The predicate used to find objects.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="limit">Applies paging to this request.</param>
        /// <returns>A list of objects from a data-source.</returns>
        public async Task<IList<TModel>> FindDescending<TOrderKey>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TOrderKey>> orderBy, Limit limit)
        {
            return await Lock(
                async () =>
                {
                    if (Limit.HasNone(limit))
                    {
                        return await _dbSet.Where(predicate).OrderByDescending(orderBy).AsNoTracking().ToListAsync();
                    }
                    return await _dbSet.Where(predicate).OrderByDescending(orderBy)
                        .Skip(limit.Skip).Take(limit.Take).AsNoTracking().ToListAsync();
                });
        }

        /// <summary>
        /// Finds multiple objects from a data-source.
        /// </summary>
        /// <typeparam name="TProperty">The type of the navigation property.</typeparam>
        /// <typeparam name="TOrderKey">The type of the order by property.</typeparam>
        /// <param name="predicate">The predicate used to find objects.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="include">The navigation property to include using a SQL JOIN.</param>
        /// <returns>A list of objects from a data-source.</returns>
        public async Task<IList<TModel>> FindDescending<TProperty, TOrderKey>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TOrderKey>> orderBy,
            Expression<Func<TModel, TProperty>> include)
        {
            return await Lock(
                async () => await _dbSet.Where(predicate).OrderByDescending(orderBy).Include(include).AsNoTracking().ToListAsync());
        }

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
        public async Task<IList<TModel>> FindDescending<TProperty1, TProperty2, TOrderKey>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TOrderKey>> orderBy,
            Expression<Func<TModel, TProperty1>> include1,
            Expression<Func<TModel, TProperty2>> include2)
        {
            return await Lock(
                async () => await _dbSet.Where(predicate).OrderByDescending(orderBy).Include(include1).Include(include2).AsNoTracking().ToListAsync());
        }

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
        public async Task<IList<TModel>> FindDescending<TProperty1, TProperty2, TProperty3, TOrderKey>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TOrderKey>> orderBy,
            Expression<Func<TModel, TProperty1>> include1,
            Expression<Func<TModel, TProperty2>> include2,
            Expression<Func<TModel, TProperty3>> include3)
        {
            return await Lock(
                async () => await _dbSet.Where(predicate).OrderByDescending(orderBy).Include(include1).Include(include2).Include(include3).AsNoTracking().ToListAsync());
        }

        /// <summary>
        /// Finds multiple objects from a data-source.
        /// </summary>
        /// <typeparam name="TProperty1">The first type of the navigation property.</typeparam>
        /// <typeparam name="TOrderKey">The type of the order by property.</typeparam>
        /// <param name="predicate">The predicate used to find objects.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="include">The navigation property to include using a SQL JOIN.</param>
        /// <param name="limit">Applies paging to this request.</param>
        /// <returns>A list of objects from a data-source.</returns>
        public async Task<IList<TModel>> FindDescending<TProperty1, TOrderKey>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TOrderKey>> orderBy,
            Expression<Func<TModel, TProperty1>> include,
            Limit limit)
        {
            return await Lock(
                async () =>
                {
                    if (Limit.HasNone(limit))
                    {
                        return await
                        _dbSet.Where(predicate)
                            .OrderByDescending(orderBy)
                            .Include(include)
                            .AsNoTracking()
                            .ToListAsync();
                    }
                    return await _dbSet.Where(predicate).OrderByDescending(orderBy)
                            .Include(include)
                            .Skip(limit.Skip)
                            .Take(limit.Take)
                            .AsNoTracking()
                            .ToListAsync();
                });
        }

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
        public async Task<IList<TModel>> FindDescending<TProperty1, TProperty2, TOrderKey>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TOrderKey>> orderBy,
            Expression<Func<TModel, TProperty1>> include1,
            Expression<Func<TModel, TProperty2>> include2,
            Limit limit)
        {
            return await Lock(
                async () =>
                {
                    if (Limit.HasNone(limit))
                    {
                        return await
                        _dbSet.Where(predicate)
                            .OrderByDescending(orderBy)
                            .Include(include1)
                            .Include(include2)
                            .AsNoTracking()
                            .ToListAsync();
                    }
                    return await _dbSet.Where(predicate).OrderByDescending(orderBy)
                            .Include(include1)
                            .Include(include2)
                            .Skip(limit.Skip)
                            .Take(limit.Take)
                            .AsNoTracking()
                            .ToListAsync();
                });
        }

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
        public async Task<IList<TModel>> FindDescending<TProperty1, TProperty2, TProperty3, TOrderKey>(
            Expression<Func<TModel, bool>> predicate,
            Expression<Func<TModel, TOrderKey>> orderBy,
            Expression<Func<TModel, TProperty1>> include1,
            Expression<Func<TModel, TProperty2>> include2,
            Expression<Func<TModel, TProperty3>> include3,
            Limit limit)
        {
            return await Lock(
                async () =>
                {
                    if (Limit.HasNone(limit))
                    {
                        return await
                        _dbSet.Where(predicate)
                            .OrderByDescending(orderBy)
                            .Include(include1)
                            .Include(include2)
                            .Include(include3)
                            .AsNoTracking()
                            .ToListAsync();
                    }
                    return await _dbSet.Where(predicate).OrderByDescending(orderBy)
                            .Include(include1)
                            .Include(include2)
                            .Include(include3)
                            .Skip(limit.Skip)
                            .Take(limit.Take)
                            .AsNoTracking()
                            .ToListAsync();
                });
        }


        #endregion

        #region <| Create |>

        /// <summary>
        /// Creates the specified object in a data-source.
        /// </summary>
        /// <param name="entity">The object to create.</param>
        /// <returns>The created object from a data-source.</returns>
        public async Task<TModel> Create(TModel entity)
        {
            return await Lock(async () =>
            {
                try
                {
                    CreateInternal(entity);
                    await _dbContext.SaveChangesAsync();
                    return entity;
                }
                catch (DbUpdateException ex)
                {
                    EdreamsDataAccessException exception = CheckException(ex);
                    if (exception == null)
                    {
                        throw;
                    }
                    throw exception;
                }
            });
        }

        /// <summary>
        /// Creates the specified object in a data-source. Will not override the InsertedBy and InsertedOn values.
        /// </summary>
        /// <param name="entity">The object to create.</param>
        /// <returns>The created object from a data-source.</returns>
        public async Task<TModel> CreateDirect(TModel entity)
        {
            return await Lock(async () =>
            {
                try
                {
                    CreateInternal(entity, false);
                    await _dbContext.SaveChangesAsync();
                    return entity;
                }
                catch (DbUpdateException ex)
                {
                    EdreamsDataAccessException exception = CheckException(ex);
                    if (exception == null)
                    {
                        throw;
                    }
                    throw exception;
                }
            });
        }

        /// <summary>
        /// Creates the specified objects in a data-source.
        /// </summary>
        /// <param name="entities">The objects to create.</param>
        /// <returns>The created objects from a data-source.</returns>
        public async Task<IList<TModel>> Create(IList<TModel> entities)
        {
            return await Lock(async () =>
            {
                try
                {
                    List<TModel> entityList = entities.ToList();
                    foreach (TModel entity in entityList)
                    {
                        CreateInternal(entity);
                    }
                    await _dbContext.SaveChangesAsync();
                    return entityList;
                }
                catch (DbUpdateException ex)
                {
                    EdreamsDataAccessException exception = CheckException(ex);
                    if (exception == null)
                    {
                        throw;
                    }
                    throw exception;
                }
            });
        }

        /// <summary>
        /// Creates the specified objects in a data-source. Will not override the InsertedBy and InsertedOn values.
        /// </summary>
        /// <param name="entities">The objects to create.</param>
        /// <returns>The created objects from a data-source.</returns>
        public async Task<IList<TModel>> CreateDirect(IList<TModel> entities)
        {
            return await Lock(async () =>
            {
                try
                {
                    List<TModel> entityList = entities.ToList();
                    foreach (TModel entity in entityList)
                    {
                        CreateInternal(entity, false);
                    }
                    await _dbContext.SaveChangesAsync();
                    return entityList;
                }
                catch (DbUpdateException ex)
                {
                    EdreamsDataAccessException exception = CheckException(ex);
                    if (exception == null)
                    {
                        throw;
                    }
                    throw exception;
                }
            });
        }

        private void CreateInternal(TModel entity, bool overWrite = true)
        {
            if (overWrite)
            {
                entity.Id = Guid.NewGuid();

                if (entity is IInsertedBy insertedByEntity)
                {
                    insertedByEntity.InsertedBy = _securityContext.PrincipalName;
                }

                if (entity is IInsertedOn insertedOnEntity)
                {
                    insertedOnEntity.InsertedOn = DateTime.UtcNow;
                }
            }
            _dbSet.Add(entity);
        }

        /// <summary>
        /// Creates the specified objects in a data source.
        /// </summary>
        /// <param name="entities">The objects to create..</param>
        /// <returns>The created objects from a data source.</returns>
        public async Task<IList<TModel>> BulkCreate(IList<TModel> entities)
        {
            return await Lock(async () =>
            {
                // Convert the entities to a list.
                List<TModel> entityList = entities.ToList();
                // Get and check the SQL server connection from the Entity Framework Core DbContext.
                SqlConnection sqlConnection = _dbContext.Database.GetDbConnection() as SqlConnection;
                if (sqlConnection != null)
                {
                    // Create the SqlBulkCopy object using a helper method based on the
                    // SQL Server connection and the Entity Framework Core DbContext.
                    using (SqlBulkCopy bulkCopy = CreateSqlBulkCopy(sqlConnection, _dbContext))
                    {
                        // Get the type of the model class.
                        Type entityType = typeof(TModel);
                        // Get the type of the internal Entity Framework Core model wrapper.
                        IEntityType entityModelType = _dbContext.Model.GetEntityTypes().Single(x => x.ClrType == entityType);
                        // Get the physical SQL Server table name from the Entity Framework Core model.
                        string tableName = entityModelType.GetTableName();
                        // Create a temporary in-memory data-table based on the table name.
                        using (DataTable dataTable = new DataTable(tableName))
                        {
                            // To increase performance, disable all events on the data-table.
                            dataTable.BeginLoadData();
                            // Create a list of delegates to get the values for all properties.
                            List<Func<TModel, object>> propertyGetters = new List<Func<TModel, object>>();
                            // Loop through all properties that are mapped by Entity Framework Core model.
                            foreach (IProperty entityTypeProperty in entityModelType.GetProperties())
                            {
                                // Get the physical SQL column name from the Entity Framework Core model.
                                string columnName = entityTypeProperty.GetColumnName();
                                bulkCopy.ColumnMappings.Add(columnName, columnName);
                                // Get the .NET type of the property from the Entity Framework Core model.
                                Type crlType = entityTypeProperty.ClrType;
                                // By default the property is expected to not be Nullable<T>.
                                bool allowDbNull = false;
                                // Check if the .NET type of the property is a generic type and is Nullable<T>.
                                if (crlType.IsGenericType && crlType.GetGenericTypeDefinition() == typeof(Nullable<>))
                                {
                                    // To get the real .NET type (that is Nullable<T>) we get the only generic argument.
                                    crlType = crlType.GetGenericArguments().Single();
                                    // Mark this property is a Nullable<T> type.
                                    allowDbNull = true;
                                }
                                if (crlType == typeof(string))
                                {
                                    // Mark this property to allow db null.
                                    allowDbNull = true;
                                }
                                // Create the data column based on the column name and the .NET property type.
                                DataColumn dataColumn = dataTable.Columns.Add(columnName, crlType);
                                // Marks as DbNull if the .NET type is Nullable<T>.
                                dataColumn.AllowDBNull = allowDbNull;
                                // Create a generic property value getter delegate for this property.
                                propertyGetters.Add(GetValueGetter(entityType.GetProperty(entityTypeProperty.Name)));
                            }
                            // Convert all entities to the data table using a multi-threaded approach.
                            Parallel.ForEach(entityList, entity =>
                            {
                                // Set a new unique identifier.
                                entity.Id = Guid.NewGuid();
                                // Set InsertedOn/InsertedBy fields.
                                IInsertedOn insertedOnEntity = entity as IInsertedOn;
                                if (insertedOnEntity != null)
                                {
                                    insertedOnEntity.InsertedOn = DateTime.UtcNow;
                                }
                                IInsertedBy insertedByEntity = entity as IInsertedBy;
                                if (insertedByEntity != null)
                                {
                                    insertedByEntity.InsertedBy = _securityContext.PrincipalName;
                                }
                                // Create a data array that will contain all property values.
                                object[] data = new object[propertyGetters.Count];
                                // Loop through all property getters.
                                foreach (Func<TModel, object> propertyGetter in propertyGetters)
                                {
                                    // Get the property value from the entity using the property getter.
                                    object value = propertyGetter(entity);
                                    // Put the property value in the data array.
                                    data[propertyGetters.IndexOf(propertyGetter)] = value;
                                }
                                // Lock the data table to avoid concurrency issues.
                                lock (dataTable)
                                {
                                    // Add the data to the data table as a data row.
                                    dataTable.Rows.Add(data);
                                }
                            });
                            // Finish loading data to the data table and re-enable events.
                            dataTable.EndLoadData();
                            // Check if the SQL Server connection is closed.
                            bool connectionWasClosed = sqlConnection.State != ConnectionState.Open;
                            // If the SQL Server connection is closed, open it.
                            if (connectionWasClosed)
                            {
                                await sqlConnection.OpenAsync();
                            }
                            // Set the SQL table name for the SqlBulkCopy.
                            bulkCopy.DestinationTableName = dataTable.TableName;
                            // Verify the number of rows copied.
                            long numberOfRowsCopied = -1;
                            bulkCopy.NotifyAfter = entities.Count;
                            bulkCopy.SqlRowsCopied += (o, args) =>
                            {
                                numberOfRowsCopied = args.Abort ? -1 : args.RowsCopied;
                            };
                            // Write all data from the data table to the SQL Server database.
                            await bulkCopy.WriteToServerAsync(dataTable);
                            // If the connection was closed before, close it again.
                            if (connectionWasClosed)
                            {
                                sqlConnection.Close();
                            }

                            // Validate the number of rows copied.
                            if (numberOfRowsCopied != entities.Count)
                            {
                                throw new EdreamsDataAccessException(
                                    $"SqlBulkCopy did not copy the expected number of rows: [{numberOfRowsCopied} copied vs. {entities.Count} expected]");
                            }
                        }
                    }
                }
                return entityList;
            });
        }

        private void BulkCopy_SqlRowsCopied(object sender, SqlRowsCopiedEventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region <| Update |>

        /// <summary>
        /// Updates the specified object in a data-source.
        /// </summary>
        /// <param name="entity">The object to update.</param>
        /// <returns>The updated object from a data-source.</returns>
        public async Task<TModel> Update(TModel entity)
        {
            return await Lock(async () =>
            {
                try
                {
                    TModel updated = UpdateInternal(entity);
                    await _dbContext.SaveChangesAsync();
                    await _dbContext.Entry(updated).ReloadAsync();
                    return entity;
                }
                catch (DbUpdateConcurrencyException)
                {
                    string message = "This object has been modified by another user. No changes have been made!";
                    throw new EdreamsConcurrencyException(message);
                }
                catch (DbUpdateException ex)
                {
                    EdreamsDataAccessException exception = CheckException(ex);
                    if (exception == null)
                    {
                        throw;
                    }
                    throw exception;
                }
            });
        }

        /// <summary>
        /// Updates the specified object in a data-source.  Will not override the InsertedBy, InsertedOn, UpdatedBy and UpdatedOn values.
        /// </summary>
        /// <param name="entity">The object to update.</param>
        /// <returns>The updated object from a data-source.</returns>
        public async Task<TModel> UpdateDirect(TModel entity)
        {
            return await Lock(async () =>
            {
                try
                {
                    TModel updated = UpdateInternal(entity, false);
                    await _dbContext.SaveChangesAsync();
                    await _dbContext.Entry(updated).ReloadAsync();
                    return entity;
                }
                catch (DbUpdateConcurrencyException)
                {
                    string message = "This object has been modified by another user. No changes have been made!";
                    throw new EdreamsConcurrencyException(message);
                }
                catch (DbUpdateException ex)
                {
                    EdreamsDataAccessException exception = CheckException(ex);
                    if (exception == null)
                    {
                        throw;
                    }
                    throw exception;
                }
            });
        }

        /// <summary>
        /// Updates the specified objects in a data-source.
        /// </summary>
        /// <param name="entities">The objects to update.</param>
        /// <returns>The updated objects from a data-source.</returns>
        public async Task<IList<TModel>> Update(IList<TModel> entities)
        {
            return await Lock(async () =>
            {
                try
                {
                    List<TModel> entityList = entities.ToList();
                    foreach (TModel entity in entityList)
                    {
                        UpdateInternal(entity);
                    }
                    await _dbContext.SaveChangesAsync();
                    return entityList;
                }
                catch (DbUpdateException ex)
                {
                    EdreamsDataAccessException exception = CheckException(ex);
                    if (exception == null)
                    {
                        throw;
                    }
                    throw exception;
                }
            });
        }

        /// <summary>
        /// Updates the specified objects in a data-source.
        /// Will not override the InsertedBy, InsertedOn, UpdatedBy and UpdatedOn values.
        /// </summary>
        /// <param name="entities">The objects to update.</param>
        /// <returns>The updated objects from a data-source.</returns>
        public async Task<IList<TModel>> UpdateDirect(IList<TModel> entities)
        {
            return await Lock(async () =>
            {
                try
                {
                    List<TModel> entityList = entities.ToList();
                    foreach (TModel entity in entityList)
                    {
                        UpdateInternal(entity, false);
                    }
                    await _dbContext.SaveChangesAsync();
                    return entityList;
                }
                catch (DbUpdateException ex)
                {
                    EdreamsDataAccessException exception = CheckException(ex);
                    if (exception == null)
                    {
                        throw;
                    }
                    throw exception;
                }
            });
        }

        /// <summary>
        /// Updates only the specified property for the specified entities.
        /// </summary>
        /// <typeparam name="TKey">The type of the property to update.</typeparam>
        /// <param name="entities">The entities to update.</param>
        /// <param name="property">The property to update.</param>
        /// <param name="value">The value to update.</param>
        public Task Update<TKey>(IList<TModel> entities, Expression<Func<TModel, TKey>> property, TKey value)
        {
            return Update(entities.Select(x => x.Id).ToList(), property, value);
        }

        /// <summary>
        /// Updates only the specified property for the specified entity ids.
        /// </summary>
        /// <typeparam name="TKey">The type of the property to update.</typeparam>
        /// <param name="ids">The entity ids to update.</param>
        /// <param name="property">The property to update.</param>
        /// <param name="value">The value to update.</param>
        public async Task Update<TKey>(IList<Guid> ids, Expression<Func<TModel, TKey>> property, TKey value)
        {
            IEntityType entityModelType = _dbContext.Model.GetEntityTypes().Single(x => x.ClrType == typeof(TModel));
            string tableName = entityModelType.GetTableName();

            IProperty entityModelProperty = entityModelType.FindProperty(property.GetPropertyName());
            string columnName = entityModelProperty.GetColumnName();

            IProperty idModelProperty = entityModelType.FindProperty(ExpressionExtensions.GetPropertyName<ModelBase, Guid>(x => x.Id));
            string idColumnName = idModelProperty.GetColumnName();
            IProperty updatedByModelProperty = entityModelType.FindProperty(ExpressionExtensions.GetPropertyName<IUpdatedBy, string>(x => x.UpdatedBy));
            string updatedByColumnName = updatedByModelProperty.GetColumnName();
            IProperty updatedOnModelProperty = entityModelType.FindProperty(ExpressionExtensions.GetPropertyName<IUpdatedOn, DateTime?>(x => x.UpdatedOn));
            string updatedOnColumnName = updatedOnModelProperty.GetColumnName();

            string idsString = string.Join(",", ids.Select(x => $"'{x}'"));

            if (_dbContext.Database.GetDbConnection() is SqlConnection sqlConnection)
            {
                // Check if there are current transactions.
                SqlTransaction currentTransaction = _dbContext.Database.CurrentTransaction?.GetDbTransaction() as SqlTransaction;

                bool connectionWasClosed = sqlConnection.State != ConnectionState.Open;
                if (connectionWasClosed)
                {
                    await sqlConnection.OpenAsync();
                }

                StringBuilder queryBuilder = new StringBuilder();
                queryBuilder.Append($"UPDATE {tableName} SET");
                queryBuilder.Append($"  {columnName} = @Value");
                queryBuilder.Append($" ,{updatedByColumnName} = @UpdatedBy");
                queryBuilder.Append($" ,{updatedOnColumnName} = @UpdatedOn");
                queryBuilder.Append($" WHERE {idColumnName} IN ({idsString})");
                SqlCommand sqlCommand = currentTransaction != null ? new SqlCommand(queryBuilder.ToString(), sqlConnection, currentTransaction) : new SqlCommand(queryBuilder.ToString(), sqlConnection);
                sqlCommand.Parameters.AddWithValue("@Value", value);
                sqlCommand.Parameters.AddWithValue("@UpdatedBy", _securityContext.PrincipalName);
                sqlCommand.Parameters.AddWithValue("@UpdatedOn", DateTime.UtcNow);
                await sqlCommand.ExecuteNonQueryAsync();

                if (connectionWasClosed)
                {
                    sqlConnection.Close();
                }
            }
        }

        private TModel UpdateInternal(TModel entity, bool overWrite = true)
        {
            IUpdatedBy updatedByEntity = entity as IUpdatedBy;
            IUpdatedOn updatedOnEntity = entity as IUpdatedOn;
            IInsertedBy insertedByEntity = entity as IInsertedBy;
            IInsertedOn insertedOnEntity = entity as IInsertedOn;

            if (overWrite)
            {
                if (updatedByEntity != null)
                {
                    updatedByEntity.UpdatedBy = _securityContext.PrincipalName;
                }
                if (updatedOnEntity != null)
                {
                    updatedOnEntity.UpdatedOn = DateTime.UtcNow;
                }
                if (insertedByEntity != null)
                {
                    insertedByEntity.InsertedBy = "THIS WILL NOT BE SET";
                }
                if (insertedOnEntity != null)
                {
                    insertedOnEntity.InsertedOn = DateTime.UtcNow;
                }
            }

            TModel localEntityModel = _dbSet.Local.SingleOrDefault(x => x.Id == entity.Id);
            if (localEntityModel == null)
            {
                _dbSet.Attach(entity).State = EntityState.Modified;
            }
            else
            {
                _dbContext.Entry(localEntityModel).CurrentValues.SetValues(entity);
            }

            if (entity is ISysId sysIdEntity)
            {
                _dbContext.Entry((ISysId)(localEntityModel ?? entity)).Property(x => x.SysId).IsModified =
                    false;
            }

            if (entity is ILongSysId longSysIdEntity)
            {
                _dbContext.Entry((ILongSysId)(localEntityModel ?? entity)).Property(x => x.SysId).IsModified =
                    false;
            }

            if (entity is IRowVersion rowVersionEntity)
            {
                PropertyEntry<IRowVersion, byte[]> rowVersionProperty = _dbContext.Entry((IRowVersion)(localEntityModel ?? entity)).Property(x => x.RowVersion);
                rowVersionProperty.OriginalValue = rowVersionProperty.CurrentValue;
            }

            if (!overWrite)
            {
                if (updatedOnEntity != null)
                {
                    _dbContext.Entry((IUpdatedOn)(localEntityModel ?? entity))
                        .Property(x => x.UpdatedOn)
                        .IsModified = false;
                }
                if (updatedByEntity != null)
                {
                    _dbContext.Entry((IUpdatedBy)(localEntityModel ?? entity))
                        .Property(x => x.UpdatedBy)
                        .IsModified = false;
                }
            }

            if (insertedOnEntity != null)
            {
                _dbContext.Entry((IInsertedOn)(localEntityModel ?? entity))
                    .Property(x => x.InsertedOn)
                    .IsModified = false;
            }

            if (insertedByEntity != null)
            {
                _dbContext.Entry((IInsertedBy)(localEntityModel ?? entity))
                    .Property(x => x.InsertedBy)
                    .IsModified = false;
            }

            return localEntityModel ?? entity;
        }

        #endregion

        #region <| Delete |>

        /// <summary>
        /// Deletes the specified object from a data-source.
        /// </summary>
        /// <param name="entity">The object to delete.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public async Task<bool> Delete(TModel entity)
        {
            return await Delete(entity.Id);
        }

        /// <summary>
        /// Deletes the specified objects from a data-source.
        /// </summary>
        /// <param name="entities">The objects to delete.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public async Task<bool> Delete(IList<TModel> entities)
        {
            return await Delete(entities.Select(x => x.Id).ToList());
        }

        /// <summary>
        /// Deletes an object with the specified unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the object to delete.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public async Task<bool> Delete(Guid id)
        {
            return await Lock(async () =>
            {
                try
                {
                    int writtenObjects = await DeleteInternal(id);
                    return writtenObjects > 0;
                }
                catch (DbUpdateConcurrencyException)
                {
                    return false;
                }
                catch (DbUpdateException ex)
                {
                    EdreamsDataAccessException exception = CheckException(ex);
                    if (exception == null)
                    {
                        throw;
                    }
                    throw exception;
                }
            });
        }

        /// <summary>
        /// Deletes multiple objects with the specified unique identifiers.
        /// </summary>
        /// <param name="ids">The unique identifiers of the objects to delete.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public async Task<bool> Delete(IList<Guid> ids)
        {
            return await Lock(async () =>
            {
                try
                {
                    int writtenObjects = 0;
                    foreach (Guid id in ids)
                    {
                        writtenObjects += await DeleteInternal(id);
                    }
                    return writtenObjects > 0;
                }
                catch (DbUpdateConcurrencyException)
                {
                    return false;
                }
                catch (DbUpdateException ex)
                {
                    EdreamsDataAccessException exception = CheckException(ex);
                    if (exception == null)
                    {
                        throw;
                    }
                    throw exception;
                }
            });
        }

        private async Task<int> DeleteInternal(Guid id)
        {
            TModel localEntityModel = _dbSet.Local.SingleOrDefault(x => x.Id == id);
            if (localEntityModel == null)
            {
                localEntityModel = Activator.CreateInstance<TModel>();
                localEntityModel.Id = id;
                _dbSet.Attach(localEntityModel);
            }
            foreach (EntityEntry entry in _dbContext.ChangeTracker.Entries().ToList())
            {
                if (entry.Entity != localEntityModel)
                {
                    entry.State = EntityState.Detached;
                }
            }
            _dbContext.Remove(localEntityModel);
            return await _dbContext.SaveChangesAsync();
        }

        #endregion

        #region <| Truncate |>

        /// <summary>
        /// Deletes all entities from the corresponding database table.
        /// </summary>
        /// <returns>The number of affected rows.</returns>
        public Task Truncate()
        {
            IEntityType entityModelType = _dbContext.Model.GetEntityTypes()
                .Single(x => x.ClrType == typeof(TModel));
            string tableName = entityModelType.GetTableName();

            return RawSql($"TRUNCATE TABLE {tableName}");
        }

        #endregion

        #region <| Raw SQL |>

        /// <summary>
        /// Executes the specified raw SQL query against the database.
        /// </summary>
        /// <param name="sql">The SQL query to execute.</param>
        /// <param name="parameters">Values for parameters used in the raw SQL query.</param>
        /// <returns>The number of affected rows.</returns>
        public Task<int> RawSql(string sql, params object[] parameters)
        {
            return _dbContext.Database.ExecuteSqlRawAsync(sql, parameters);
        }

        public async Task<IList<TModel>> SelectRawSql(string sql, params object[] parameters)
        {
            return await Lock(
                async () => await _dbSet.FromSqlRaw(sql, parameters).AsNoTracking().ToListAsync());
        }

        public async Task<IList<TModel>> SelectRawSql<TProperty>(string sql,
            Expression<Func<TModel, TProperty>> include, params object[] parameters)
        {
            return await Lock(
                async () => await _dbSet.FromSqlRaw(sql, parameters).Include(include).AsNoTracking().ToListAsync());
        }

        #endregion

        #region <| Helper Methods |>

        private EdreamsDataAccessException CheckException(DbUpdateException ex)
        {
            if (ex.InnerException is SqlException sqlException)
            {
                if (_dbContext.Database.GetDbConnection() is SqlConnection sqlConnection)
                {
                    string dataSource = sqlConnection.DataSource;
                    string database = sqlConnection.Database;
                    string table = GetTableNameFromDbContext(_dbContext, sqlException);
                    string column = GetColumnNameFromDbContext(_dbContext, sqlException);
                    // Cannot insert the value NULL into column 'xxx', table 'xxx'
                    // column does not allow nulls. INSERT fails. The statement has been terminated.
                    if (sqlException.Number == 515)
                    {
                        return new EdreamsNotNullConstraintException(dataSource, database, table, column, sqlException);
                    }
                    // The DELETE statement conflicted with the REFERENCE constraint "xxx".
                    // The conflict occurred in database "xxx", table "xxx", column "xxx".
                    // The statement has been terminated.
                    if (sqlException.Number == 547)
                    {
                        return new EdreamsForeignKeyConstraintException(dataSource, database, table, column, sqlException);
                    }
                    // The CREATE or UPDATE statement conflicted with the UNIQUE constraint "xxx".
                    // The conflict occurred in database "xxx", table "xxx", column "xxx".
                    // The statement has been terminated.
                    if (sqlException.Number == 2627 || sqlException.Number == 2601)
                    {
                        return new EdreamsUniqueConstraintException(dataSource, database, table, column, sqlException);
                    }
                    return new EdreamsDataAccessException(sqlException);
                }
            }
            return null;
        }

        private static string GetTableNameFromDbContext(DbContext dbContext, SqlException sqlException)
        {
            foreach (IEntityType entityType in dbContext.Model.GetEntityTypes())
            {
                if (sqlException.Message.Contains(entityType.GetTableName()))
                {
                    return entityType.GetTableName();
                }
            }
            return null;
        }

        private static string GetColumnNameFromDbContext(DbContext dbContext, SqlException sqlException)
        {
            string column = "";
            foreach (IEntityType entityType in dbContext.Model.GetEntityTypes())
            {
                if (sqlException.Message.Contains(entityType.GetTableName()))
                {
                    foreach (IProperty property in entityType.GetProperties())
                    {
                        if (sqlException.Message.Contains(property.GetColumnName()))
                        {
                            column = property.GetColumnName().Length > column.Length ? property.GetColumnName() : column;
                        }
                    }
                }
            }
            return column;
        }

        /// <summary>
        /// Creates the SqlBulkCopy object from a SQL Server connection and Entity Framework Core DbContext.
        /// </summary>
        /// <param name="sqlConnection">The SQL Server connection.</param>
        /// <param name="dbContext">The Entity Framework Core DbContext.</param>
        /// <returns>The requested SqlBulkCopy object.</returns>
        private static SqlBulkCopy CreateSqlBulkCopy(SqlConnection sqlConnection, DbContext dbContext)
        {
            // Check if there are current transactions.
            SqlTransaction currentTransaction = dbContext.Database.CurrentTransaction?.GetDbTransaction() as SqlTransaction;
            // Return a SqlBulkCopy object using the current transaction, if it exists.
            return currentTransaction == null
                ? new SqlBulkCopy(sqlConnection)
                : new SqlBulkCopy(sqlConnection, SqlBulkCopyOptions.Default, currentTransaction);
        }

        /// <summary>
        /// Gets a getter func for a given PropertyInfo.
        /// </summary>
        /// <param name="propertyInfo">A PropertyInfo to get a getter func for.</param>
        /// <returns>The requested getter func.</returns>
        private static Func<TModel, object> GetValueGetter(PropertyInfo propertyInfo)
        {
            // To improve performance of getting a lot of values
            // using reflection we use compiled C# expressions.
            if (propertyInfo == null || propertyInfo.DeclaringType == null) { throw new EdreamsException(); }
            ParameterExpression instance = Expression.Parameter(propertyInfo.DeclaringType, "i");
            MemberExpression property = Expression.Property(instance, propertyInfo);
            UnaryExpression convert = Expression.TypeAs(property, typeof(object));
            return (Func<TModel, object>)Expression.Lambda(convert, instance).Compile();
        }

        private async Task<TReturn> Lock<TReturn>(Func<Task<TReturn>> action)
        {
            await _semaphore.WaitAsync();
            try
            {
                return await action();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        #endregion

        #region <| IDisposable Implementation |>

        /// <summary>
        /// Performs application-defined tasks associated with freeing, 
        /// releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _semaphore?.Dispose();
        }

        #endregion
    }
}