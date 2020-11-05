using Edreams.OutlookMiddleware.BusinessLogic.Interfaces;
using Edreams.OutlookMiddleware.DataAccess.Repositories.Helpers;
using Edreams.OutlookMiddleware.DataAccess.Repositories.Interfaces;
using Edreams.OutlookMiddleware.DataTransferObjects.Api;
using CategorizationModel = Edreams.OutlookMiddleware.Model.CategorizationRequest;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Edreams.OutlookMiddleware.Common.Configuration.Interfaces;

namespace Edreams.OutlookMiddleware.BusinessLogic
{
    /// <summary>
    /// CategoryManager defining methods to related to Categories.
    /// </summary>
    public class CategoryManager : ICategoryManager
    {
        private readonly IEdreamsConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly IRepository<CategorizationModel> _categorizationRequestRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryManager" /> class.
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="configurationManager"></param>
        /// <param name="logger"></param>
        public CategoryManager(IRepository<CategorizationModel> categorizationRequestsRepository,
            IEdreamsConfiguration configuration, ILogger logger)
        {
            _categorizationRequestRepository = categorizationRequestsRepository;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Method to get the pending categories for a given user.
        /// </summary>
        /// <param name="user">The UserName for whom the pending categories needs to be fetched.</param>
        /// <returns></returns>
        public async Task<GetCategoryResponse> GetPendingCategories(string user)
        {
            try
            {
                int maxNumberEmails = Convert.ToInt32(_configuration.MaxNumberPendingCategories);
                Limit limit = new Limit(0, maxNumberEmails);

                List<CategorizationRequest> categorizations = (List<CategorizationRequest>)await _categorizationRequestRepository.FindDescending(x => x.UserPrincipalName.Equals(user) && !x.Sent,
                    order => order.SysId, limit);
               
                return new GetCategoryResponse
                {
                    CategorizationRequests =
                        categorizations.Select(x => new CategorizationRequest
                        {
                            InternetMessageId = x.InternetMessageId,
                            IsCompose = x.IsCompose,
                            CategorizationRequestType = x.CategorizationRequestType
                        }).ToList()
                };
            }
            catch (Exception ex)
            {
                // TODO : Need to Implement custom logger -  after Completing Task # 41044   
                _logger.LogError(ex.Message);
            }

            return new GetCategoryResponse();
        }
        /// <summary>
        /// Method to set the processed categories for a given user.
        /// </summary>
        /// <param name="categories">The list of categories to be processed.</param>
        /// <param name="user">The UserName for whom the categories should be processed.</param>
        /// <returns></returns>
        public async Task<ProcessedCategoriesResponse> SetProcessedCategories(List<ProcessedCategoriesRequest> categories, string user)
        {
            try
            {
                IEnumerable<string> internetMessageIds = categories.Select(y => y.InternetMessageId).Distinct();

                List<CategorizationModel> categorizationRequests = (List<CategorizationModel>)await _categorizationRequestRepository.Find(x =>
                    internetMessageIds.Contains(x.InternetMessageId) && x.UserPrincipalName.Equals(user) && !x.Sent);

                categorizationRequests.ForEach(x => x.Sent = true);

                await _categorizationRequestRepository.Create(categorizationRequests);

                return new ProcessedCategoriesResponse { Success = true };
            }
            catch (Exception ex)
            {
                // TODO : Need to Implement custom logger -  after Completing Task # 41044 
                _logger.LogError(ex.Message);
            }
            return new ProcessedCategoriesResponse { Success = false };
        }
    }
}
