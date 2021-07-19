using System.Threading.Tasks;
using Edreams.Common.Exceptions;
using Edreams.Common.Logging.Interfaces;
using Edreams.Contracts.Data.Common;
using Edreams.Contracts.Data.Extensibility;
using Edreams.OutlookMiddleware.BusinessLogic;
using Edreams.OutlookMiddleware.Common.Constants;
using Edreams.OutlookMiddleware.Common.Helpers.Interfaces;
using Edreams.OutlookMiddleware.Common.Validation;
using Edreams.OutlookMiddleware.Common.Validation.Interface;
using FluentAssertions;
using Moq;
using Xunit;
using ProjectTask = Edreams.Contracts.Data.Extensibility.ProjectTask;

namespace Edreams.OutlookMiddleware.Tests.BusinessLogic
{
    public class ExtensibilityManagerTests
    {
        #region <| SetSuggestedSites |>

        [Fact]
        public async Task ExtensibilityManager_SetSuggestedSites_Empty_siteUrl_Value_Should_Return_Validation()
        {
            #region [ ARRANGE ]

            // Create mocks for all dependencies
            var suggestedSiteRestHelperMock = new Mock<IRestHelper<SuggestedSite>>();
            var sharePointFileRestHelperMock = new Mock<IRestHelper<SharePointFile>>();
            var projectTaskRestHelperMock = new Mock<IRestHelper<ProjectTask>>();
            var validator = new Validator();
            var loggerMock = new Mock<IEdreamsLogger<ExtensibilityManager>>();


            // Create an instance of the "ExtensibilityManager" using the mocked dependencies.
            ExtensibilityManager extensibilityManager = new ExtensibilityManager(suggestedSiteRestHelperMock.Object, sharePointFileRestHelperMock.Object, 
                projectTaskRestHelperMock.Object, validator, loggerMock.Object);

            // Prepare a request to use for when calling the "SetSuggestedSites" method.
            string from = string.Empty;
            string siteUrl = string.Empty;
            string principalName = string.Empty;
            #endregion

            #region [ ACT ]
            // Call the "SetSuggestedSites" method using the prepared request.
            EdreamsValidationException exception = await Assert.ThrowsAsync<EdreamsValidationException>(() => extensibilityManager.SetSuggestedSites(from, siteUrl, principalName));
            #endregion

            #region [ ASSERT ]
            // If there are is no "from" it should throw from is required validation
            exception.Should().NotBeNull();
            // verify validation message from is required.
            exception.ValidationErrors[0].Should().Be(ValidationMessages.WebApi.FromRequired);
            #endregion
        }

        [Fact]
        public async Task ExtensibilityManager_SetSuggestedSites_Empty_SiteUrl_Value_Should_Return_Validation()
        {
            #region [ ARRANGE ]

            // Create mocks for all dependencies
            var suggestedSiteRestHelperMock = new Mock<IRestHelper<SuggestedSite>>();
            var sharePointFileRestHelperMock = new Mock<IRestHelper<SharePointFile>>();
            var projectTaskRestHelperMock = new Mock<IRestHelper<ProjectTask>>();
            var validator = new Validator();
            var loggerMock = new Mock<IEdreamsLogger<ExtensibilityManager>>();


            // Create an instance of the "ExtensibilityManager" using the mocked dependencies.
            ExtensibilityManager extensibilityManager = new ExtensibilityManager(suggestedSiteRestHelperMock.Object, sharePointFileRestHelperMock.Object,
                projectTaskRestHelperMock.Object, validator, loggerMock.Object);

            // Prepare a request to use for when calling the "SetSuggestedSites" method.

            string from = "edreamstest@deloitte.com";
            string siteUrl = string.Empty;
            string principalName = string.Empty;
            #endregion

            #region [ ACT ]
            // Call the "SetSuggestedSites" method using the prepared request.
            EdreamsValidationException exception = await Assert.ThrowsAsync<EdreamsValidationException>(() => extensibilityManager.SetSuggestedSites(from, siteUrl, principalName));
            #endregion

            #region [ ASSERT ]
            // If there are is no "from" it should throw site url is required validation
            exception.Should().NotBeNull();
            exception.ValidationErrors[0].Should().Be(ValidationMessages.WebApi.SiteUrlRequired);
            #endregion
        }

        [Fact]
        public async Task ExtensibilityManager_SetSuggestedSites_Empty_From_Value_Should_Return_Validation()
        {
            #region [ ARRANGE ]

            // Create mocks for all dependencies
            var suggestedSiteRestHelperMock = new Mock<IRestHelper<SuggestedSite>>();
            var sharePointFileRestHelperMock = new Mock<IRestHelper<SharePointFile>>();
            var projectTaskRestHelperMock = new Mock<IRestHelper<ProjectTask>>();
            var validator = new Validator();
            var loggerMock = new Mock<IEdreamsLogger<ExtensibilityManager>>();


            // Create an instance of the "ExtensibilityManager" using the mocked dependencies.
            ExtensibilityManager extensibilityManager = new ExtensibilityManager(suggestedSiteRestHelperMock.Object, sharePointFileRestHelperMock.Object,
                projectTaskRestHelperMock.Object, validator, loggerMock.Object);

            // Prepare a request to use for when calling the "SetSuggestedSites" method.

            string from = "edreamstest@deloitte.com";
            string siteUrl = "edreams/sites/Rdadfs/Tsdesrs";
            string principalName = string.Empty;
            #endregion

            #region [ ACT ]
            // Call the "SetSuggestedSites" method using the prepared request.
            EdreamsValidationException exception = await Assert.ThrowsAsync<EdreamsValidationException>(() => extensibilityManager.SetSuggestedSites(from, siteUrl, principalName));
            #endregion

            #region [ ASSERT ]
            // If there are is no "from" it should throw principlename is required validation
            exception.Should().NotBeNull();
            exception.ValidationErrors[0].Should().Be(ValidationMessages.WebApi.PrincipalNameRequired);
            #endregion
        }

        [Fact]
        public async Task ExtensibilityManager_SetSuggestedSites_Should_Create_Suggested_Site()
        {
            #region [ ARRANGE ]

            // Create mocks for all dependencies
            var suggestedSiteRestHelperMock = new Mock<IRestHelper<SuggestedSite>>();
            var sharePointFileRestHelperMock = new Mock<IRestHelper<SharePointFile>>();
            var projectTaskRestHelperMock = new Mock<IRestHelper<ProjectTask>>();
            var validator = new Validator();
            var loggerMock = new Mock<IEdreamsLogger<ExtensibilityManager>>();


            // Create an instance of the "ExtensibilityManager" using the mocked dependencies.
            ExtensibilityManager extensibilityManager = new ExtensibilityManager(suggestedSiteRestHelperMock.Object, sharePointFileRestHelperMock.Object,
                projectTaskRestHelperMock.Object, validator, loggerMock.Object);

            // Prepare a request to use for when calling the "SetSuggestedSites" method.

            string from = "edreamstest@deloitte.com";
            string siteUrl = "edreams/sites/Rdadfs/Tsdesrs";
            string principalName = "edreamstest@deloitte.com";
            #endregion

            #region [ MOCK ]

            // Mock the "Find" method on the "Repository" and run the predicate lambda expression on
            // an empty list of "FilePreload" objects.
            suggestedSiteRestHelperMock.Setup(x=>x.CreateNew(It.IsAny<string>(),It.IsAny<SuggestedSite>(),false)).ReturnsAsync(new ApiResult<SuggestedSite>() { });

            #endregion

            #region [ ACT ]
            // Call the "SetSuggestedSites" method using the prepared request.
            await extensibilityManager.SetSuggestedSites(from, siteUrl, principalName);
            #endregion

            #region [ ASSERT ]
            //verify SuggestedSiteRepositorys CreateNew method calls once.
            suggestedSiteRestHelperMock.Verify(x => x.CreateNew(It.IsAny<string>(), It.IsAny<SuggestedSite>(), false), Times.Once);
            #endregion
        }
        
        #endregion

        #region <| UploadFile |>

        [Fact]
        public async Task ExtensibilityManager_UploadFile_Empty_siteUrl_Value_Should_Return_Validation()
        {
            #region [ ARRANGE ]

            // Create mocks for all dependencies
            var suggestedSiteRestHelperMock = new Mock<IRestHelper<SuggestedSite>>();
            var sharePointFileRestHelperMock = new Mock<IRestHelper<SharePointFile>>();
            var projectTaskRestHelperMock = new Mock<IRestHelper<ProjectTask>>();
            var validator = new Validator();
            var loggerMock = new Mock<IEdreamsLogger<ExtensibilityManager>>();


            // Create an instance of the "ExtensibilityManager" using the mocked dependencies.
            ExtensibilityManager extensibilityManager = new ExtensibilityManager(suggestedSiteRestHelperMock.Object, sharePointFileRestHelperMock.Object,
                projectTaskRestHelperMock.Object, validator, loggerMock.Object);

            // Prepare a request to use for when calling the "UploadFile" method.

            string siteUrl = string.Empty;
            string folderName = string.Empty;
            string fileName = string.Empty;
            byte[] fileBytes = new byte[10];
            bool overrite = false;
            #endregion

            #region [ ACT ]
            // Call the "UploadFile" method using the prepared request.
            EdreamsValidationException exception = await Assert.ThrowsAsync<EdreamsValidationException>(() => extensibilityManager.UploadFile(fileBytes,siteUrl,folderName,fileName, overrite));
            #endregion

            #region [ ASSERT ]
            // If there are is no "siteUr" it should throw  site url required validation
            exception.Should().NotBeNull();
            exception.ValidationErrors[0].Should().Be(ValidationMessages.WebApi.SiteUrlRequired);
            #endregion
        }

        [Fact]
        public async Task ExtensibilityManager_UploadFile_Empty_Folder_Value_Should_Return_Validation()
        {
            #region [ ARRANGE ]

            // Create mocks for all dependencies
            var suggestedSiteRestHelperMock = new Mock<IRestHelper<SuggestedSite>>();
            var sharePointFileRestHelperMock = new Mock<IRestHelper<SharePointFile>>();
            var projectTaskRestHelperMock = new Mock<IRestHelper<ProjectTask>>();
            var validator = new Validator();
            var loggerMock = new Mock<IEdreamsLogger<ExtensibilityManager>>();


            // Create an instance of the "ExtensibilityManager" using the mocked dependencies.
            ExtensibilityManager extensibilityManager = new ExtensibilityManager(suggestedSiteRestHelperMock.Object, sharePointFileRestHelperMock.Object,
                projectTaskRestHelperMock.Object, validator, loggerMock.Object);

            // Prepare a request to use for when calling the "UploadFile" method.
            string siteUrl = "eDreams/sites/Adsdrr/Defkr";
            string folderName = string.Empty;
            string fileName = string.Empty;
            byte[] fileBytes = new byte[10];
            bool overrite = false;
            #endregion

            #region [ ACT ]
            // Call the "UploadFile" method using the prepared request.
            EdreamsValidationException exception = await Assert.ThrowsAsync<EdreamsValidationException>(() => extensibilityManager.UploadFile(fileBytes, siteUrl, folderName, fileName, overrite));
            #endregion

            #region [ ASSERT ]
            // If there are is no "Folder" it should throw  Folder is required validation
            exception.Should().NotBeNull();
            exception.ValidationErrors[0].Should().Be(ValidationMessages.WebApi.FolderRequired);
            #endregion
        }

        [Fact]
        public async Task ExtensibilityManager_UploadFile_Empty_FileName_Value_Should_Return_Validation()
        {
            #region [ ARRANGE ]

            // Create mocks for all dependencies
            var suggestedSiteRestHelperMock = new Mock<IRestHelper<SuggestedSite>>();
            var sharePointFileRestHelperMock = new Mock<IRestHelper<SharePointFile>>();
            var projectTaskRestHelperMock = new Mock<IRestHelper<ProjectTask>>();
            var validator = new Validator();
            var loggerMock = new Mock<IEdreamsLogger<ExtensibilityManager>>();


            // Create an instance of the "ExtensibilityManager" using the mocked dependencies.
            ExtensibilityManager extensibilityManager = new ExtensibilityManager(suggestedSiteRestHelperMock.Object, sharePointFileRestHelperMock.Object,
                projectTaskRestHelperMock.Object, validator, loggerMock.Object);

            // Prepare a request to use for when calling the "UploadFile" method.

            string siteUrl = "eDreams/sites/Adsdrr/Defkr";
            string folderName = "AllDocuments";
            string fileName = string.Empty;
            byte[] fileBytes = new byte[10];
            bool overrite = false;
            #endregion

            #region [ ACT ]
            // Call the "UploadFile" method using the prepared request.
            EdreamsValidationException exception = await Assert.ThrowsAsync<EdreamsValidationException>(() => extensibilityManager.UploadFile(fileBytes, siteUrl, folderName, fileName, overrite));
            #endregion

            #region [ ASSERT ]
            // If there are is no "Folder" it should throw  FileName is required validation
            exception.Should().NotBeNull();
            exception.ValidationErrors[0].Should().Be(ValidationMessages.WebApi.FileNameRequired);
            #endregion
        }

        [Fact]
        public async Task ExtensibilityManager_UploadFile_Should_Upload_File()
        {
            #region [ ARRANGE ]

            // Create mocks for all dependencies
            var suggestedSiteRestHelperMock = new Mock<IRestHelper<SuggestedSite>>();
            var sharePointFileRestHelperMock = new Mock<IRestHelper<SharePointFile>>();
            var projectTaskRestHelperMock = new Mock<IRestHelper<ProjectTask>>();
            var validator = new Validator();
            var loggerMock = new Mock<IEdreamsLogger<ExtensibilityManager>>();


            // Create an instance of the "ExtensibilityManager" using the mocked dependencies.
            ExtensibilityManager extensibilityManager = new ExtensibilityManager(suggestedSiteRestHelperMock.Object, sharePointFileRestHelperMock.Object,
                projectTaskRestHelperMock.Object, validator, loggerMock.Object);

            // Prepare a request to use for when calling the "SetSuggestedSites" method.

            string siteUrl = "eDreams/sites/Adsdrr/Defkr";
            string folderName = "AllDocuments";
            string fileName = "TaxDocument123.doc";
            byte[] fileBytes = new byte[10];
            bool overrite = false;
            #endregion

            #region [ MOCK ]

            // Mock the "Find" method on the "Repository" and run the predicate lambda expression on
            // an empty list of "FilePreload" objects.
            sharePointFileRestHelperMock.Setup(x => x.CreateFile(It.IsAny<string>(), It.IsAny<RestSharp.FileParameter>(), false)).ReturnsAsync(new ApiResult<SharePointFile>() { Content = new SharePointFile() { AbsoluteUrl= "eDreams/sites/Adsdrr/Defkr/AllDocuments/TaxDocument123.doc" } });

            #endregion

            #region [ ACT ]
            // Call the "UploadFile" method using the prepared request.
            SharePointFile result = await extensibilityManager.UploadFile(fileBytes, siteUrl, folderName, fileName, overrite);
            #endregion

            #region [ ASSERT ]
            // If file is uploaded then it shoudld return uploaded file details
            result.Should().NotBeNull();
            result.AbsoluteUrl.Should().Be("eDreams/sites/Adsdrr/Defkr/AllDocuments/TaxDocument123.doc");
            #endregion
        }
        
        [Fact(Skip ="Need to write tests  for exception cases.")]
        public async Task ExtensibilityManager_UploadFile_Should_Throw_Exception()
        {
            #region [ ARRANGE ]

            // Create mocks for all dependencies
            var suggestedSiteRestHelperMock = new Mock<IRestHelper<SuggestedSite>>();
            var sharePointFileRestHelperMock = new Mock<IRestHelper<SharePointFile>>();
            var projectTaskRestHelperMock = new Mock<IRestHelper<ProjectTask>>();
            var validator = new Validator();
            var loggerMock = new Mock<IEdreamsLogger<ExtensibilityManager>>();


            // Create an instance of the "ExtensibilityManager" using the mocked dependencies.
            ExtensibilityManager extensibilityManager = new ExtensibilityManager(suggestedSiteRestHelperMock.Object, sharePointFileRestHelperMock.Object,
                projectTaskRestHelperMock.Object, validator, loggerMock.Object);

            // Prepare a request to use for when calling the "SetSuggestedSites" method.

            string siteUrl = "eDreams/sites/Adsdrr/Defkr";
            string folderName = "AllDocuments";
            string fileName = string.Empty;
            byte[] fileBytes = new byte[10];
            bool overrite = false;
            #endregion

            #region [ ACT ]
            // Call the "UploadFile" method using the prepared request.
            await extensibilityManager.UploadFile(fileBytes, siteUrl, folderName, fileName, overrite);
            #endregion

            #region [ ASSERT ]
            // If there are is no "from" it should throw principlename is required validation
            //exception.Should().NotBeNull();
            //exception.ValidationErrors[0].Should().Be(ValidationMessages.WebApi.PrincipalNameRequired);
            #endregion
        }

        #endregion
    }
}