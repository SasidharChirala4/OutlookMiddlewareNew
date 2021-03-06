using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Edreams.Common.DataAccess;
using Edreams.Common.DataAccess.Interfaces;
using Edreams.Common.Exceptions;
using Edreams.Common.Logging.Interfaces;
using Edreams.Common.Security.Interfaces;
using Edreams.OutlookMiddleware.BusinessLogic.Interfaces;
using Edreams.OutlookMiddleware.Common.Configuration.Interfaces;
using Edreams.OutlookMiddleware.Common.Constants;
using Edreams.OutlookMiddleware.Common.Validation.Interface;
using Edreams.OutlookMiddleware.DataTransferObjects.Api;
using Edreams.OutlookMiddleware.Enums;
using Edreams.OutlookMiddleware.Mapping.Interfaces;
using Edreams.OutlookMiddleware.Model;
using Microsoft.EntityFrameworkCore;
using CategorizationRequestContract = Edreams.OutlookMiddleware.DataTransferObjects.Api.CategorizationRequest;
using CategorizationRequestEntity = Edreams.OutlookMiddleware.Model.CategorizationRequest;
using EmailEntity = Edreams.OutlookMiddleware.Model.Email;

namespace Edreams.OutlookMiddleware.BusinessLogic
{
    /// <summary>
    /// CategorizationManager defining methods to related to Categories.
    /// </summary>
    public class CategorizationManager : ICategorizationManager
    {
        #region <| Private Members |>

        private readonly IRepository<CategorizationRequestEntity> _categorizationRequestRepository;
        private readonly IRepository<EmailEntity> _emailRepository;
        private readonly IMapper<CategorizationRequestEntity, CategorizationRequestContract> _categorizationRequestMapper;
        private readonly IEdreamsConfiguration _configuration;
        private readonly IEdreamsLogger<CategorizationManager> _logger;
        private readonly IValidator _validator;
        private readonly ISecurityContext _securityContext;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="CategorizationManager" /> class.
        /// </summary>
        /// <param name="categorizationRequestsRepository"></param>
        /// <param name="categorizationRequestMapper"></param>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <param name="emailRepository"></param>
        /// <param name="validator"></param>
        public CategorizationManager(IRepository<CategorizationRequestEntity> categorizationRequestsRepository,
            IMapper<CategorizationRequestEntity, CategorizationRequestContract> categorizationRequestMapper,
            IEdreamsConfiguration configuration, IEdreamsLogger<CategorizationManager> logger,
            IRepository<EmailEntity> emailRepository, IValidator validator, ISecurityContext securityContext)
        {
            _categorizationRequestRepository = categorizationRequestsRepository;
            _categorizationRequestMapper = categorizationRequestMapper;
            _configuration = configuration;
            _logger = logger;
            _emailRepository = emailRepository;
            _validator = validator;
            _securityContext = securityContext;
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
                GetPendingCategoriesResponse response = new GetPendingCategoriesResponse
                {
                    CorrelationId = _securityContext.CorrelationId
                };

                Limit limit = new Limit(0, _configuration.MaxNumberPendingCategories);

                IList<CategorizationRequestEntity> categorizations = await _categorizationRequestRepository.FindDescending(x =>
                                                                        x.EmailAddress.Equals(userPrincipalName) && x.Status == CategorizationRequestStatus.Pending, order => order.SysId, limit);
                if (categorizations.Count > 0)
                {
                    response.CategorizationRequests = _categorizationRequestMapper.Map(categorizations).ToList();
                }

                return response;
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
                 internetMessageIds.Contains(x.InternetMessageId) && x.EmailAddress.Equals(updatePendingCategoriesRequest.UserPrincipalName) && x.Status == CategorizationRequestStatus.Pending);

                foreach (CategorizationRequestEntity categorizationRequest in categorizationRequests)
                {
                    categorizationRequest.Status = CategorizationRequestStatus.Processed;
                }

                await _categorizationRequestRepository.Update(categorizationRequests);

                return new UpdatePendingCategoriesResponse
                {
                    CorrelationId = _securityContext.CorrelationId,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at update pending categories");
                throw;
            }
        }

        /// <summary>
        /// Method to add a category.
        /// </summary>
        /// <param name="internetMessageId">The internet message Id of the Email.</param>
        /// <param name="recipientsList">The list of recipients.</param>
        /// <param name="type">The type of the CategorizationRequest.</param>
        /// <returns></returns>
        public async Task AddCategorizationRequest(string internetMessageId, List<string> recipientsList, CategorizationRequestType type)
        {
            // Validations
            _validator.ValidateString(internetMessageId, ValidationMessages.WebApi.InternetMessageIdRequired);
            _validator.ValidateList(recipientsList, ValidationMessages.WebApi.RecipientsListRequired);

            try
            {
                // Get latest email by internetMessageId 
                Email mail = await _emailRepository.GetFirstDescending(x => x.InternetMessageId == internetMessageId, x => x.Id);
                if (mail != null)
                {
                    foreach (string recipient in recipientsList)
                    {
                        // Adding Categorise by individual recipent.
                        await AddCategorizationByIndividualRecipient(internetMessageId, recipient, type);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "something went wrong while adding categorization.");
            }
        }

        #region Private Methods

        /// <summary>
        ///  Adding Catergorizations by individual recipient
        /// </summary>
        /// <param name="internetMessageId"></param>
        /// <param name="recipient"></param>
        /// <param name="isUploaded"></param>
        /// <returns></returns>
        private async Task AddCategorizationByIndividualRecipient(string internetMessageId, string recipient, CategorizationRequestType type)
        {
            int errors = 0;
            bool retry = false;
            do
            {
                try
                {
                    var categorization = new CategorizationRequestEntity()
                    {
                        EmailAddress = recipient,
                        InternetMessageId = internetMessageId,
                        Status = CategorizationRequestStatus.Pending,
                        Type = type
                    };
                    await _categorizationRequestRepository.Create(categorization);
                }
                catch (EdreamsDataAccessException ex)
                {
                    errors++;

                    // Delay for half a second to help recover from SQL deadlock.
                    await Task.Delay(500);

                    if (errors < 3)
                    {
                        retry = true;
                    }
                    else
                    {
                        //TODO : Need to check with Johnny/Sasi about proper logs
                        _logger.LogError(ex, "Error at adding categorization after exceeding 3 times.");
                        retry = false;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error occured while adding catergorization.");
                    retry = false;
                }
            } while (retry);
        }
    }

    #endregion
}