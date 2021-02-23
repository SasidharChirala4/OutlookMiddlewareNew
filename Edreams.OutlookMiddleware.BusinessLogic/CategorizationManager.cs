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
using EmailEntity = Edreams.OutlookMiddleware.Model.Email;
using Edreams.OutlookMiddleware.Common.Validation.Interface;
using Edreams.OutlookMiddleware.Common.Constants;
using Edreams.OutlookMiddleware.Model;
using Edreams.OutlookMiddleware.Enums;
using Edreams.OutlookMiddleware.DataAccess.Exceptions;

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
        private readonly ILogger _logger;
        private readonly IValidator _validator;

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
        IEdreamsConfiguration configuration, ILogger logger, IRepository<EmailEntity> emailRepository, IValidator validator)
        {
            _categorizationRequestRepository = categorizationRequestsRepository;
            _categorizationRequestMapper = categorizationRequestMapper;
            _configuration = configuration;
            _logger = logger;
            _validator = validator;
            _emailRepository = emailRepository;
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
                                                                        x.EmailAddress.Equals(userPrincipalName) && x.Status == CategorizationRequestStatus.Pending, order => order.SysId, limit);
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
                 internetMessageIds.Contains(x.InternetMessageId) && x.EmailAddress.Equals(updatePendingCategoriesRequest.UserPrincipalName) && x.Status == CategorizationRequestStatus.Pending);

                foreach (CategorizationRequestEntity categorizationRequest in categorizationRequests)
                {
                    categorizationRequest.Status = CategorizationRequestStatus.Processed;
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

        /// <summary>
        /// Method to add a category.
        /// </summary>
        /// <param name="internetMessageId">The internet message Id of the Email.</param>
        /// <param name="recipientsList">The list of recipients.</param>
        /// <param name="isUploaded">Flag to set Uploaded/Failed Categorization.</param>
        /// <returns></returns>
        public async Task AddCategorizationRequest(string internetMessageId, List<string> recipientsList, bool isUploaded)
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
                        await AddCategorizationByIndividualRecipient(internetMessageId, recipient, isUploaded);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("something went wrong while adding categorization.", ex);
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
        private async Task AddCategorizationByIndividualRecipient(string internetMessageId, string recipient, bool isUploaded)
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

                        // TODO : Need to check with Johnny/Sasi about how & where to set all CategorizationRequestType
                        // setting catergorization type based on isUploaded value
                        /* isUploaded is True -> Email/Attchament setting as Uploaded
                         * isUploaded is False -> Email/Attchament setting as Failed */

                        Type = isUploaded
                           ? CategorizationRequestType.EmailUploaded
                           : CategorizationRequestType.EmailUploadFailed
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
                        _logger.LogError("Error at adding categorization after exceeding 3 times.", ex);
                        retry = false;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("Unexpected error occured while adding catergorization.", ex);
                    retry = false;
                }
            } while (retry);
        }
    }
    #endregion

}
