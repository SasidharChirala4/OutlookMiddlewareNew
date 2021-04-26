using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Edreams.Common.DataAccess.Interfaces;
using Edreams.Common.Exceptions.Factories.Interfaces;
using Edreams.OutlookMiddleware.BusinessLogic;
using Edreams.OutlookMiddleware.BusinessLogic.Factories.Interfaces;
using Edreams.OutlookMiddleware.BusinessLogic.Interfaces;
using Edreams.OutlookMiddleware.BusinessLogic.Transactions.Interfaces;
using Edreams.OutlookMiddleware.Common.Validation.Interface;
using Edreams.OutlookMiddleware.DataTransferObjects.Api;
using Edreams.OutlookMiddleware.Enums;
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
            var batchFactoryMock = new Mock<IBatchFactory>();
            var emailsToEmailDetailsMapperMock = new Mock<IEmailsToEmailDetailsMapper>();
            var preloadedFilesToFilesMapperMock = new Mock<IPreloadedFilesToFilesMapper>();
            var transactionHelperMock = new Mock<ITransactionHelper>();
            var validatorMock = new Mock<IValidator>();
            var exceptionFactoryMock = new Mock<IExceptionFactory>();

            // Create an instance of the "Subject Under Test" using the mocked dependencies.
            IBatchManager batchManager = new BatchManager(
                preloadedFilesRepositoryMock.Object, batchRepositoryMock.Object, emailRepositoryMock.Object,
                fileRepositoryMock.Object, batchFactoryMock.Object,
                emailsToEmailDetailsMapperMock.Object, preloadedFilesToFilesMapperMock.Object,
                transactionHelperMock.Object, validatorMock.Object, exceptionFactoryMock.Object);

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
            var batchFactoryMock = new Mock<IBatchFactory>();
            var emailsToEmailDetailsMapperMock = new Mock<IEmailsToEmailDetailsMapper>();
            var preloadedFilesToFilesMapperMock = new Mock<IPreloadedFilesToFilesMapper>();
            var transactionHelperMock = new Mock<ITransactionHelper>();
            var validatorMock = new Mock<IValidator>();
            var exceptionFactoryMock = new Mock<IExceptionFactory>();

            // Create an instance of the "Subject Under Test" using the mocked dependencies.
            IBatchManager batchManager = new BatchManager(
                preloadedFilesRepositoryMock.Object, batchRepositoryMock.Object, emailRepositoryMock.Object,
                fileRepositoryMock.Object, batchFactoryMock.Object,
                emailsToEmailDetailsMapperMock.Object, preloadedFilesToFilesMapperMock.Object,
                transactionHelperMock.Object, validatorMock.Object, exceptionFactoryMock.Object);

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
            var batchFactoryMock = new Mock<IBatchFactory>();
            var emailsToEmailDetailsMapperMock = new Mock<IEmailsToEmailDetailsMapper>();
            var preloadedFilesToFilesMapperMock = new Mock<IPreloadedFilesToFilesMapper>();
            var transactionHelperMock = new Mock<ITransactionHelper>();
            var validatorMock = new Mock<IValidator>();
            var exceptionFactoryMock = new Mock<IExceptionFactory>();

            // Create an instance of the "Subject Under Test" using the mocked dependencies.
            IBatchManager batchManager = new BatchManager(
                preloadedFilesRepositoryMock.Object, batchRepositoryMock.Object, emailRepositoryMock.Object,
                fileRepositoryMock.Object, batchFactoryMock.Object,
                emailsToEmailDetailsMapperMock.Object, preloadedFilesToFilesMapperMock.Object,
                transactionHelperMock.Object, validatorMock.Object, exceptionFactoryMock.Object);

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
            response.CorrelationId.Should().Be(request.CorrelationId);

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