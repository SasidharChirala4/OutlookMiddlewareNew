using Edreams.OutlookMiddleware.Api.Helpers;
using Edreams.OutlookMiddleware.BusinessLogic.Interfaces;
using Edreams.OutlookMiddleware.DataTransferObjects;
using Edreams.OutlookMiddleware.DataTransferObjects.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
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
        /// Gets the pending email categories for the specified user.
        /// </summary>
        /// <param name="userPrincipalName">The UserPricipalName for whom the pending categories needs to be fetched.</param>
        /// <returns>An ApiResult containing the pending categories.</returns>
        /// <remarks>This HTTP GET operation lists all pending categories for the specified mail address.</remarks>
        [HttpGet]
        [SwaggerResponse(200, "Successfully returns a GetPendingCategories object.", typeof(GetPendingCategoriesResponse))]
        [SwaggerResponse(500, "An internal server error has occurred. This is not your fault.", typeof(ApiErrorResult))]
        public Task<IActionResult> GetPendingCategories(string userPrincipalName)
        {
            return ExecuteManager(manager => manager.GetPendingCategories(userPrincipalName));
        }

        /// <summary>
        /// Method to update the pending categories for the specified user.
        /// </summary>
        /// <param name="updatePendingCategoriesRequest">Request of UpdatePendingCategories</param>
        /// <returns></returns>
        /// <remarks>This HTTP POST operation updates the pending categories for the specified user.</remarks>
        [HttpPost]
        [SwaggerResponse(200, "Successfully categories have been processed.", typeof(UpdatePendingCategoriesResponse))]
        [SwaggerResponse(404, "Specified categories could not be found.", typeof(ApiResult))]
        [SwaggerResponse(500, "An internal server error has occurred. This is not your fault.", typeof(ApiErrorResult))]
        public async Task<IActionResult> UpdatePendingCategories([FromBody] UpdatePendingCategoriesRequest updatePendingCategoriesRequest)
        {
            return await ExecuteManager(manager => manager.UpdatePendingCategories(updatePendingCategoriesRequest));
        }
    }
}