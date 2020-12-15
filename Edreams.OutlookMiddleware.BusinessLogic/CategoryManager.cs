using Edreams.OutlookMiddleware.BusinessLogic.Interfaces;
using Edreams.OutlookMiddleware.DataAccess.Repositories.Helpers;
using Edreams.OutlookMiddleware.DataAccess.Repositories.Interfaces;
using Edreams.OutlookMiddleware.DataTransferObjects.Api;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Edreams.OutlookMiddleware.Common.Configuration.Interfaces;
using Edreams.OutlookMiddleware.Mapping.Interfaces;
using CategorizationRequestEntity = Edreams.OutlookMiddleware.Model.CategorizationRequest;
using CategorizationRequestContract = Edreams.OutlookMiddleware.DataTransferObjects.Api.CategorizationRequest;

namespace Edreams.OutlookMiddleware.BusinessLogic
{
    /// <summary>
    /// CategoryManager defining methods to related to Categories.
    /// </summary>
    public class CategoryManager : ICategoryManager
    {
        #region <| Private Members |>

        private readonly IRepository<CategorizationRequestEntity> _categorizationRequestRepository;
        private readonly IMapper<CategorizationRequestEntity, CategorizationRequestContract> _categorizationRequestMapper;
        private readonly IEdreamsConfiguration _configuration;
        private readonly ILogger _logger;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryManager" /> class.
        /// </summary>        
        /// <param name="configurationManager"></param>
        /// <param name="logger"></param>
        public CategoryManager(IRepository<CategorizationRequestEntity> categorizationRequestsRepository,
            IMapper<CategorizationRequestEntity, CategorizationRequestContract> categorizationRequestMapper,
        IEdreamsConfiguration configuration, ILogger logger)
        {
            _categorizationRequestRepository = categorizationRequestsRepository;
            _categorizationRequestMapper = categorizationRequestMapper;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Method to get the pending categories for the specified user.
        /// </summary>
        /// <param name="userPrincipalName">The UserPricipalName for whom the pending categories needs to be fetched.</param>
        /// <returns></returns>
        public async Task<GetPendingCategoriesResponse> GetPendingCategories(string userPrincipalName)
        {
            try
            {
                GetPendingCategoriesResponse getPendingCategoriesResponse = new GetPendingCategoriesResponse();
                Limit limit = new Limit(0, _configuration.MaxNumberPendingCategories);

                IList<CategorizationRequestEntity> categorizations = await _categorizationRequestRepository.FindDescending(x =>
                                                                        x.UserPrincipalName.Equals(userPrincipalName) && !x.Sent, order => order.SysId, limit);
                if (categorizations.Count > 0)
                {
                    getPendingCategoriesResponse.CategorizationRequests = _categorizationRequestMapper.Map(categorizations).ToList();
                }
                return getPendingCategoriesResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at getting pending categories");
                throw;
            }
        }

        /// <summary>
        /// Method to update the pending categories for the specified user.
        /// </summary>
        /// <param name="updatePendingCategoriesRequest"></param>
        /// <returns></returns>
        public async Task<UpdatePendingCategoriesResponse> UpdatePendingCategories(UpdatePendingCategoriesRequest updatePendingCategoriesRequest)
        {
            try
            {
                IEnumerable<string> internetMessageIds = updatePendingCategoriesRequest.CategorizationRequests.Select(y => y.InternetMessageId).Distinct();

                IList<CategorizationRequestEntity> categorizationRequests = await _categorizationRequestRepository.Find(x =>
                 internetMessageIds.Contains(x.InternetMessageId) && x.UserPrincipalName.Equals(updatePendingCategoriesRequest.UserPrincipalName) && !x.Sent);

                foreach (CategorizationRequestEntity categorizationRequest in categorizationRequests)
                {
                    categorizationRequest.Sent = true;
                }

                await _categorizationRequestRepository.Update(categorizationRequests);

                return new UpdatePendingCategoriesResponse { Success = true };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at update pending categories");
                throw;
            }
        }
    }
}
