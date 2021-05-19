using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Edreams.Common.DataAccess.Interfaces;
using Edreams.OutlookMiddleware.BusinessLogic;
using Edreams.OutlookMiddleware.BusinessLogic.Interfaces;
using Edreams.OutlookMiddleware.BusinessLogic.Transactions;
using Edreams.OutlookMiddleware.Common.Configuration.Interfaces;
using Edreams.OutlookMiddleware.Common.Helpers.Interfaces;
using Edreams.OutlookMiddleware.Mapping;
using Edreams.OutlookMiddleware.Model;
using Edreams.OutlookMiddleware.Tests.Framework.Extensions;
using FluentAssertions;
using Moq;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace Edreams.OutlookMiddleware.Tests.BusinessLogic
{
    public class CleanupManagerTests
    {
        #region <| ExpirePreloadedFiles |>

        [Fact]
        public async Task CleanupManager_ExpirePreloadedFiles_Should_Update_PreloadedFiles()
        {
            #region [ ARRANGE ]

            // Create mocks for all dependencies
            var preloadedFilesRepositoryMock = new Mock<IRepository<FilePreload>>();
            var transactionHistoryRepositoryMock = new Mock<IRepository<HistoricTransaction>>();
            var categorizationRequestsRepositoryMock = new Mock<IRepository<CategorizationRequest>>();
            var categorizationRequestMapper = new CategorizationRequestMapper();
            var batchRepositoryMock = new Mock<IRepository<Batch>>();
            var emailRepositoryMock = new Mock<IRepository<Email>>();
            var emailRecipientRepositoryMock = new Mock<IRepository<EmailRecipient>>();
            var fileRepositoryMock = new Mock<IRepository<File>>();
            var fileHelperMock = new Mock<IFileHelper>();
            var transactionHelperMock = new Mock<TransactionHelper>();
            var edreamsConfigurationMock = new Mock<IEdreamsConfiguration>();

            // Create an instance of the "CleanupManager" using the mocked dependencies.
            ICleanupManager cleanupManager = new CleanupManager(
                preloadedFilesRepositoryMock.Object,
                transactionHistoryRepositoryMock.Object,
                categorizationRequestsRepositoryMock.Object,
                batchRepositoryMock.Object,
                emailRepositoryMock.Object,
                emailRecipientRepositoryMock.Object,
                fileRepositoryMock.Object,
                fileHelperMock.Object,
                transactionHelperMock.Object,
                edreamsConfigurationMock.Object);

            // Prepare a request to use for when calling the "ExpirePreloadedFiles" method.
            FilePreload preloadFile = new FilePreload()
            {
                FileName = "TaxDocument.doc",
                EmailId = new Guid(),
                FileStatus = Enums.FilePreloadStatus.Pending,
                PreloadedOn = DateTime.Now.AddDays(-3),
                EmailSubject = "Files Upload"
            };
            List<FilePreload> preloadedFiles = new List<FilePreload>() { preloadFile };
            #endregion

            #region [ MOCK ]
            // Mock the configuration to set PreloadedFilesExpiryInMinutes to value 10.
            edreamsConfigurationMock.Setup(x => x.PreloadedFilesExpiryInMinutes).Returns(10);
            // Mock the "Find" method on the "Repository" and run the predicate lambda expression on
            // an  list of "preloadFile" objects.
            preloadedFilesRepositoryMock.SetupRepositoryFind(preloadedFiles);
            #endregion

            #region [ ACT ]s

            // Call the "ExpirePreloadedFiles" method using the prepared request.
            int filesCount = await cleanupManager.ExpirePreloadedFiles();

            #endregion

            #region [ ASSERT ]

            // The "ExpirePreloadedFiles" method should return Expired Preloaded Files count 1
            filesCount.Should().Be(1);
            // Verify the PreloadedFile repository's updated method should be called once.
            preloadedFilesRepositoryMock.VerifyRepositoryUpdate(Times.Once());
            #endregion
        }
        #endregion

        #region <| ExpireTransactions |>
        [Fact]
        public async Task CleanupManager_ExpireTransactions_Should_Update_PreloadedFiles()
        {
            #region [ ARRANGE ]

            // Create mocks for all dependencies
            var preloadedFilesRepositoryMock = new Mock<IRepository<FilePreload>>();
            var transactionHistoryRepositoryMock = new Mock<IRepository<HistoricTransaction>>();
            var categorizationRequestsRepositoryMock = new Mock<IRepository<CategorizationRequest>>();
            var categorizationRequestMapper = new CategorizationRequestMapper();
            var batchRepositoryMock = new Mock<IRepository<Batch>>();
            var emailRepositoryMock = new Mock<IRepository<Email>>();
            var emailRecipientRepositoryMock = new Mock<IRepository<EmailRecipient>>();
            var fileRepositoryMock = new Mock<IRepository<File>>();
            var fileHelperMock = new Mock<IFileHelper>();
            var transactionHelperMock = new Mock<TransactionHelper>();
            var edreamsConfigurationMock = new Mock<IEdreamsConfiguration>();

            // Create an instance of the "CleanupManager" using the mocked dependencies.
            ICleanupManager cleanupManager = new CleanupManager(
                preloadedFilesRepositoryMock.Object,
                transactionHistoryRepositoryMock.Object,
                categorizationRequestsRepositoryMock.Object,
                batchRepositoryMock.Object,
                emailRepositoryMock.Object,
                emailRecipientRepositoryMock.Object,
                fileRepositoryMock.Object,
                fileHelperMock.Object,
                transactionHelperMock.Object,
                edreamsConfigurationMock.Object);

            // Prepare a request to use for when calling the "ExpireTransactions" method.
            HistoricTransaction historicTransaction = new HistoricTransaction()
            {
                CorrelationId = new Guid(),
                Status = Enums.TransactionStatus.ProcessingSucceeded,
                ProcessingFinished = DateTime.Now.AddDays(-1)
            };
            List<HistoricTransaction> historicTransactions = new List<HistoricTransaction>() { historicTransaction };
            #endregion

            #region [ MOCK ]
            // Mock the configuration to setup PreloadedFilesExpiryInMinutes to 10
            edreamsConfigurationMock.Setup(x => x.PreloadedFilesExpiryInMinutes).Returns(10);
            // Mock the "Find" method on the "Repository" and run the predicate lambda expression on
            // an  list of "historicTransaction" objects.
            transactionHistoryRepositoryMock.SetupRepositoryFind(historicTransactions);
            #endregion

            #region [ ACT ]

            // Call the "ExpireTransactions" method using the prepared request.
            int filesCount = await cleanupManager.ExpireTransactions();

            #endregion

            #region [ ASSERT ]

            // The "ExpireTransactions" method should return files count 1.
            filesCount.Should().Be(1);
            // Verify TransactionHistory repository's update method should be called once
            transactionHistoryRepositoryMock.VerifyRepositoryUpdate(Times.Once());
            #endregion
        }
        #endregion

        #region <| CleanupPreloadedFiles |>
        [Fact]
        public async Task CleanupManager_CleanupPreloadedFiles_When_No_Expired_No_Cancel_Status_Should_Not_Delete_Files()
        {
            #region [ ARRANGE ]

            // Create mocks for all dependencies
            var preloadedFilesRepositoryMock = new Mock<IRepository<FilePreload>>();
            var transactionHistoryRepositoryMock = new Mock<IRepository<HistoricTransaction>>();
            var categorizationRequestsRepositoryMock = new Mock<IRepository<CategorizationRequest>>();
            var categorizationRequestMapper = new CategorizationRequestMapper();
            var batchRepositoryMock = new Mock<IRepository<Batch>>();
            var emailRepositoryMock = new Mock<IRepository<Email>>();
            var emailRecipientRepositoryMock = new Mock<IRepository<EmailRecipient>>();
            var fileRepositoryMock = new Mock<IRepository<File>>();
            var fileHelperMock = new Mock<IFileHelper>();
            var transactionHelperMock = new Mock<TransactionHelper>();
            var edreamsConfigurationMock = new Mock<IEdreamsConfiguration>();

            // Create an instance of the "CleanupManager" using the mocked dependencies.
            ICleanupManager cleanupManager = new CleanupManager(
                preloadedFilesRepositoryMock.Object,
                transactionHistoryRepositoryMock.Object,
                categorizationRequestsRepositoryMock.Object,
                batchRepositoryMock.Object,
                emailRepositoryMock.Object,
                emailRecipientRepositoryMock.Object,
                fileRepositoryMock.Object,
                fileHelperMock.Object,
                transactionHelperMock.Object,
                edreamsConfigurationMock.Object);

            // Prepare a request to use for when calling the "CleanupPreloadedFiles" method.
            FilePreload preloadFile = new FilePreload()
            {
                FileName = "TaxDocument.doc",
                EmailId = new Guid(),
                Status = Enums.EmailPreloadStatus.Committed,
                PreloadedOn = DateTime.Now.AddDays(-3),
                EmailSubject = "Files Upload"
            };
            List<FilePreload> preloadedFiles = new List<FilePreload>() { preloadFile };
            #endregion

            #region [ MOCK ]

            edreamsConfigurationMock.Setup(x => x.PreloadedFilesExpiryInMinutes).Returns(10);
            // Mock the "FindAndProject" method on the "Repository" and run the predicate lambda expression on
            // an  list of "preloadFile" objects.
            preloadedFilesRepositoryMock.SetupRepositoryFindAndProject<FilePreload, object>(preloadedFiles);
            // Mock the "DeleteFile" method on the "Repository" and run the predicate lambda expression on
            // an  list of  "filePath" objects.
            fileHelperMock.Setup(x => x.DeleteFile(It.IsAny<List<string>>()));
            // Mock the "Delete" method on the "Repository" and run the predicate lambda expression on
            // an "FilePreload" object.
            preloadedFilesRepositoryMock.Setup(x => x.Delete(It.IsAny<FilePreload>()));
            #endregion

            #region [ ACT ]s

            // Call the "CleanupPreloadedFiles" method using the prepared request.
            int filesCount = await cleanupManager.CleanupPreloadedFiles();

            #endregion

            #region [ ASSERT ]

            // The "CleanupPreloadedFiles" method should return files count zero 
            filesCount.Should().Be(0);
            // Verify the PreloadedFiles Repository's update method should not call.
            preloadedFilesRepositoryMock.VerifyRepositoryUpdate(Times.Never());
            #endregion
        }
        [Fact(Skip = "Need to mock SetupRepositoryFindAndProject for anonymouse types")]
        public async Task CleanupManager_CleanupPreloadedFiles_When_Expired_Cancel_Status_Should_Delete_Files()
        {
            #region [ ARRANGE ]

            // Create mocks for all dependencies
            var preloadedFilesRepositoryMock = new Mock<IRepository<FilePreload>>();
            var transactionHistoryRepositoryMock = new Mock<IRepository<HistoricTransaction>>();
            var categorizationRequestsRepositoryMock = new Mock<IRepository<CategorizationRequest>>();
            var categorizationRequestMapper = new CategorizationRequestMapper();
            var batchRepositoryMock = new Mock<IRepository<Batch>>();
            var emailRepositoryMock = new Mock<IRepository<Email>>();
            var emailRecipientRepositoryMock = new Mock<IRepository<EmailRecipient>>();
            var fileRepositoryMock = new Mock<IRepository<File>>();
            var fileHelperMock = new Mock<IFileHelper>();
            var transactionHelperMock = new Mock<TransactionHelper>();
            var edreamsConfigurationMock = new Mock<IEdreamsConfiguration>();

            // Create an instance of the "CleanupManager" using the mocked dependencies.
            ICleanupManager cleanupManager = new CleanupManager(
                preloadedFilesRepositoryMock.Object,
                transactionHistoryRepositoryMock.Object,
                categorizationRequestsRepositoryMock.Object,
                batchRepositoryMock.Object,
                emailRepositoryMock.Object,
                emailRecipientRepositoryMock.Object,
                fileRepositoryMock.Object,
                fileHelperMock.Object,
                transactionHelperMock.Object,
                edreamsConfigurationMock.Object);

            // Prepare a request to use for when calling the "CleanupPreloadedFiles" method.
            FilePreload preloadFile = new FilePreload()
            {
                FileName = "TaxDocument.doc",
                EmailId = new Guid(),
                Status = Enums.EmailPreloadStatus.Expired,
                PreloadedOn = DateTime.Now.AddDays(-3),
                EmailSubject = "Files Upload"
            };
            List<FilePreload> preloadedFiles = new List<FilePreload>() { preloadFile };
            #endregion

            #region [ MOCK ]

            edreamsConfigurationMock.Setup(x => x.PreloadedFilesExpiryInMinutes).Returns(10);
            // Mock the "GetFirstAscending" method on the "Repository" and run the predicate lambda expression on
            // an  list of "preloadFile" objects.
            preloadedFilesRepositoryMock.SetupRepositoryGetFirstAscending<FilePreload, DateTime>(preloadedFiles);
            // Mock the "FindAndProject" method on the "Repository" and run the predicate lambda expression on
            // an  list of "preloadedFile" objects.
            preloadedFilesRepositoryMock.SetupRepositoryFindAndProject<FilePreload, dynamic>(preloadedFiles);
            // Mock the "DeleteFile" method on the "Repository" and run the predicate lambda expression on
            // an  list of "filepaths" 
            fileHelperMock.Setup(x => x.DeleteFile(It.IsAny<List<string>>())).Returns(Task.CompletedTask);
            // Mock the "Delete" method on the "Repository" and run the predicate lambda expression on
            // an  list of "fileids"
            preloadedFilesRepositoryMock.Setup(x => x.Delete(It.IsAny<List<Guid>>())).ReturnsAsync(true);
            #endregion

            #region [ ACT ]s

            // Call the "CleanupPreloadedFiles" method using the prepared request.
            int filesCount = await cleanupManager.CleanupPreloadedFiles();

            #endregion

            #region [ ASSERT ]

            // The "CleanupPreloadedFiles" method should return filesCount to 1.
            filesCount.Should().Be(1);
            // Verify Preloaded files repository's update method call once.
            preloadedFilesRepositoryMock.VerifyRepositoryUpdate(Times.Once());
            #endregion
        }
        #endregion

        #region <| CleanupTransactions |>
        [Fact]
        public async Task CleanupManager_CleanupTransactions_When_No_Expired_Should_Not_Delete_Transactions()
        {
            #region [ ARRANGE ]

            // Create mocks for all dependencies
            var preloadedFilesRepositoryMock = new Mock<IRepository<FilePreload>>();
            var transactionHistoryRepositoryMock = new Mock<IRepository<HistoricTransaction>>();
            var categorizationRequestsRepositoryMock = new Mock<IRepository<CategorizationRequest>>();
            var batchRepositoryMock = new Mock<IRepository<Batch>>();
            var emailRepositoryMock = new Mock<IRepository<Email>>();
            var emailRecipientRepositoryMock = new Mock<IRepository<EmailRecipient>>();
            var fileRepositoryMock = new Mock<IRepository<File>>();
            var fileHelperMock = new Mock<IFileHelper>();
            var transactionHelperMock = new Mock<TransactionHelper>();
            var edreamsConfigurationMock = new Mock<IEdreamsConfiguration>();

            // Create an instance of the "CleanupManager" using the mocked dependencies.
            ICleanupManager cleanupManager = new CleanupManager(
                 preloadedFilesRepositoryMock.Object,
                 transactionHistoryRepositoryMock.Object,
                 categorizationRequestsRepositoryMock.Object,
                 batchRepositoryMock.Object,
                 emailRepositoryMock.Object,
                 emailRecipientRepositoryMock.Object,
                 fileRepositoryMock.Object,
                 fileHelperMock.Object,
                 transactionHelperMock.Object,
                 edreamsConfigurationMock.Object);

            // Prepare a request to use for when calling the "CleanupTransactions" method.
            HistoricTransaction historicTransaction = new HistoricTransaction()
            {
                CorrelationId = new Guid(),
                Status = Enums.TransactionStatus.Queued
            };
            List<HistoricTransaction> historicTransactions = new List<HistoricTransaction>() { historicTransaction };
            #endregion

            #region [ MOCK ]
            // Mock the "FindAndProject" method on the "Repository" and run the predicate lambda expression on
            // an  list of "historicTransactions" objects.
            transactionHistoryRepositoryMock.SetupRepositoryFindAndProject<HistoricTransaction, DateTime>(historicTransactions);
            #endregion

            #region [ ACT ]s

            // Call the "CleanupTransactions" method using the prepared request.
            int filesCount = await cleanupManager.CleanupTransactions();

            #endregion

            #region [ ASSERT ]

            // The "CleanupTransactions" method should return files count as zero
            filesCount.Should().Be(0);
            // Verify Preloaded files repository's update method never calls
            preloadedFilesRepositoryMock.VerifyRepositoryUpdate(Times.Never());
            #endregion
        }
        [Fact(Skip = "Need to mock SetupRepositoryFindAndProject for anonymouse types")]
        public async Task CleanupManager_CleanupTransactions_When_Expired_Should_Delete_Transactions()
        {
            #region [ ARRANGE ]

            // Create mocks for all dependencies
            var preloadedFilesRepositoryMock = new Mock<IRepository<FilePreload>>();
            var transactionHistoryRepositoryMock = new Mock<IRepository<HistoricTransaction>>();
            var categorizationRequestsRepositoryMock = new Mock<IRepository<CategorizationRequest>>();
            var categorizationRequestMapper = new CategorizationRequestMapper();
            var batchRepositoryMock = new Mock<IRepository<Batch>>();
            var emailRepositoryMock = new Mock<IRepository<Email>>();
            var emailRecipientRepositoryMock = new Mock<IRepository<EmailRecipient>>();
            var fileRepositoryMock = new Mock<IRepository<File>>();
            var fileHelperMock = new Mock<IFileHelper>();
            var transactionHelperMock = new Mock<TransactionHelper>();
            var edreamsConfigurationMock = new Mock<IEdreamsConfiguration>();

            // Create an instance of the "CleanupManager" using the mocked dependencies.
            ICleanupManager cleanupManager = new CleanupManager(
                 preloadedFilesRepositoryMock.Object,
                 transactionHistoryRepositoryMock.Object,
                 categorizationRequestsRepositoryMock.Object,
                 batchRepositoryMock.Object,
                 emailRepositoryMock.Object,
                 emailRecipientRepositoryMock.Object,
                 fileRepositoryMock.Object,
                 fileHelperMock.Object,
                 transactionHelperMock.Object,
                 edreamsConfigurationMock.Object);

            // Prepare a request to use for when calling the "CleanupTransactions" method.
            HistoricTransaction historicTransaction = new HistoricTransaction()
            {
                CorrelationId = new Guid(),
                Status = Enums.TransactionStatus.Expired,
                BatchId = new Guid()
            };
            List<HistoricTransaction> historicTransactions = new List<HistoricTransaction>() { historicTransaction };
            Email email = new Email()
            {
                Batch = new Batch() { Id = new Guid() },
                Id = new Guid()
            };
            List<Email> emailsList = new List<Email>() { email };
            File file = new File()
            {
                FileName = "TaxDocument.doc",
                Kind = Enums.FileKind.Attachment,
                EmailSubject = "Files Upload",
                Status = Enums.FileStatus.ReadyToUpload
            };
            List<File> files = new List<File>() { file };
            #endregion

            #region [ MOCK ]
            // Mock the "GetFirstAscending" method on the "Repository" and run the predicate lambda expression on
            // an  list of "historicTransaction" objects.
            transactionHistoryRepositoryMock.SetupRepositoryGetFirstAscending<HistoricTransaction, DateTime>(historicTransactions);
            // Mock the "FindAndProject" method on the "Repository" and run the predicate lambda expression on
            // an  list of "email" objects.
            emailRepositoryMock.SetupRepositoryFindAndProject<Email, Guid>(emailsList);
            // Mock the "FindAndProject" method on the "Repository" and run the predicate lambda expression on
            // an  list of "File" objects.
            fileRepositoryMock.SetupRepositoryFindAndProject<File, dynamic>(files);
            // Mock the "DeleteFile" method on the "Repository" and run the predicate lambda expression on
            // an  list of "FilePaths" objects.
            fileHelperMock.Setup(x => x.DeleteFile(It.IsAny<List<string>>()));
            // Mock the "Delete" method on the "Repository" and run the predicate lambda expression on
            // an  list of "File" object.
            fileRepositoryMock.Setup(x => x.Delete(It.IsAny<File>()));
            // Mock the "Delete" method on the "Repository" and run the predicate lambda expression on
            // an  list of "Email" object.
            emailRepositoryMock.Setup(x => x.Delete(It.IsAny<Email>()));
            // Mock the "Delete" method on the "Repository" and run the predicate lambda expression on
            // an  list of "Batch" object.
            batchRepositoryMock.Setup(x => x.Delete(It.IsAny<Batch>()));
            // Mock the "Delete" method on the "Repository" and run the predicate lambda expression on
            // an  list of "Guid" object.
            transactionHistoryRepositoryMock.Setup(x => x.Delete(It.IsAny<Guid>()));
            #endregion

            #region [ ACT ]s

            // Call the "CleanupTransactions" method using the prepared request.
            int filesCount = await cleanupManager.CleanupTransactions();

            #endregion

            #region [ ASSERT ]

            // The "CleanupTransactions" method should return files count be 4.
            filesCount.Should().Be(4);
            #endregion
        }
        #endregion

        #region <| CleanupCategorizations |>
        [Fact]
        public async Task CleanupManager_CleanupCategorizations_When_No_Expired_No_Cancel_Status_Should_Not_Delete_Files()
        {
            #region [ ARRANGE ]

            // Create mocks for all dependencies
            var preloadedFilesRepositoryMock = new Mock<IRepository<FilePreload>>();
            var transactionHistoryRepositoryMock = new Mock<IRepository<HistoricTransaction>>();
            var categorizationRequestsRepositoryMock = new Mock<IRepository<CategorizationRequest>>();
            var batchRepositoryMock = new Mock<IRepository<Batch>>();
            var emailRepositoryMock = new Mock<IRepository<Email>>();
            var emailRecipientRepositoryMock = new Mock<IRepository<EmailRecipient>>();
            var fileRepositoryMock = new Mock<IRepository<File>>();
            var fileHelperMock = new Mock<IFileHelper>();
            var transactionHelperMock = new Mock<TransactionHelper>();
            var edreamsConfigurationMock = new Mock<IEdreamsConfiguration>();

            // Create an instance of the "CleanupManager" using the mocked dependencies.
            ICleanupManager cleanupManager = new CleanupManager(
                preloadedFilesRepositoryMock.Object,
                transactionHistoryRepositoryMock.Object,
                categorizationRequestsRepositoryMock.Object,
                batchRepositoryMock.Object,
                emailRepositoryMock.Object,
                emailRecipientRepositoryMock.Object,
                fileRepositoryMock.Object,
                fileHelperMock.Object,
                transactionHelperMock.Object,
                edreamsConfigurationMock.Object);

            // Prepare a request to use for when calling the "CleanupTransactions" method.
            CategorizationRequest categorizationRequest = new CategorizationRequest()
            {
                EmailAddress = "edreamstest@deloitte.com",
                Id = new Guid("2D1EE7EC-6E7D-46DA-B2DD-9EC6F0C66653"),
                InternetMessageId = "<b267a11ab00348e3aa77bd20e20e1060>",
                Status = Enums.CategorizationRequestStatus.Pending
            };
            IList<CategorizationRequest> categorizationRequestList = new List<CategorizationRequest>() { categorizationRequest };
            #endregion

            #region [ MOCK ]
            // Mock the "FindAndProject" method on the "Repository" and run the predicate lambda expression on
            // an  list of "categorizationRequest" objects.
            categorizationRequestsRepositoryMock.SetupRepositoryFindAndProject<CategorizationRequest, Guid>(categorizationRequestList);
            #endregion

            #region [ ACT ]

            // Call the "CleanupCategorizations" method using the prepared request.
            int filesCount = await cleanupManager.CleanupCategorizations();

            #endregion

            #region [ ASSERT ]

            // The "CleanupCategorizations"  should return files count as 0.
            filesCount.Should().Be(0);
            #endregion
        }
        [Fact]
        public async Task CleanupManager_CleanupCategorizations_When_Expired_Cancel_Status_Should_Delete_Files()
        {
            #region [ ARRANGE ]

            // Create mocks for all dependencies
            var preloadedFilesRepositoryMock = new Mock<IRepository<FilePreload>>();
            var transactionHistoryRepositoryMock = new Mock<IRepository<HistoricTransaction>>();
            var categorizationRequestsRepositoryMock = new Mock<IRepository<CategorizationRequest>>();
            var categorizationRequestMapper = new CategorizationRequestMapper();
            var batchRepositoryMock = new Mock<IRepository<Batch>>();
            var emailRepositoryMock = new Mock<IRepository<Email>>();
            var emailRecipientRepositoryMock = new Mock<IRepository<EmailRecipient>>();
            var fileRepositoryMock = new Mock<IRepository<File>>();
            var fileHelperMock = new Mock<IFileHelper>();
            var transactionHelperMock = new Mock<TransactionHelper>();
            var edreamsConfigurationMock = new Mock<IEdreamsConfiguration>();

            // Create an instance of the "CleanupManager" using the mocked dependencies.
            ICleanupManager cleanupManager = new CleanupManager(
                preloadedFilesRepositoryMock.Object,
                transactionHistoryRepositoryMock.Object,
                categorizationRequestsRepositoryMock.Object,
                batchRepositoryMock.Object,
                emailRepositoryMock.Object,
                emailRecipientRepositoryMock.Object,
                fileRepositoryMock.Object,
                fileHelperMock.Object,
                transactionHelperMock.Object,
                edreamsConfigurationMock.Object);

            // Prepare a request to use for when calling the "CleanupTransactions" method.
            CategorizationRequest categorizationRequest = new CategorizationRequest()
            {
                EmailAddress = "edreamstest@deloitte.com",
                Id = new Guid("2D1EE7EC-6E7D-46DA-B2DD-9EC6F0C66653"),
                InternetMessageId = "<b267a11ab00348e3aa77bd20e20e1060>",
                Status = Enums.CategorizationRequestStatus.Expired
            };
            IList<CategorizationRequest> categorizationRequestList = new List<CategorizationRequest>() { categorizationRequest };
            #endregion

            #region [ MOCK ]
            // Mock the "FindAndProject" method on the "Repository" and run the predicate lambda expression on
            // an  list of "categorizationRequest" objects.
            categorizationRequestsRepositoryMock.SetupRepositoryFindAndProject<CategorizationRequest, Guid>(categorizationRequestList);
            // Mock the "Delete" method on the "Repository" and run the predicate lambda expression on
            // an  list of "categorizationRequest" objects.
            categorizationRequestsRepositoryMock.Setup(x => x.Delete(It.IsAny<CategorizationRequest>())).ReturnsAsync(true);
            #endregion

            #region [ ACT ]s

            // Call the "CleanupTransactions" method using the prepared request.
            int filesCount = await cleanupManager.CleanupCategorizations();

            #endregion

            #region [ ASSERT ]

            // The "CleanupTransactions" method should return filecount be 1
            filesCount.Should().Be(1);
            #endregion
        }
        #endregion

        #region <| ExpireCategorizations |>
        [Fact]
        public async Task CleanupManager_ExpireCategorizations_When_No_Expired_No_Cancel_Status_Should_Not_Delete_Files()
        {
            #region [ ARRANGE ]

            // Create mocks for all dependencies
            var preloadedFilesRepositoryMock = new Mock<IRepository<FilePreload>>();
            var transactionHistoryRepositoryMock = new Mock<IRepository<HistoricTransaction>>();
            var categorizationRequestsRepositoryMock = new Mock<IRepository<CategorizationRequest>>();
            var categorizationRequestMapper = new CategorizationRequestMapper();
            var batchRepositoryMock = new Mock<IRepository<Batch>>();
            var emailRepositoryMock = new Mock<IRepository<Email>>();
            var emailRecipientRepositoryMock = new Mock<IRepository<EmailRecipient>>();
            var fileRepositoryMock = new Mock<IRepository<File>>();
            var fileHelperMock = new Mock<IFileHelper>();
            var transactionHelperMock = new Mock<TransactionHelper>();
            var edreamsConfigurationMock = new Mock<IEdreamsConfiguration>();

            // Create an instance of the "CleanupManager" using the mocked dependencies.
            ICleanupManager cleanupManager = new CleanupManager(
                preloadedFilesRepositoryMock.Object,
                transactionHistoryRepositoryMock.Object,
                categorizationRequestsRepositoryMock.Object,
                batchRepositoryMock.Object,
                emailRepositoryMock.Object,
                emailRecipientRepositoryMock.Object,
                fileRepositoryMock.Object,
                fileHelperMock.Object,
                transactionHelperMock.Object,
                edreamsConfigurationMock.Object);

            // Prepare a request to use for when calling the "ExpireCategorizations" method.
            CategorizationRequest categorizationRequest = new CategorizationRequest()
            {
                EmailAddress = "edreamstest@deloitte.com",
                Id = new Guid("2D1EE7EC-6E7D-46DA-B2DD-9EC6F0C66653"),
                InternetMessageId = "<b267a11ab00348e3aa77bd20e20e1060>",
                Status = Enums.CategorizationRequestStatus.Expired,
                InsertedOn = DateTime.Now.AddDays(-1)
            };
            IList<CategorizationRequest> categorizationRequestList = new List<CategorizationRequest>() { categorizationRequest };
            #endregion

            #region [ MOCK ]
            
            edreamsConfigurationMock.Setup(x => x.PreloadedFilesExpiryInMinutes).Returns(10);
            // Mock the "Find" method on the "Repository" and run the predicate lambda expression on
            // an  list of "CategorizationRequest" objects.
            categorizationRequestsRepositoryMock.SetupRepositoryFind(categorizationRequestList);
            #endregion

            #region [ ACT ]s

            // Call the "ExpireCategorizations" method using the prepared request.
            int filesCount = await cleanupManager.ExpireCategorizations();

            #endregion

            #region [ ASSERT ]

            // The "ExpireCategorizations" method should return files count 0.
            filesCount.Should().Be(0);
            #endregion
        }
        [Fact]
        public async Task CleanupManager_ExpireCategorizations_When_Expired_Cancel_Status_Should_Delete_Files()
        {
            #region [ ARRANGE ]

            // Create mocks for all dependencies
            var preloadedFilesRepositoryMock = new Mock<IRepository<FilePreload>>();
            var transactionHistoryRepositoryMock = new Mock<IRepository<HistoricTransaction>>();
            var categorizationRequestsRepositoryMock = new Mock<IRepository<CategorizationRequest>>();
            var categorizationRequestMapper = new CategorizationRequestMapper();
            var batchRepositoryMock = new Mock<IRepository<Batch>>();
            var emailRepositoryMock = new Mock<IRepository<Email>>();
            var emailRecipientRepositoryMock = new Mock<IRepository<EmailRecipient>>();
            var fileRepositoryMock = new Mock<IRepository<File>>();
            var fileHelperMock = new Mock<IFileHelper>();
            var transactionHelperMock = new Mock<TransactionHelper>();
            var edreamsConfigurationMock = new Mock<IEdreamsConfiguration>();

            // Create an instance of the "CleanupManager" using the mocked dependencies.
            ICleanupManager cleanupManager = new CleanupManager(
                preloadedFilesRepositoryMock.Object,
                transactionHistoryRepositoryMock.Object,
                categorizationRequestsRepositoryMock.Object,
                batchRepositoryMock.Object,
                emailRepositoryMock.Object,
                emailRecipientRepositoryMock.Object,
                fileRepositoryMock.Object,
                fileHelperMock.Object,
                transactionHelperMock.Object,
                edreamsConfigurationMock.Object);

            // Prepare a request to use for when calling the "ExpireCategorizations" method.
            CategorizationRequest categorizationRequest = new CategorizationRequest()
            {
                EmailAddress = "edreamstest@deloitte.com",
                Id = new Guid("2D1EE7EC-6E7D-46DA-B2DD-9EC6F0C66653"),
                InternetMessageId = "<b267a11ab00348e3aa77bd20e20e1060>",
                Status = Enums.CategorizationRequestStatus.Pending,
                InsertedOn = DateTime.Now.AddDays(-1)
            };
            IList<CategorizationRequest> categorizationRequestList = new List<CategorizationRequest>() { categorizationRequest };
            #endregion

            #region [ MOCK ]
            // Mock teh configuration to set up PreloadedFilesExpiryInMinutes to 10 min
            edreamsConfigurationMock.Setup(x => x.PreloadedFilesExpiryInMinutes).Returns(10);
            // Mock the "Find" method on the "Repository" and run the predicate lambda expression on
            // an  list of "categorizationRequest" objects.
            categorizationRequestsRepositoryMock.SetupRepositoryFind(categorizationRequestList);
            #endregion

            #region [ ACT ]s

            // Call the "ExpireCategorizations" method using the prepared request.
            int filesCount = await cleanupManager.ExpireCategorizations();

            #endregion

            #region [ ASSERT ]

            //The "ExpireCategorizations" method should return files count be 1.
            filesCount.Should().Be(1);
            #endregion
        }
        #endregion
    }
}
