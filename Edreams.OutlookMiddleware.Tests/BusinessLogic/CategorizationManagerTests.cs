using Edreams.OutlookMiddleware.DataAccess.Repositories.Interfaces;
using CategorizationRequestEntity = Edreams.OutlookMiddleware.Model.CategorizationRequest;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Edreams.OutlookMiddleware.Common.Configuration.Interfaces;
using Microsoft.Extensions.Logging;
using Edreams.OutlookMiddleware.Model;
using Edreams.OutlookMiddleware.BusinessLogic.Interfaces;
using Edreams.OutlookMiddleware.BusinessLogic;
using FluentAssertions;
using Edreams.OutlookMiddleware.DataAccess.Repositories.Helpers;
using Edreams.OutlookMiddleware.Tests.Framework.Extensions;
using Edreams.OutlookMiddleware.Mapping;
using Edreams.OutlookMiddleware.DataTransferObjects.Api;
using CategorizationRequest = Edreams.OutlookMiddleware.DataTransferObjects.Api.CategorizationRequest;
using Edreams.OutlookMiddleware.Common.Exceptions;
using Edreams.OutlookMiddleware.Common.Constants;
using Edreams.OutlookMiddleware.Common.Validation;

namespace Edreams.OutlookMiddleware.Tests.BusinessLogic
{
    public class CategorizationManagerTests
    {
        #region <| GetPendingCategories |>

        [Fact]
        public async Task CategorizationManager_GetPendingCategories__Without_PendingCategories_Should_Return_Null()
        {
            #region [ ARRANGE ]

            // Create mocks for all dependencies
            var categorizationRequestsRepositoryMock = new Mock<IRepository<CategorizationRequestEntity>>();
            var categorizationRequestMapper = new CategorizationRequestMapper();
            var emailRepositoryMock = new Mock<IRepository<Email>>();
            var validator = new Validator();
            var edreamsConfigurationMock = new Mock<IEdreamsConfiguration>();
            var loggerMock = new Mock<ILogger>();

            // Create an instance of the "CategorizationManager" using the mocked dependencies.
            ICategorizationManager categorizationManager = new CategorizationManager(
                categorizationRequestsRepositoryMock.Object, categorizationRequestMapper, edreamsConfigurationMock.Object, loggerMock.Object, emailRepositoryMock.Object, validator);

            // Prepare a request to use for when calling the "CommitBatch" method.
            CategorizationRequestEntity categorizationRequest = new CategorizationRequestEntity()
            {
                EmailAddress = "edreamstest@deloitte.com",
                Id = new Guid("2D1EE7EC-6E7D-46DA-B2DD-9EC6F0C66653"),
                InternetMessageId = "<b267a11ab00348e3aa77bd20e20e1060>",
                Status = Enums.CategorizationRequestStatus.Expired
            };
            string userPrincipalName = "edreamstest@deloitte.com";
            Limit limit = new Limit(0, 20);
            IList<CategorizationRequestEntity> categorizationRequestList = new List<CategorizationRequestEntity>() { categorizationRequest };

            #endregion

            #region [ MOCK ]
            // Mock the "FindDescending" method on the "Repository" and run the predicate lambda expression on
            // an  list of "categorizationResult" objects.
            categorizationRequestsRepositoryMock.SetupRepositoryFindDescending<CategorizationRequestEntity, long>(categorizationRequestList);

            #endregion

            #region [ ACT ]

            // Call the "GetPendingCategories" method using the prepared request.
            DataTransferObjects.Api.GetPendingCategoriesResponse response = await categorizationManager.GetPendingCategories(userPrincipalName);

            #endregion

            #region [ ASSERT ]

            // If there are no "PendingCategories" objects found, the "GetPendingCategories" method should return Null.
            response.CategorizationRequests.Should().BeNull();

            #endregion
        }

        [Fact]
        public async Task CategorizationManager_GetPendingCategories__Should_Return_PendingCategories()
        {
            #region [ ARRANGE ]

            // Create mocks for all dependencies
            var categorizationRequestsRepositoryMock = new Mock<IRepository<CategorizationRequestEntity>>();
            var categorizationRequestMapper = new CategorizationRequestMapper();
            var emailRepositoryMock = new Mock<IRepository<Email>>();
            var validator = new Validator();
            var edreamsConfigurationMock = new Mock<IEdreamsConfiguration>();
            var loggerMock = new Mock<ILogger>();

            // Create an instance of the "CategorizationManager" using the mocked dependencies.
            ICategorizationManager categorizationManager = new CategorizationManager(
                categorizationRequestsRepositoryMock.Object, categorizationRequestMapper, edreamsConfigurationMock.Object, loggerMock.Object, emailRepositoryMock.Object, validator);

            // Prepare a request to use for when calling the "CommitBatch" method.
            CategorizationRequestEntity categorizationRequest = new CategorizationRequestEntity()
            {
                EmailAddress = "edreamstest@deloitte.com",
                Id = new Guid("2D1EE7EC-6E7D-46DA-B2DD-9EC6F0C66653"),
                InternetMessageId = "<b267a11ab00348e3aa77bd20e20e1060>",
                Status = Enums.CategorizationRequestStatus.Pending
            };
            string userPrincipalName = "edreamstest@deloitte.com";
            Limit limit = new Limit(0, 20);
            IList<CategorizationRequestEntity> categorizationRequestList = new List<CategorizationRequestEntity>() { categorizationRequest };

            #endregion

            #region [ MOCK ]
            // Mock the "FindDescending" method on the "Repository" and run the predicate lambda expression on
            // an  list of "categorizationResult" objects.
            categorizationRequestsRepositoryMock.SetupRepositoryFindDescending<CategorizationRequestEntity, long>(categorizationRequestList);

            #endregion

            #region [ ACT ]

            // Call the "GetPendingCategories" method using the prepared request.
            DataTransferObjects.Api.GetPendingCategoriesResponse response = await categorizationManager.GetPendingCategories(userPrincipalName);

            #endregion

            #region [ ASSERT ]

            // If there are  "PendingCategories" objects found, the "GetPendingCategories" method should return PendingCategories.
            response.Should().NotBeNull();

            #endregion
        }
        #endregion

        #region <| UpdatePendingCategories |>

        [Fact]
        public async Task CategorizationManager_UpdatePendingCategories__Without_PendingCategories_Should_Return_Null()
        {
            #region [ ARRANGE ]

            // Create mocks for all dependencies
            var categorizationRequestsRepositoryMock = new Mock<IRepository<CategorizationRequestEntity>>();
            var categorizationRequestMapper = new CategorizationRequestMapper();
            var emailRepositoryMock = new Mock<IRepository<Email>>();
            var validator = new Validator();
            var edreamsConfigurationMock = new Mock<IEdreamsConfiguration>();
            var loggerMock = new Mock<ILogger>();

            // Create an instance of the "CategorizationManager" using the mocked dependencies.
            ICategorizationManager categorizationManager = new CategorizationManager(
                categorizationRequestsRepositoryMock.Object, categorizationRequestMapper, edreamsConfigurationMock.Object, loggerMock.Object, emailRepositoryMock.Object, validator);

            // Prepare a request to use for when calling the "CommitBatch" method.
            CategorizationRequestEntity categorizationRequestEntity = new CategorizationRequestEntity()
            {
                EmailAddress = "edreamstest@deloitte.com",
                Id = new Guid("2D1EE7EC-6E7D-46DA-B2DD-9EC6F0C66653"),
                InternetMessageId = "<b267a11ab00348e3aa77bd20e20e1060>",
                Status = Enums.CategorizationRequestStatus.Expired
            };
            List<CategorizationRequestEntity> categorizationRequestEntityList = new List<CategorizationRequestEntity>() { categorizationRequestEntity };
            CategorizationRequest categorizationRequest = new CategorizationRequest()
            {
                InternetMessageId = "<b267a11ab00348e3aa77bd20e20e1060>",
            };
            List<CategorizationRequest> categorizationRequestList = new List<CategorizationRequest>() { categorizationRequest };
            UpdatePendingCategoriesRequest updatePendingCategoriesRequest = new UpdatePendingCategoriesRequest()
            {
                CategorizationRequests = categorizationRequestList
            };
            #endregion

            #region [ MOCK ]
            // Mock the "FindDescending" method on the "Repository" and run the predicate lambda expression on
            // an  list of "categorizationResult" objects.
            categorizationRequestsRepositoryMock.SetupRepositoryFind(categorizationRequestEntityList);


            #endregion

            #region [ ACT ]

            // Call the "UpdatePendingCategoriesResponse" method using the prepared request.
            UpdatePendingCategoriesResponse response = await categorizationManager.UpdatePendingCategories(updatePendingCategoriesRequest);

            #endregion

            #region [ ASSERT ]
            // If there are no "PendingCategories" objects found, the "GetPendingCategories" method should return Null.
            response.Success.Should().Be(true);

            #endregion
        }

        [Fact]
        public async Task CategorizationManager_UpdatePendingCategories__Should_Update_PendingCategories()
        {
            #region [ ARRANGE ]

            // Create mocks for all dependencies
            var categorizationRequestsRepositoryMock = new Mock<IRepository<CategorizationRequestEntity>>();
            var categorizationRequestMapper = new CategorizationRequestMapper();
            var emailRepositoryMock = new Mock<IRepository<Email>>();
            var validator = new Validator();
            var edreamsConfigurationMock = new Mock<IEdreamsConfiguration>();
            var loggerMock = new Mock<ILogger>();

            // Create an instance of the "CategorizationManager" using the mocked dependencies.
            ICategorizationManager categorizationManager = new CategorizationManager(
                categorizationRequestsRepositoryMock.Object, categorizationRequestMapper, edreamsConfigurationMock.Object, loggerMock.Object, emailRepositoryMock.Object, validator);

            // Prepare a request to use for when calling the "CommitBatch" method.
            CategorizationRequestEntity categorizationRequestEntity = new CategorizationRequestEntity()
            {
                EmailAddress = "edreamstest@deloitte.com",
                Id = new Guid("2D1EE7EC-6E7D-46DA-B2DD-9EC6F0C66653"),
                InternetMessageId = "<b267a11ab00348e3aa77bd20e20e1060>",
                Status = Enums.CategorizationRequestStatus.Pending
            };
            List<CategorizationRequestEntity> categorizationRequestEntityList = new List<CategorizationRequestEntity>() { categorizationRequestEntity };
            CategorizationRequest categorizationRequest = new CategorizationRequest()
            {
                InternetMessageId = "<b267a11ab00348e3aa77bd20e20e1060>",
            };
            List<CategorizationRequest> categorizationRequestList = new List<CategorizationRequest>() { categorizationRequest };
            UpdatePendingCategoriesRequest updatePendingCategoriesRequest = new UpdatePendingCategoriesRequest()
            {
                CategorizationRequests = categorizationRequestList
            };
            #endregion

            #region [ MOCK ]
            // Mock the "FindDescending" method on the "Repository" and run the predicate lambda expression on
            // an  list of "categorizationResult" objects.
            categorizationRequestsRepositoryMock.SetupRepositoryFind(categorizationRequestEntityList);


            #endregion

            #region [ ACT ]

            // Call the "UpdatePendingCategoriesResponse" method using the prepared request.
            UpdatePendingCategoriesResponse response = await categorizationManager.UpdatePendingCategories(updatePendingCategoriesRequest);

            #endregion

            #region [ ASSERT ]

            // If there are no "PendingCategories" objects found, the "GetPendingCategories" method should return Null.
            categorizationRequestsRepositoryMock.VerifyRepositoryUpdate(Times.Once());

            #endregion
        }
        #endregion

        #region <| AddCategorizationRequest |>

        [Fact]
        public async Task CategorizationManager_AddCategorizationRequest__Without_InternetMessageId_Should_Throw_Validattion()
        {
            #region [ ARRANGE ]

            // Create mocks for all dependencies
            var categorizationRequestsRepositoryMock = new Mock<IRepository<CategorizationRequestEntity>>();
            var categorizationRequestMapper = new CategorizationRequestMapper();
            var emailRepositoryMock = new Mock<IRepository<Email>>();
            var validator = new Validator();
            var edreamsConfigurationMock = new Mock<IEdreamsConfiguration>();
            var loggerMock = new Mock<ILogger>();

            // Create an instance of the "CategorizationManager" using the mocked dependencies.
            ICategorizationManager categorizationManager = new CategorizationManager(
                categorizationRequestsRepositoryMock.Object, categorizationRequestMapper, edreamsConfigurationMock.Object, loggerMock.Object, emailRepositoryMock.Object, validator);

            // Prepare a request to use for when calling the "AddCategorizationRequest" method.
            string internetMessageId = string.Empty;
            List<string> recipientList = new List<string>();
            #endregion

            #region [ ACT ]
            // Call the "AddCategorizationRequest" method using the prepared request.
            EdreamsValidationException exception = await Assert.ThrowsAsync<EdreamsValidationException>(() => categorizationManager.AddCategorizationRequest(internetMessageId, recipientList, Enums.CategorizationRequestType.EmailUploaded));
            #endregion

            #region [ ASSERT ]
            // If there are is no "internetMessageId" it should throw internetmessageid is required validation
            exception.Should().NotBeNull();
            exception.ValidationErrors[0].Should().Be(ValidationMessages.WebApi.InternetMessageIdRequired);
            #endregion
        }
        [Fact]
        public async Task CategorizationManager_AddCategorizationRequest__Without_RecipientsList_Should_Throw_Validattion()
        {
            #region [ ARRANGE ]

            // Create mocks for all dependencies
            var categorizationRequestsRepositoryMock = new Mock<IRepository<CategorizationRequestEntity>>();
            var categorizationRequestMapper = new CategorizationRequestMapper();
            var emailRepositoryMock = new Mock<IRepository<Email>>();
            var validator = new Validator();
            var edreamsConfigurationMock = new Mock<IEdreamsConfiguration>();
            var loggerMock = new Mock<ILogger>();

            // Create an instance of the "CategorizationManager" using the mocked dependencies.
            ICategorizationManager categorizationManager = new CategorizationManager(
                categorizationRequestsRepositoryMock.Object, categorizationRequestMapper, edreamsConfigurationMock.Object, loggerMock.Object, emailRepositoryMock.Object, validator);

            // Prepare a request to use for when calling the "AddCategorizationRequest" method.
            string internetMessageId = "<b267a11ab00348e3aa77bd20e20e1060>";
            List<string> recipientList = new List<string>();
            #endregion

            #region [ ACT ]
            // Call the "AddCategorizationRequest" method using the prepared request.
            EdreamsValidationException exception = await Assert.ThrowsAsync<EdreamsValidationException>(() => categorizationManager.AddCategorizationRequest(internetMessageId, recipientList, Enums.CategorizationRequestType.EmailUploaded));
            #endregion

            #region [ ASSERT ]
            // If there is no "RecipientsList" it should throw RecipientsList is required validation
            exception.Should().NotBeNull();
            exception.ValidationErrors[0].Should().Be(ValidationMessages.WebApi.RecipientsListRequired);
            #endregion
        }
        [Fact]
        public async Task CategorizationManager_AddCategorizationRequest_Should_Add_CategorizationRequest()
        {
            #region [ ARRANGE ]

            // Create mocks for all dependencies
            var categorizationRequestsRepositoryMock = new Mock<IRepository<CategorizationRequestEntity>>();
            var categorizationRequestMapper = new CategorizationRequestMapper();
            var emailRepositoryMock = new Mock<IRepository<Email>>();
            var validator = new Validator();
            var edreamsConfigurationMock = new Mock<IEdreamsConfiguration>();
            var loggerMock = new Mock<ILogger>();

            // Create an instance of the "CategorizationManager" using the mocked dependencies.
            CategorizationRequestEntity categorizationRequestEntity = new CategorizationRequestEntity()
            {
                EmailAddress = "edreamstest@deloitte.com",
                Id = new Guid("2D1EE7EC-6E7D-46DA-B2DD-9EC6F0C66653"),
                InternetMessageId = "<b267a11ab00348e3aa77bd20e20e1060>",
                Status = Enums.CategorizationRequestStatus.Pending
            };
            List<CategorizationRequestEntity> categorizationRequestEntityList = new List<CategorizationRequestEntity>() { categorizationRequestEntity };

            ICategorizationManager categorizationManager = new CategorizationManager(
                categorizationRequestsRepositoryMock.Object, categorizationRequestMapper, edreamsConfigurationMock.Object, loggerMock.Object, emailRepositoryMock.Object, validator);

            // Prepare a request to use for when calling the "AddCategorizationRequest" method.
            Email emailEntity = new Email()
            {
                InternetMessageId = "<b267a11ab00348e3aa77bd20e20e1060>",
            };
            List<Email> emailEntityList = new List<Email>() { emailEntity };
            string internetMessageId = "<b267a11ab00348e3aa77bd20e20e1060>";
            List<string> recipientList = new List<string>() { "edreamstest@deloitte.com" };
            #endregion

            #region [ MOCK ]
            // Mock the "FirstDescending" method on the "Repository" and run the predicate lambda expression on
            // an  list of "categorizationResult" objects.
            emailRepositoryMock.SetupRepositoryGetFirstDescending<Email, Guid>(emailEntityList);
            #endregion

            #region [ ACT ]

            // Call the "UpdatePendingCategoriesResponse" method using the prepared request.
            await categorizationManager.AddCategorizationRequest(internetMessageId, recipientList, Enums.CategorizationRequestType.AttachmentUploaded);

            #endregion

            #region [ ASSERT ]

            // If there are no "PendingCategories" objects found, the "GetPendingCategories" method should return Null.
            categorizationRequestsRepositoryMock.VerifyRepositoryCreate(Times.Once());

            #endregion
        }
        #endregion
    }
}
