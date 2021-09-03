using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Edreams.Common.DataAccess.Interfaces;
using Edreams.Common.Exceptions.Factories.Interfaces;
using Edreams.Common.Security.Interfaces;
using Edreams.OutlookMiddleware.BusinessLogic;
using Edreams.OutlookMiddleware.BusinessLogic.Factories.Interfaces;
using Edreams.OutlookMiddleware.BusinessLogic.Interfaces;
using Edreams.OutlookMiddleware.BusinessLogic.Transactions;
using Edreams.OutlookMiddleware.BusinessLogic.Transactions.Interfaces;
using Edreams.OutlookMiddleware.Common.Validation.Interface;
using Edreams.OutlookMiddleware.DataTransferObjects.Api;
using Edreams.OutlookMiddleware.Enums;
using Edreams.OutlookMiddleware.Mapping.Custom;
using Edreams.OutlookMiddleware.Mapping.Custom.Interfaces;
using Edreams.OutlookMiddleware.Model;
using Edreams.OutlookMiddleware.Tests.Framework.Extensions;
using FluentAssertions;
using Moq;
using Xunit;

namespace Edreams.OutlookMiddleware.Tests.BusinessLogic
{
    public class BatchManagerTests
    {
        #region <| CommitBatch |>

        [Fact]
        public async Task BatchManager_CommitBatch_Without_PreloadedFiles_Should_Return_Null()
        {
            #region [ ARRANGE ]

            // Create mocks for all dependencies
            var preloadedFilesRepositoryMock = new Mock<IRepository<FilePreload>>();
            var batchRepositoryMock = new Mock<IRepository<Batch>>();
            var emailRepositoryMock = new Mock<IRepository<Email>>();
            var fileRepositoryMock = new Mock<IRepository<File>>();
            var projectTaskRepositoryMock = new Mock<IRepository<ProjectTask>>();
            var batchFactoryMock = new Mock<IBatchFactory>();
            var emailsToEmailDetailsMapperMock = new Mock<IEmailsToEmailDetailsMapper>();
            var preloadedFilesToFilesMapperMock = new Mock<IPreloadedFilesToFilesMapper>();
            var projectTaskDetailsDtoToProjectTaskMapperMock = new Mock<IProjectTaskDetailsDtoToProjectTaskMapper>();
            var transactionHelperMock = new Mock<ITransactionHelper>();
            var validatorMock = new Mock<IValidator>();
            var exceptionFactoryMock = new Mock<IExceptionFactory>();
            var securityContextMock = new Mock<ISecurityContext>();

            // Create an instance of the "Subject Under Test" using the mocked dependencies.
            IBatchManager batchManager = new BatchManager(
                preloadedFilesRepositoryMock.Object, batchRepositoryMock.Object, emailRepositoryMock.Object,
                fileRepositoryMock.Object, projectTaskRepositoryMock.Object, batchFactoryMock.Object,
                emailsToEmailDetailsMapperMock.Object, preloadedFilesToFilesMapperMock.Object, projectTaskDetailsDtoToProjectTaskMapperMock.Object,
                transactionHelperMock.Object, validatorMock.Object, exceptionFactoryMock.Object, securityContextMock.Object);

            // Prepare a request to use for when calling the "CommitBatch" method.
            CommitBatchRequest request = new CommitBatchRequest
            {
                BatchId = Guid.NewGuid(),
                CorrelationId = Guid.NewGuid()
            };

            #endregion

            #region [ MOCK ]

            // Mock the "Find" method on the "Repository" and run the predicate lambda expression on
            // an empty list of "FilePreload" objects.
            preloadedFilesRepositoryMock.SetupRepositoryFind(new List<FilePreload>());

            #endregion

            #region [ ACT ]

            // Call the "CommitBatch" method using the prepared request.
            CommitBatchResponse response = await batchManager.CommitBatch(request.BatchId, request);

            #endregion

            #region [ ASSERT ]

            // If there are no "FilePreload" objects found, the "CommitBatch" method should return "null".
            response.Should().BeNull();

            #endregion
        }

        [Fact]
        public async Task BatchManager_CommitBatch_With_PreloadedFiles_Should_Change_Their_Status_To_Committed()
        {
            #region [ ARRANGE ]

            // Create mocks for all dependencies
            var preloadedFilesRepositoryMock = new Mock<IRepository<FilePreload>>();
            var batchRepositoryMock = new Mock<IRepository<Batch>>();
            var emailRepositoryMock = new Mock<IRepository<Email>>();
            var fileRepositoryMock = new Mock<IRepository<File>>();
            var projectTaskRepositoryMock = new Mock<IRepository<ProjectTask>>();
            var batchFactoryMock = new Mock<IBatchFactory>();
            var emailsToEmailDetailsMapperMock = new Mock<IEmailsToEmailDetailsMapper>();
            var preloadedFilesToFilesMapperMock = new Mock<IPreloadedFilesToFilesMapper>();
            var projectTaskDetailsDtoToProjectTaskMapperMock = new Mock<IProjectTaskDetailsDtoToProjectTaskMapper>();
            var transactionHelperMock = new Mock<ITransactionHelper>();
            var validatorMock = new Mock<IValidator>();
            var exceptionFactoryMock = new Mock<IExceptionFactory>();
            var securityContextMock = new Mock<ISecurityContext>();

            // Create an instance of the "Subject Under Test" using the mocked dependencies.
            IBatchManager batchManager = new BatchManager(
                preloadedFilesRepositoryMock.Object, batchRepositoryMock.Object, emailRepositoryMock.Object,
                fileRepositoryMock.Object, projectTaskRepositoryMock.Object, batchFactoryMock.Object,
                emailsToEmailDetailsMapperMock.Object, preloadedFilesToFilesMapperMock.Object, projectTaskDetailsDtoToProjectTaskMapperMock.Object,
                transactionHelperMock.Object, validatorMock.Object, exceptionFactoryMock.Object, securityContextMock.Object);

            Guid correlationId = Guid.NewGuid();

            Guid batchId = new Guid("8A3CAEB3-6906-4A9E-9125-332A03C867E5");

            FilePreload preloadedFile = new FilePreload
            {
                Id = new Guid("641A1ECC-ACA4-4331-A6A2-8F988619DAE4"),
                BatchId = batchId,
                EmailId = new Guid("42B61BE1-36A4-45F4-BD40-12A7A7261F37"),
                EntryId = "00000000ADFDDFFDE22D5B439FDA2190B01E51050700EEDC75E0CC0152438DE5E9B653C1B71300000000010C0000EEDC75E0CC0152438DE5E9B653C1B713000187D126950000",
                EwsId = "AAMkADZlZmU5MmRhLWVlOTUtNGU3Yy04OTMxLWU2ZmExNWY2MDZkYwBGAAAAAACt/d/94i1bQ5/aIZCwHlEFBwDu3HXgzAFSQ43l6bZTwbcTAAAAAAEMAADu3HXgzAFSQ43l6bZTwbcTAAGH0SaVAAA=",
                EmailKind = EmailKind.Received,
                EdreamsReferenceId = new Guid("656B921D-2410-48E4-B1C8-89E9C56F24CF"),
                EmailSubject = "Test Outlook Plugin",
                AttachmentId = "AAMkAGNhNjU5OTRjLWFhNzItNDViMS1iZjQ4LTkzMWQzZmU5M2FiNgBGAAAAAADYEcvAfM1cSpiloDzK5d70BwAy9fJ8HnimT75y+TYqY8hdAAAAAAEJAAAy9fJ8HnimT75y+TYqY8hdAADK4QxIAAABEgAQANA6d3sygGVAg4V94uPH0mI=",
                FileName = "TestSampleFile",
                Kind = FileKind.Attachment,
            };
            List<FilePreload> preloadedFiles = new List<FilePreload>
            {
                preloadedFile
            };

            // Prepare a request to use for when calling the "CommitBatch" method.
            CommitBatchRequest request = new CommitBatchRequest
            {
                BatchId = batchId,
                UploadOption = EmailUploadOptions.Attachments,
                UploadLocationFolder = "https://edreams4-t.be.deloitte.com/Sites/3qlzmskx/3qm18475/AllDocuments/Employment%20Law",
                UploadLocationSite = "https://edreams4-t.be.deloitte.com/Sites/3qlzmskx/3qm18475",
                CorrelationId = Guid.NewGuid(),
            };
            Batch batch = new Batch()
            {
                Id = new Guid("DF110E66-CED5-4122-A752-740896272B90"),
                CreatedOn = DateTime.UtcNow,
                CreatedBy = "TestUser",
                Status = BatchStatus.Pending

            };
            File file = new File()
            {
                Id = new Guid("943ACE66-3C08-445C-975A-CBD08C7015EA"),
                EmailSubject = "Test Subject",
                AttachmentId = "AAMkAGNhNjU5OTRjLWFhNzItNDViMS1iZjQ4LTkzMWQzZmU5M2FiNgBGAAAAAADYEcvAfM1cSpiloDzK5d70BwAy9fJ8HnimT75y+TYqY8hdAAAAAAEJAAAy9fJ8HnimT75y+TYqY8hdAADK4QxIAAABEgAQANA6d3sygGVAg4V94uPH0mI=",
                Kind = FileKind.Attachment,
                NewName = "Test NewName",
                OriginalName = "Old Name",
                InsertedBy = "Bkonijeti",
                InsertedOn = DateTime.UtcNow,
                ShouldUpload = false
            };
            List<File> files = new List<File>() {
            file
            };

            Mock<TransactionScope> transactionScopeMock = new Mock<TransactionScope>();

            #endregion

            #region [ MOCK ]

            // Mock the "Find" method on the "Repository" and run the predicate lambda expression on
            // an empty list of "FilePreload" objects.
            preloadedFilesRepositoryMock.SetupRepositoryFind(preloadedFiles);
            batchRepositoryMock.Setup(x => x.Create(It.IsAny<Batch>())).ReturnsAsync(batch);
            batchFactoryMock.Setup(x => x.CreatePendingBatch()).Returns(batch);
            preloadedFilesToFilesMapperMock.Setup(x => x.Map(batch, preloadedFiles, request)).Returns(files);
            fileRepositoryMock.Setup(x => x.Create(It.IsAny<File>())).ReturnsAsync(file);
            preloadedFilesRepositoryMock.Setup(x => x.Update(It.IsAny<FilePreload>())).ReturnsAsync(preloadedFile);
            // Mock the Transaction helper to mock the createscope method and returns mock trasaction scope object
            transactionHelperMock.Setup(x => x.CreateScope()).Returns(transactionScopeMock.Object);
            securityContextMock.SetupCorrelationId(correlationId);


            #endregion

            #region [ ACT ]

            // Call the "CommitBatch" method using the prepared request.
            CommitBatchResponse response = await batchManager.CommitBatch(request.BatchId, request);

            #endregion

            #region [ ASSERT ]

            // The "BatchId" in the response should be equal to the "BatchId" in the response.
            response.BatchId.Should().Be(batch.Id);

            // The "CorrelationId" in the response should be equal to the "CorrelationId" in the response.
            response.CorrelationId.Should().Be(correlationId);

            // The "NumberOfCancelledFiles" should equal "1".
            response.NumberOfFiles.Should().Be(1);

            // The "FilePreload" objects should be updated by calling the "Update" method on the repository.
            preloadedFilesRepositoryMock.VerifyRepositoryUpdate(Times.Once());



            #endregion
        }

        #endregion

        #region <| CancelBatch |>

        [Fact]
        public async Task BatchManager_CancelBatch_Without_PreloadedFiles_Should_Return_Null()
        {
            #region [ ARRANGE ]

            // Create mocks for all dependencies
            var preloadedFilesRepositoryMock = new Mock<IRepository<FilePreload>>();
            var batchRepositoryMock = new Mock<IRepository<Batch>>();
            var emailRepositoryMock = new Mock<IRepository<Email>>();
            var fileRepositoryMock = new Mock<IRepository<File>>();
            var projectTaskRepositoryMock = new Mock<IRepository<ProjectTask>>();
            var batchFactoryMock = new Mock<IBatchFactory>();
            var emailsToEmailDetailsMapperMock = new Mock<IEmailsToEmailDetailsMapper>();
            var preloadedFilesToFilesMapperMock = new Mock<IPreloadedFilesToFilesMapper>();
            var projectTaskDetailsDtoToProjectTaskMapperMock = new Mock<IProjectTaskDetailsDtoToProjectTaskMapper>();
            var transactionHelperMock = new Mock<ITransactionHelper>();
            var validatorMock = new Mock<IValidator>();
            var exceptionFactoryMock = new Mock<IExceptionFactory>();
            var securityContextMock = new Mock<ISecurityContext>();

            // Create an instance of the "Subject Under Test" using the mocked dependencies.
            IBatchManager batchManager = new BatchManager(
                preloadedFilesRepositoryMock.Object, batchRepositoryMock.Object, emailRepositoryMock.Object,
                fileRepositoryMock.Object, projectTaskRepositoryMock.Object, batchFactoryMock.Object,
                emailsToEmailDetailsMapperMock.Object, preloadedFilesToFilesMapperMock.Object, projectTaskDetailsDtoToProjectTaskMapperMock.Object,
                transactionHelperMock.Object, validatorMock.Object, exceptionFactoryMock.Object, securityContextMock.Object);

            // Prepare a request to use for when calling the "CancelBatch" method.
            CancelBatchRequest request = new CancelBatchRequest
            {
                BatchId = Guid.NewGuid(),
                CorrelationId = Guid.NewGuid()
            };

            #endregion

            #region [ MOCK ]

            // Mock the "Find" method on the "Repository" and run the predicate lambda expression on
            // an empty list of "FilePreload" objects.
            preloadedFilesRepositoryMock.SetupRepositoryFind(new List<FilePreload>());

            #endregion

            #region [ ACT ]

            // Call the "CancelBatch" method using the prepared request.
            CancelBatchResponse response = await batchManager.CancelBatch(request.BatchId, request);

            #endregion

            #region [ ASSERT ]

            // If there are no "FilePreload" objects found, the "CancelBatch" method should return "null".
            response.Should().BeNull();

            #endregion
        }

        [Fact]
        public async Task BatchManager_CancelBatch_With_PreloadedFiles_Should_Change_Their_Status_To_Cancelled()
        {
            #region [ ARRANGE ]

            // Create mocks for all dependencies
            var preloadedFilesRepositoryMock = new Mock<IRepository<FilePreload>>();
            var batchRepositoryMock = new Mock<IRepository<Batch>>();
            var emailRepositoryMock = new Mock<IRepository<Email>>();
            var fileRepositoryMock = new Mock<IRepository<File>>();
            var projectTaskRepositoryMock = new Mock<IRepository<ProjectTask>>();
            var batchFactoryMock = new Mock<IBatchFactory>();
            var emailsToEmailDetailsMapperMock = new Mock<IEmailsToEmailDetailsMapper>();
            var preloadedFilesToFilesMapperMock = new Mock<IPreloadedFilesToFilesMapper>();
            var projectTaskDetailsDtoToProjectTaskMapperMock = new Mock<IProjectTaskDetailsDtoToProjectTaskMapper>();
            var transactionHelperMock = new Mock<ITransactionHelper>();
            var validatorMock = new Mock<IValidator>();
            var exceptionFactoryMock = new Mock<IExceptionFactory>();
            var securityContextMock = new Mock<ISecurityContext>();

            // Create an instance of the "Subject Under Test" using the mocked dependencies.
            IBatchManager batchManager = new BatchManager(
                preloadedFilesRepositoryMock.Object, batchRepositoryMock.Object, emailRepositoryMock.Object,
                fileRepositoryMock.Object, projectTaskRepositoryMock.Object, batchFactoryMock.Object,
                emailsToEmailDetailsMapperMock.Object, preloadedFilesToFilesMapperMock.Object, projectTaskDetailsDtoToProjectTaskMapperMock.Object,
                transactionHelperMock.Object, validatorMock.Object, exceptionFactoryMock.Object, securityContextMock.Object);

            // Generate a unique id to use for batches.
            Guid batchId = Guid.NewGuid();

            // Prepare a fake database containing "FilePreload" objects.
            // The first and third file will be linked to an important batch,
            // the second file will be linked to another random batch.
            List<FilePreload> preloadedFiles = new List<FilePreload>
            {
                new FilePreload { FileName = "FILE_1", BatchId = batchId },
                new FilePreload { FileName = "FILE_2", BatchId = Guid.NewGuid() },
                new FilePreload { FileName = "FILE_3", BatchId = batchId }
            };

            Guid correlationId = Guid.NewGuid();

            // Prepare a request to use for when calling the "CancelBatch" method.
            CancelBatchRequest request = new CancelBatchRequest
            {
                BatchId = batchId,
                CorrelationId = Guid.NewGuid()
            };

            #endregion

            #region [ MOCK ]

            // Mock the "Find" method on the "Repository" and run the predicate lambda expression on
            // the prepared fake database of "FilePreload" objects.
            preloadedFilesRepositoryMock.SetupRepositoryFind(preloadedFiles);
            securityContextMock.SetupCorrelationId(correlationId);

            #endregion

            #region [ ACT ]

            // Call the "CancelBatch" method using the prepared request.
            CancelBatchResponse response = await batchManager.CancelBatch(request.BatchId, request);

            #endregion

            #region [ ASSERT ]

            // The "CancelBatch" method should not return "null" because
            // there are "FilePreload" objects for the prepared batch.
            response.Should().NotBeNull();

            // The "BatchId" in the response should be equal to the "BatchId" in the response.
            response.BatchId.Should().Be(batchId);

            // The "CorrelationId" in the response should be equal to the "CorrelationId" in the response.
            response.CorrelationId.Should().Be(correlationId);

            // The "NumberOfCancelledFiles" should equal "2".
            response.NumberOfCancelledFiles.Should().Be(2);

            // The "Status" for "FilePreload" objects should be changed for
            // "FILE_1" and "FILE_3" because they were linked to the prepared batch.
            preloadedFiles.Single(x => x.FileName == "FILE_1").Status.Should().Be(EmailPreloadStatus.Cancelled);
            preloadedFiles.Single(x => x.FileName == "FILE_2").Status.Should().Be(EmailPreloadStatus.Pending);
            preloadedFiles.Single(x => x.FileName == "FILE_3").Status.Should().Be(EmailPreloadStatus.Cancelled);

            // The "FilePreload" objects should be updated by calling the "Update" method on the repository.
            preloadedFilesRepositoryMock.VerifyRepositoryUpdate(Times.Once());

            #endregion
        }

        #endregion
    }
}