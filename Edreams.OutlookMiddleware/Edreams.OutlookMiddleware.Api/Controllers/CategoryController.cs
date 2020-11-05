using Edreams.OutlookMiddleware.Api.Helpers;
using Edreams.OutlookMiddleware.BusinessLogic.Interfaces;
using Edreams.OutlookMiddleware.Common.Configuration.Interfaces;
using Edreams.OutlookMiddleware.DataTransferObjects;
using Edreams.OutlookMiddleware.DataTransferObjects.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Edreams.OutlookMiddleware.Api.Controllers
{
    /// <summary>
    /// HTTP API Controller containing all mail category related operations.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class CategoryController : ApiController<ICategoryManager>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryController" /> class.
        /// </summary>
        /// <param name="categoryManager">T</param>
        /// <param name="logger"></param>
        public CategoryController(ILogger<CategoryController> logger, ICategoryManager categoryManager)
           : base(categoryManager, logger) { }

        /// <summary>
        /// Gets the pending email categories for the specified mail address.
        /// </summary>
        /// <param name="mailAddress">The mail address to get the pending categories for.</param>
        /// <returns>An ApiResult containing the pending categories.</returns>
        /// <remarks>This HTTP GET operation lists all pending categories for the specified mail address.</remarks>
        [HttpGet]
        [SwaggerResponse(200, Type = typeof(ApiResult), Description = "Successfully returns a GetPendingCategories object.")]
        public Task<IActionResult> GetPendingCategories(string mailAddress)
        {
            return ExecuteManager(manager => manager.GetPendingCategories(mailAddress));
        }

        /// <summary>
        /// Method to set the processed categories for a given user.
        /// </summary>
        /// <param name="categories">The list of categories to be processed.</param>
        /// <param name="user">The UserName for whom the categories should be processed.</param>
        /// <returns></returns>
        [HttpPost]
        [SwaggerResponse(200, Type = typeof(ApiResult), Description = "Successfully categories have been processed ")]
        public async Task<IActionResult> SetCategories([FromBody] List<ProcessedCategoriesRequest> categories, string user)
        {
            return await ExecuteManager(manager => manager.SetProcessedCategories(categories, user));
        }
    }
}
