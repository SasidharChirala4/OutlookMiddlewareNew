using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Edreams.Common.AzureServiceBus.Contracts;
using Edreams.Common.AzureServiceBus.Interfaces;
using Edreams.Common.Exceptions;
using Edreams.Common.Exceptions.Factories;
using Edreams.OutlookMiddleware.BusinessLogic;
using Edreams.OutlookMiddleware.BusinessLogic.Interfaces;
using Edreams.OutlookMiddleware.BusinessLogic.Transactions.Interfaces;
using Edreams.OutlookMiddleware.Common.Configuration.Interfaces;
using Edreams.OutlookMiddleware.Common.Security.Interfaces;
using Edreams.OutlookMiddleware.DataAccess.Repositories.Interfaces;
using Edreams.OutlookMiddleware.DataTransferObjects;
using Edreams.OutlookMiddleware.DataTransferObjects.Api;
using Edreams.OutlookMiddleware.Enums;
using Edreams.OutlookMiddleware.Mapping;
using Edreams.OutlookMiddleware.Model;
using Edreams.OutlookMiddleware.Tests.Framework.Extensions;
using FluentAssertions;
using Moq;
using Xunit;
using TransactionScope = Edreams.OutlookMiddleware.BusinessLogic.Transactions.TransactionScope;
namespace Edreams.OutlookMiddleware.Tests.BusinessLogic
{
    public class TransactionQueueManagerTests
    {
        #region <| IsTransactionAvailable |>
        [Fact]
        public async Task TransactionQueueManager_IsTransactionAvailable_ShouldNot_Return_Transaction()
        {
            #region [ ARRANGE ]

            // Create mocks for all dependencies
            var transactionHelperMock = new Mock<ITransactionHelper>();
            var transactionRepositoryMock = new Mock<IRepository<Transaction>>();
            var historicTransactionRepositoryMock = new Mock<IRepository<HistoricTransaction>>();
            var _transactionMapper = new TransactionToTransactionDtoMapper();
            var _historicTransactionMapper = new TransactionToHistoricTransactionMapper();
            var _serviceBusHandlerMock = new Mock<IServiceBusHandler>();
            var _configurationMock = new Mock<IEdreamsConfiguration>();
            var _securityContextMock = new Mock<ISecurityContext>();
            var _exceptionFactory = new ExceptionFactory();

            // Create an instance of the "TransactionQueueManager" using the mocked dependencies.
            ITransactionQueueManager transactionQueueManager = new TransactionQueueManager(
                transactionHelperMock.Object, transactionRepositoryMock.Object,historicTransactionRepositoryMock.Object,
                _transactionMapper,_historicTransactionMapper, _serviceBusHandlerMock.Object, _configurationMock.Object,
                _securityContextMock.Object, _exceptionFactory);

            // Prepare a request to use for when calling the "IsTransactionAvailable" method.
            Guid transactionId = new Guid("46b7722d-f317-4a1d-9e69-d7592ad79c99");
            Transaction transaction = new Transaction
            {
                Id = new Guid("925b4ad2-d815-48a8-8f6f-b4b951b9c5d8"),
                BatchId = new Guid("46f61a38-ac53-46fe-9de3-ba80c5df5a92"),
                CorrelationId = Guid.NewGuid()
            };
            List<Transaction> transactions = new List<Transaction>() { transaction};
            #endregion

            #region [ MOCK ]

            // Mock the "Exists" method on the "Repository" and run the predicate lambda expression on
            // an empty list of "transaction" objects.
            transactionRepositoryMock.SetupRepositoryExists(transactions);

            #endregion

            #region [ ACT ]

            // Call the "IsTransactionAvailable" method using the prepared request.
            bool response = await transactionQueueManager.IsTransactionAvailable(transactionId);

            #endregion

            #region [ ASSERT ]

            // If there are no "Transaction" objects found, the "IsTransactionAvailable" method should return "false".
            response.Should().Be(false);

            #endregion
        }

        [Fact]
        public async Task TransactionQueueManager_IsTransactionAvailable_Should_Return_Transaction()
        {
            #region [ ARRANGE ]

            // Create mocks for all dependencies
            var transactionHelperMock = new Mock<ITransactionHelper>();
            var transactionRepositoryMock = new Mock<IRepository<Transaction>>();
            var historicTransactionRepositoryMock = new Mock<IRepository<HistoricTransaction>>();
            var _transactionMapper = new TransactionToTransactionDtoMapper();
            var _historicTransactionMapper = new TransactionToHistoricTransactionMapper();
            var _serviceBusHandlerMock = new Mock<IServiceBusHandler>();
            var _configurationMock = new Mock<IEdreamsConfiguration>();
            var _securityContextMock = new Mock<ISecurityContext>();
            var _exceptionFactory = new ExceptionFactory();

            // Create an instance of the "TransactionQueueManager" using the mocked dependencies.
            ITransactionQueueManager transactionQueueManager = new TransactionQueueManager(
                transactionHelperMock.Object, transactionRepositoryMock.Object, historicTransactionRepositoryMock.Object,
                _transactionMapper, _historicTransactionMapper, _serviceBusHandlerMock.Object, _configurationMock.Object,
                _securityContextMock.Object, _exceptionFactory);

            // Prepare a request to use for when calling the "IsTransactionAvailable" method.
            Guid transactionId = new Guid("46b7722d-f317-4a1d-9e69-d7592ad79c99");
            Transaction transaction = new Transaction
            {
                Id = new Guid("46b7722d-f317-4a1d-9e69-d7592ad79c99"),
                BatchId = new Guid("46f61a38-ac53-46fe-9de3-ba80c5df5a92"),
                CorrelationId = Guid.NewGuid()
            };
            List<Transaction> transactions = new List<Transaction>() { transaction };
            #endregion

            #region [ MOCK ]

            // Mock the "Exists" method on the "Repository" and run the predicate lambda expression on
            // an empty list of "transaction" objects.
            transactionRepositoryMock.SetupRepositoryExists(transactions);

            #endregion

            #region [ ACT ]

            // Call the "IsTransactionAvailable" method using the prepared request.
            bool response = await transactionQueueManager.IsTransactionAvailable(transactionId);

            #endregion

            #region [ ASSERT ]

            // If there are  "Transaction" objects found, the "IsTransactionAvailable" method should return "true".
            response.Should().Be(true);

            #endregion
        }
        #endregion

        #region <| GetTransaction |>

        [Fact]
        public async Task TransactionQueueManager_GetTransaction_ShouldNot_Return_Transaction()
        {
            #region [ ARRANGE ]

            // Create mocks for all dependencies
            var transactionHelperMock = new Mock<ITransactionHelper>();
            var transactionRepositoryMock = new Mock<IRepository<Transaction>>();
            var historicTransactionRepositoryMock = new Mock<IRepository<HistoricTransaction>>();
            var _transactionMapper = new TransactionToTransactionDtoMapper();
            var _historicTransactionMapper = new TransactionToHistoricTransactionMapper();
            var _serviceBusHandlerMock = new Mock<IServiceBusHandler>();
            var _configurationMock = new Mock<IEdreamsConfiguration>();
            var _securityContextMock = new Mock<ISecurityContext>();
            var _exceptionFactory = new ExceptionFactory();

            // Create an instance of the "TransactionQueueManager" using the mocked dependencies.
            ITransactionQueueManager transactionQueueManager = new TransactionQueueManager(
                transactionHelperMock.Object, transactionRepositoryMock.Object, historicTransactionRepositoryMock.Object,
                _transactionMapper, _historicTransactionMapper, _serviceBusHandlerMock.Object, _configurationMock.Object,
                _securityContextMock.Object, _exceptionFactory);

            // Prepare a request to use for when calling the "GetTransaction" method.
            Guid transactionId = new Guid("46b7722d-f317-4a1d-9e69-d7592ad79c99");
            Transaction transaction = new Transaction
            {
                Id = new Guid("925b4ad2-d815-48a8-8f6f-b4b951b9c5d8"),
                BatchId = new Guid("46f61a38-ac53-46fe-9de3-ba80c5df5a92"),
                CorrelationId = Guid.NewGuid()
            };
            List<Transaction> transactions = new List<Transaction>() { transaction };
            #endregion

            #region [ MOCK ]

            // Mock the "Find" method on the "Repository" and run the predicate lambda expression on
            // an empty list of "FilePreload" objects.
            transactionRepositoryMock.SetupRepositoryGetSingle(transactions);

            #endregion

            #region [ ACT ]

            // Call the "GetTransaction" method using the prepared request.
            TransactionDto response = await transactionQueueManager.GetTransaction(transactionId);

            #endregion

            #region [ ASSERT ]

            // If there are no "Transaction" objects found, the "GetTransaction" method should return "null".
            response.Should().BeNull();

            #endregion
        }

        [Fact]
        public async Task TransactionQueueManager_GetTransaction_Should_Return_Transaction()
        {
            #region [ ARRANGE ]

            // Create mocks for all dependencies
            var transactionHelperMock = new Mock<ITransactionHelper>();
            var transactionRepositoryMock = new Mock<IRepository<Transaction>>();
            var historicTransactionRepositoryMock = new Mock<IRepository<HistoricTransaction>>();
            var _transactionMapper = new TransactionToTransactionDtoMapper();
            var _historicTransactionMapper = new TransactionToHistoricTransactionMapper();
            var _serviceBusHandlerMock = new Mock<IServiceBusHandler>();
            var _configurationMock = new Mock<IEdreamsConfiguration>();
            var _securityContextMock = new Mock<ISecurityContext>();
            var _exceptionFactory = new ExceptionFactory();

            // Create an instance of the "TransactionQueueManager" using the mocked dependencies.
            ITransactionQueueManager transactionQueueManager = new TransactionQueueManager(
                transactionHelperMock.Object, transactionRepositoryMock.Object, historicTransactionRepositoryMock.Object,
                _transactionMapper, _historicTransactionMapper, _serviceBusHandlerMock.Object, _configurationMock.Object,
                _securityContextMock.Object, _exceptionFactory);

            // Prepare a request to use for when calling the "GetTransaction" method.
            Guid transactionId = new Guid("46b7722d-f317-4a1d-9e69-d7592ad79c99");
            Transaction transaction = new Transaction
            {
                Id = new Guid("46b7722d-f317-4a1d-9e69-d7592ad79c99"),
                BatchId = new Guid("46f61a38-ac53-46fe-9de3-ba80c5df5a92"),
                CorrelationId = Guid.NewGuid()
            };
            List<Transaction> transactions = new List<Transaction>() { transaction };
            #endregion

            #region [ MOCK ]

            // Mock the "GetSingle" method on the "Repository" and run the predicate lambda expression on
            // an empty list of "Transaction" objects.
            transactionRepositoryMock.SetupRepositoryGetSingle(transactions);

            #endregion

            #region [ ACT ]

            // Call the "GetTransaction" method using the prepared request.
            TransactionDto response = await transactionQueueManager.GetTransaction(transactionId);

            #endregion

            #region [ ASSERT ]

            // If there are no "Transaction" objects found, the "GetTransaction" method should return "null".
            response.Should().NotBeNull();

            #endregion
        }
        #endregion

        #region <| GetTransactionStatus |>

        [Fact]
        public async Task TransactionQueueManager_GetTransactionStatus_ShouldNot_Return_Transaction()
        {
            #region [ ARRANGE ]

            // Create mocks for all dependencies
            var transactionHelperMock = new Mock<ITransactionHelper>();
            var transactionRepositoryMock = new Mock<IRepository<Transaction>>();
            var historicTransactionRepositoryMock = new Mock<IRepository<HistoricTransaction>>();
            var _transactionMapper = new TransactionToTransactionDtoMapper();
            var _historicTransactionMapper = new TransactionToHistoricTransactionMapper();
            var _serviceBusHandlerMock = new Mock<IServiceBusHandler>();
            var _configurationMock = new Mock<IEdreamsConfiguration>();
            var _securityContextMock = new Mock<ISecurityContext>();
            var _exceptionFactory = new ExceptionFactory();

            // Create an instance of the "TransactionQueueManager" using the mocked dependencies.
            ITransactionQueueManager transactionQueueManager = new TransactionQueueManager(
                transactionHelperMock.Object, transactionRepositoryMock.Object, historicTransactionRepositoryMock.Object,
                _transactionMapper, _historicTransactionMapper, _serviceBusHandlerMock.Object, _configurationMock.Object,
                _securityContextMock.Object, _exceptionFactory);

            // Prepare a request to use for when calling the "GetTransaction" method.
            Guid transactionId = new Guid("46b7722d-f317-4a1d-9e69-d7592ad79c99");
            Transaction transaction = new Transaction
            {
                Id = new Guid("925b4ad2-d815-48a8-8f6f-b4b951b9c5d8"),
                BatchId = new Guid("46f61a38-ac53-46fe-9de3-ba80c5df5a92"),
                CorrelationId = Guid.NewGuid(),
                Status = TransactionStatus.ProcessingStarted
            };
            List<Transaction> transactions = new List<Transaction>() { transaction };
            #endregion

            #region [ MOCK ]

            // Mock the "GetSingleAndProject" method on the "Repository" and run the predicate lambda expression on
            // an empty list of "Transaction" objects.
            transactionRepositoryMock.SetupRepositoryGetSingleAndProject<Transaction,TransactionStatus>(transactions);

            #endregion

            #region [ ACT ]

            // Call the "GetTransaction" method using the prepared request.
            TransactionStatus? response = await transactionQueueManager.GetTransactionStatus(transactionId);

            #endregion

            #region [ ASSERT ]

            // If there are no "Transaction" objects found, the "GetTransaction" method should return "null".
            response.Should().BeNull();

            #endregion
        }

        [Fact]
        public async Task TransactionQueueManager_GetTransactionStatus_Should_Return_Transaction()
        {
            #region [ ARRANGE ]

            // Create mocks for all dependencies
            var transactionHelperMock = new Mock<ITransactionHelper>();
            var transactionRepositoryMock = new Mock<IRepository<Transaction>>();
            var historicTransactionRepositoryMock = new Mock<IRepository<HistoricTransaction>>();
            var _transactionMapper = new TransactionToTransactionDtoMapper();
            var _historicTransactionMapper = new TransactionToHistoricTransactionMapper();
            var _serviceBusHandlerMock = new Mock<IServiceBusHandler>();
            var _configurationMock = new Mock<IEdreamsConfiguration>();
            var _securityContextMock = new Mock<ISecurityContext>();
            var _exceptionFactory = new ExceptionFactory();

            // Create an instance of the "TransactionQueueManager" using the mocked dependencies.
            ITransactionQueueManager transactionQueueManager = new TransactionQueueManager(
                transactionHelperMock.Object, transactionRepositoryMock.Object, historicTransactionRepositoryMock.Object,
                _transactionMapper, _historicTransactionMapper, _serviceBusHandlerMock.Object, _configurationMock.Object,
                _securityContextMock.Object, _exceptionFactory);

            // Prepare a request to use for when calling the "GetTransactionStatus" method.
            Guid transactionId = new Guid("46b7722d-f317-4a1d-9e69-d7592ad79c99");
            Transaction transaction = new Transaction
            {
                Id = new Guid("46b7722d-f317-4a1d-9e69-d7592ad79c99"),
                BatchId = new Guid("46f61a38-ac53-46fe-9de3-ba80c5df5a92"),
                CorrelationId = Guid.NewGuid(),
                Status = TransactionStatus.ProcessingStarted
            };
            List<Transaction> transactions = new List<Transaction>() { transaction };
            #endregion

            #region [ MOCK ]

            // Mock the "GetSingleAndProject" method on the "Repository" and run the predicate lambda expression on
            // an empty list of "transaction" objects.
            transactionRepositoryMock.SetupRepositoryGetSingleAndProject<Transaction, TransactionStatus?>(transactions);

            #endregion

            #region [ ACT ]

            // Call the "GetTransactionStatus" method using the prepared request.
            TransactionStatus? response = await transactionQueueManager.GetTransactionStatus(transactionId);

            #endregion

            #region [ ASSERT ]

            // If there are "Transaction" objects found, the "GetTransactionStatus" method should return transaction status object.
            response.Should().NotBeNull();
            response.Should().Be(TransactionStatus.ProcessingStarted);



            #endregion
        }
        #endregion

        #region <| GetNextUploadTransaction |>

        [Fact]
        public async Task TransactionQueueManager_GetNextUploadTransaction_Should_Not_Return_Transaction()
        {
            #region [ ARRANGE ]

            // Create mocks for all dependencies
            var transactionHelperMock = new Mock<ITransactionHelper>();
            var transactionRepositoryMock = new Mock<IRepository<Transaction>>();
            var historicTransactionRepositoryMock = new Mock<IRepository<HistoricTransaction>>();
            var _transactionMapper = new TransactionToTransactionDtoMapper();
            var _historicTransactionMapper = new TransactionToHistoricTransactionMapper();
            var _serviceBusHandlerMock = new Mock<IServiceBusHandler>();
            var _configurationMock = new Mock<IEdreamsConfiguration>();
            var _securityContextMock = new Mock<ISecurityContext>();
            var _exceptionFactory = new ExceptionFactory();

            // Create an instance of the "TransactionQueueManager" using the mocked dependencies.
            ITransactionQueueManager transactionQueueManager = new TransactionQueueManager(
                transactionHelperMock.Object, transactionRepositoryMock.Object, historicTransactionRepositoryMock.Object,
                _transactionMapper, _historicTransactionMapper, _serviceBusHandlerMock.Object, _configurationMock.Object,
                _securityContextMock.Object, _exceptionFactory);

            // Prepare a request to use for when calling the "GetNextUploadTransaction" method.
            Guid transactionId = new Guid("46b7722d-f317-4a1d-9e69-d7592ad79c99");
            Transaction transaction = new Transaction
            {
                Id = new Guid("925b4ad2-d815-48a8-8f6f-b4b951b9c5d8"),
                BatchId = new Guid("46f61a38-ac53-46fe-9de3-ba80c5df5a92"),
                CorrelationId = Guid.NewGuid(),
                Status = TransactionStatus.Retried,
                
            };
            List<Transaction> transactions = new List<Transaction>() { transaction };
            #endregion

            #region [ MOCK ]

            // Mock the "Find" method on the "Repository" and run the predicate lambda expression on
            // an empty list of "FilePreload" objects.
            transactionRepositoryMock.SetupRepositoryGetFirstAscending<Transaction, DateTime?>(transactions);

            #endregion

            #region [ ACT ]

            // Call the "GetNextUploadTransaction" method using the prepared request.
            TransactionDto? response = await transactionQueueManager.GetNextUploadTransaction();

            #endregion

            #region [ ASSERT ]

            // If there are no "NextUploadTransaction" objects found, the "GetNextUploadTransaction" method should return "null".
            response.Should().BeNull();

            #endregion
        }

        [Fact]
        public async Task TransactionQueueManager_GetNextUploadTransaction_Should_Return_Transaction()
        {
            #region [ ARRANGE ]

            // Create mocks for all dependencies
            var transactionHelperMock = new Mock<ITransactionHelper>();
            var transactionRepositoryMock = new Mock<IRepository<Transaction>>();
            var historicTransactionRepositoryMock = new Mock<IRepository<HistoricTransaction>>();
            var _transactionMapper = new TransactionToTransactionDtoMapper();
            var _historicTransactionMapper = new TransactionToHistoricTransactionMapper();
            var _serviceBusHandlerMock = new Mock<IServiceBusHandler>();
            var _configurationMock = new Mock<IEdreamsConfiguration>();
            var _securityContextMock = new Mock<ISecurityContext>();
            var _exceptionFactory = new ExceptionFactory();

            // Create an instance of the "TransactionQueueManager" using the mocked dependencies.
            ITransactionQueueManager transactionQueueManager = new TransactionQueueManager(
                transactionHelperMock.Object, transactionRepositoryMock.Object, historicTransactionRepositoryMock.Object,
                _transactionMapper, _historicTransactionMapper, _serviceBusHandlerMock.Object, _configurationMock.Object,
                _securityContextMock.Object, _exceptionFactory);

            // Prepare a request to use for when calling the "GetNextUploadTransaction" method.
            Transaction transaction = new Transaction
            {
                Id = new Guid("46b7722d-f317-4a1d-9e69-d7592ad79c99"),
                BatchId = new Guid("46f61a38-ac53-46fe-9de3-ba80c5df5a92"),
                CorrelationId = Guid.NewGuid(),
                Status = TransactionStatus.Queued,
                Type = TransactionType.Upload,
                ReleaseDate = DateTime.UtcNow.AddDays(-1)
            };
            List<Transaction> transactions = new List<Transaction>() { transaction };
            #endregion

            #region [ MOCK ]

            // Mock the "GetFirstAscending" method on the "Repository" and run the predicate lambda expression on
            // an empty list of "transaction" objects.
            transactionRepositoryMock.SetupRepositoryGetFirstAscending<Transaction, DateTime?>(transactions);

            #endregion

            #region [ ACT ]

            // Call the "GetNextUploadTransaction" method using the prepared request.
            TransactionDto response = await transactionQueueManager.GetNextUploadTransaction();

            #endregion

            #region [ ASSERT ]

            // If there are  "NextUploadTransaction" objects found, the "GetNextUploadTransaction" method should return TransactionDto object.
            response.Should().NotBeNull();

            #endregion
        }
        #endregion


        #region <| CreateUploadTransaction |>

        [Fact]
        public async Task TransactionQueueManager_CreateUploadTransaction_Should_Create_TransactionTypeUpload()
        {
            #region [ ARRANGE ]

            // Create mocks for all dependencies
            var transactionHelperMock = new Mock<ITransactionHelper>();
            var transactionRepositoryMock = new Mock<IRepository<Transaction>>();
            var historicTransactionRepositoryMock = new Mock<IRepository<HistoricTransaction>>();
            var _transactionMapper = new TransactionToTransactionDtoMapper();
            var _historicTransactionMapper = new TransactionToHistoricTransactionMapper();
            var _serviceBusHandlerMock = new Mock<IServiceBusHandler>();
            var _configurationMock = new Mock<IEdreamsConfiguration>();
            var _securityContextMock = new Mock<ISecurityContext>();
            var _exceptionFactory = new ExceptionFactory();

            // Create an instance of the "TransactionQueueManager" using the mocked dependencies.
            ITransactionQueueManager transactionQueueManager = new TransactionQueueManager(
                transactionHelperMock.Object, transactionRepositoryMock.Object, historicTransactionRepositoryMock.Object,
                _transactionMapper, _historicTransactionMapper, _serviceBusHandlerMock.Object, _configurationMock.Object,
                _securityContextMock.Object, _exceptionFactory);

            // Prepare a request to use for when calling the "CreateUploadTransaction" method.
            Guid batchId = new Guid("9FC3D8F8-2A11-4B39-86A8-BD73D7FA3316");
            #endregion


            #region [ ACT ]

            // Call the "CreateUploadTransaction" method using the prepared request.
            await transactionQueueManager.CreateUploadTransaction(batchId);

            #endregion

            #region [ ASSERT ]

            // If the upload transaction created then the transction repository create method should call once.
            transactionRepositoryMock.VerifyRepositoryCreateSingle(Times.Once());

            #endregion
        }
        #endregion

        #region <| CreateUploadTransaction |>

        [Fact]
        public async Task TransactionQueueManager_CreateCategorizationTransaction_Should_Create_TransactionTypeCategorization()
        {
            #region [ ARRANGE ]

            // Create mocks for all dependencies
            var transactionHelperMock = new Mock<ITransactionHelper>();
            var transactionRepositoryMock = new Mock<IRepository<Transaction>>();
            var historicTransactionRepositoryMock = new Mock<IRepository<HistoricTransaction>>();
            var _transactionMapper = new TransactionToTransactionDtoMapper();
            var _historicTransactionMapper = new TransactionToHistoricTransactionMapper();
            var _serviceBusHandlerMock = new Mock<IServiceBusHandler>();
            var _configurationMock = new Mock<IEdreamsConfiguration>();
            var _securityContextMock = new Mock<ISecurityContext>();
            var _exceptionFactory = new ExceptionFactory();

            // Create an instance of the "TransactionQueueManager" using the mocked dependencies.
            ITransactionQueueManager transactionQueueManager = new TransactionQueueManager(
                transactionHelperMock.Object, transactionRepositoryMock.Object, historicTransactionRepositoryMock.Object,
                _transactionMapper, _historicTransactionMapper, _serviceBusHandlerMock.Object, _configurationMock.Object,
                _securityContextMock.Object, _exceptionFactory);

            // Prepare a request to use for when calling the "CreateUploadTransaction" method.
            Guid batchId = new Guid("9FC3D8F8-2A11-4B39-86A8-BD73D7FA3316");
            #endregion


            #region [ ACT ]

            // Call the "CreateCategorizationTransaction" method using the prepared request.
            await transactionQueueManager.CreateCategorizationTransaction(batchId);

            #endregion

            #region [ ASSERT ]

            // If the Categorization Transaction  created then the transction repository create method should call once.
            transactionRepositoryMock.VerifyRepositoryCreateSingle(Times.Once());

            #endregion
        }
        #endregion

        #region <| UpdateTransactionStatus |>

        [Fact]
        public async Task TransactionQueueManager_UpdateTransactionStatus_Should_Update_TransactionStatus_To_ProcessingStarted()
        {
            #region [ ARRANGE ]

            // Create mocks for all dependencies
            var transactionHelperMock = new Mock<ITransactionHelper>();
            var transactionRepositoryMock = new Mock<IRepository<Transaction>>();
            var historicTransactionRepositoryMock = new Mock<IRepository<HistoricTransaction>>();
            var _transactionMapper = new TransactionToTransactionDtoMapper();
            var _historicTransactionMapper = new TransactionToHistoricTransactionMapper();
            var _serviceBusHandlerMock = new Mock<IServiceBusHandler>();
            var _configurationMock = new Mock<IEdreamsConfiguration>();
            var _securityContextMock = new Mock<ISecurityContext>();
            var _exceptionFactory = new ExceptionFactory();

            // Create an instance of the "TransactionQueueManager" using the mocked dependencies.
            ITransactionQueueManager transactionQueueManager = new TransactionQueueManager(
                transactionHelperMock.Object, transactionRepositoryMock.Object, historicTransactionRepositoryMock.Object,
                _transactionMapper, _historicTransactionMapper, _serviceBusHandlerMock.Object, _configurationMock.Object,
                _securityContextMock.Object, _exceptionFactory);

            // Prepare a request to use for when calling the "UpdateTransactionStatus" method.
            Transaction transaction = new Transaction
            {
                Id = new Guid("46b7722d-f317-4a1d-9e69-d7592ad79c99"),
                BatchId = new Guid("46f61a38-ac53-46fe-9de3-ba80c5df5a92"),
                CorrelationId = Guid.NewGuid(),
                Status = TransactionStatus.Queued,
                ReleaseDate = DateTime.Now.AddHours(-1)
            };
            List<Transaction> transactions = new List<Transaction>() { transaction };
            Guid transactionId = new Guid("46b7722d-f317-4a1d-9e69-d7592ad79c99");
            TransactionStatus transactionStatus = TransactionStatus.ProcessingStarted;
            #endregion

            #region [ MOCK ]

            // Mock the "GetSingle" method on the "Repository" and run the predicate lambda expression on
            // an empty list of "FilePreload" objects.
            transactionRepositoryMock.SetupRepositoryGetSingle(transactions);

            #endregion

            #region [ ACT ]

            // Call the "UpdateTransactionStatus" method using the prepared request.
            TransactionDto result = await transactionQueueManager.UpdateTransactionStatus(transactionId, transactionStatus);

            #endregion

            #region [ ASSERT ]

            // The "UpdateTransactionStatus" method should update transaction status to processing strted from Queued
            result.Should().NotBeNull();
            result.Status.Should().Be(TransactionStatus.ProcessingStarted);

            #endregion
        }
        #endregion

        #region <| UpdateTransactionStatusAndArchive |>

        [Fact]
        public async Task TransactionQueueManager_UpdateTransactionStatusAndArchive_Should_Update_TransactionStatus_To_ProcessingStarted()
        {
            #region [ ARRANGE ]

            // Create mocks for all dependencies
            var transactionHelperMock = new Mock<ITransactionHelper>();
            var transactionRepositoryMock = new Mock<IRepository<Transaction>>();
            var historicTransactionRepositoryMock = new Mock<IRepository<HistoricTransaction>>();
            var _transactionMapper = new TransactionToTransactionDtoMapper();
            var _historicTransactionMapper = new TransactionToHistoricTransactionMapper();
            var _serviceBusHandlerMock = new Mock<IServiceBusHandler>();
            var _configurationMock = new Mock<IEdreamsConfiguration>();
            var _securityContextMock = new Mock<ISecurityContext>();
            var _exceptionFactory = new ExceptionFactory();

            // Create an instance of the "TransactionQueueManager" using the mocked dependencies.
            ITransactionQueueManager transactionQueueManager = new TransactionQueueManager(
                transactionHelperMock.Object, transactionRepositoryMock.Object, historicTransactionRepositoryMock.Object,
                _transactionMapper, _historicTransactionMapper, _serviceBusHandlerMock.Object, _configurationMock.Object,
                _securityContextMock.Object, _exceptionFactory);

            // Prepare a request to use for when calling the "UpdateTransactionStatusAndArchive" method.
            Transaction transaction = new Transaction
            {
                Id = new Guid("9FC3D8F8-2A11-4B39-86A8-BD73D7FA3316"),
                BatchId = new Guid("46f61a38-ac53-46fe-9de3-ba80c5df5a92"),
                CorrelationId = Guid.NewGuid(),
                Status = TransactionStatus.Queued,
                ReleaseDate = DateTime.Now.AddHours(-1)
            };
            List<Transaction> transactions = new List<Transaction>() { transaction };
            Guid transactionId = new Guid("9FC3D8F8-2A11-4B39-86A8-BD73D7FA3316");
            TransactionStatus transactionStatus = TransactionStatus.ProcessingStarted;
            Mock<TransactionScope> transactionScopeMock = new Mock<TransactionScope>();
            #endregion

            #region [ MOCK ]

            // Mock the "CreateScope" method on the "Helper" and run the predicate lambda expression on
            // "transactionScopeMock" object.
            transactionHelperMock.Setup(x => x.CreateScope()).Returns(transactionScopeMock.Object);
            // Mock the "GetSingle" method on the "Repository" and run the predicate lambda expression on
            // "transactions" object.
            transactionRepositoryMock.SetupRepositoryGetSingle(transactions);
            // Mock the "Delete" method on the "Repository" and run the predicate lambda expression on
            // "Transaction" object.
            transactionRepositoryMock.Setup(x=>x.Delete(It.IsAny<Transaction>())).ReturnsAsync(true);

            #endregion

            #region [ ACT ]

            // Call the "UpdateTransactionStatusAndArchive" method using the prepared request.
            TransactionDto result = await transactionQueueManager.UpdateTransactionStatusAndArchive(transactionId, transactionStatus);

            #endregion

            #region [ ASSERT ]

            // The "UpdateTransactionStatusAndArchive" method should update the transaction and archive.
            result.Should().NotBeNull();
            result.Status.Should().Be(TransactionStatus.Queued);
            #endregion

        }
        #endregion


        #region <| GetTransactionQueueStatistics |>

        [Fact]
        public async Task TransactionQueueManager_GetTransactionQueueStatistics_Should_Validate_ConnectionString()
        {
            #region [ ARRANGE ]

            // Create mocks for all dependencies
            var transactionHelperMock = new Mock<ITransactionHelper>();
            var transactionRepositoryMock = new Mock<IRepository<Transaction>>();
            var historicTransactionRepositoryMock = new Mock<IRepository<HistoricTransaction>>();
            var _transactionMapper = new TransactionToTransactionDtoMapper();
            var _historicTransactionMapper = new TransactionToHistoricTransactionMapper();
            var _serviceBusHandlerMock = new Mock<IServiceBusHandler>();
            var _configurationMock = new Mock<IEdreamsConfiguration>();
            var _securityContextMock = new Mock<ISecurityContext>();
            var _exceptionFactory = new ExceptionFactory();

            // Create an instance of the "TransactionQueueManager" using the mocked dependencies.
            ITransactionQueueManager transactionQueueManager = new TransactionQueueManager(
                transactionHelperMock.Object, transactionRepositoryMock.Object, historicTransactionRepositoryMock.Object,
                _transactionMapper, _historicTransactionMapper, _serviceBusHandlerMock.Object, _configurationMock.Object,
                _securityContextMock.Object, _exceptionFactory);

            // Prepare a request to use for when calling the "GetTransactionQueueStatistics" method.
            Transaction transaction = new Transaction
            {
                Id = new Guid("46b7722d-f317-4a1d-9e69-d7592ad79c99"),
                BatchId = new Guid("46f61a38-ac53-46fe-9de3-ba80c5df5a92"),
                CorrelationId = Guid.NewGuid(),
                Status = TransactionStatus.Queued,
                ReleaseDate = DateTime.Now.AddHours(-1)
            };
            List<Transaction> transactions = new List<Transaction>() { transaction };
            Guid transactionId = new Guid("9FC3D8F8-2A11-4B39-86A8-BD73D7FA3316");
            #endregion

            #region [ MOCK ]

            //Mock the configuration to setup ServiceBusConnectionString,ServiceBusQueueName values
            _configurationMock.Setup(x => x.ServiceBusConnectionString).Returns(string.Empty);
            _configurationMock.Setup(x => x.ServiceBusQueueName).Returns("OutlookMiddleWare");

            #endregion

            #region [ ACT ]

            // Call the "GetTransactionQueueStatistics" method using the prepared request.
            EdreamsException exception = await Assert.ThrowsAsync<EdreamsException>(() => transactionQueueManager.GetTransactionQueueStatistics());
            #endregion

            #region [ ASSERT ]
            // If there are is no "connection string" it should throw Connection String required validation
            exception.Should().NotBeNull();
            #endregion

        }

        [Fact]
        public async Task TransactionQueueManager_GetTransactionQueueStatistics_Should_Validate_ServiceBusQueueName()
        {
            #region [ ARRANGE ]

            // Create mocks for all dependencies
            var transactionHelperMock = new Mock<ITransactionHelper>();
            var transactionRepositoryMock = new Mock<IRepository<Transaction>>();
            var historicTransactionRepositoryMock = new Mock<IRepository<HistoricTransaction>>();
            var _transactionMapper = new TransactionToTransactionDtoMapper();
            var _historicTransactionMapper = new TransactionToHistoricTransactionMapper();
            var _serviceBusHandlerMock = new Mock<IServiceBusHandler>();
            var _configurationMock = new Mock<IEdreamsConfiguration>();
            var _securityContextMock = new Mock<ISecurityContext>();
            var _exceptionFactory = new ExceptionFactory();

            // Create an instance of the "TransactionQueueManager" using the mocked dependencies.
            ITransactionQueueManager transactionQueueManager = new TransactionQueueManager(
                transactionHelperMock.Object, transactionRepositoryMock.Object, historicTransactionRepositoryMock.Object,
                _transactionMapper, _historicTransactionMapper, _serviceBusHandlerMock.Object, _configurationMock.Object,
                _securityContextMock.Object, _exceptionFactory);

            // Prepare a request to use for when calling the "GetTransactionQueueStatistics" method.
            Transaction transaction = new Transaction
            {
                Id = new Guid("46b7722d-f317-4a1d-9e69-d7592ad79c99"),
                BatchId = new Guid("46f61a38-ac53-46fe-9de3-ba80c5df5a92"),
                CorrelationId = Guid.NewGuid(),
                Status = TransactionStatus.Queued,
                ReleaseDate = DateTime.Now.AddHours(-1)
            };
            List<Transaction> transactions = new List<Transaction>() { transaction };
            Guid transactionId = new Guid("9FC3D8F8-2A11-4B39-86A8-BD73D7FA3316");
            #endregion

            #region [ MOCK ]

            //Mock the configuration to setup ServiceBusConnectionString,ServiceBusQueueName values
            _configurationMock.Setup(x => x.ServiceBusConnectionString).Returns("OutlookMiddleWareConnectionString");
            _configurationMock.Setup(x => x.ServiceBusQueueName).Returns(string.Empty);

            #endregion

            #region [ ACT ]

            // Call the "GetTransactionQueueStatistics" method using the prepared request.
            EdreamsException exception = await Assert.ThrowsAsync<EdreamsException>(() => transactionQueueManager.GetTransactionQueueStatistics());
            #endregion

            #region [ ASSERT ]
            // If there are is no "connection string" it should throw service bus name validation
            exception.Should().NotBeNull();
            #endregion

        }

        [Fact]
        public async Task TransactionQueueManager_GetTransactionQueueStatistics_Should_Get_TransactionQueueStatistics()
        {
            #region [ ARRANGE ]

            // Create mocks for all dependencies
            var transactionHelperMock = new Mock<ITransactionHelper>();
            var transactionRepositoryMock = new Mock<IRepository<Transaction>>();
            var historicTransactionRepositoryMock = new Mock<IRepository<HistoricTransaction>>();
            var _transactionMapper = new TransactionToTransactionDtoMapper();
            var _historicTransactionMapper = new TransactionToHistoricTransactionMapper();
            var _serviceBusHandlerMock = new Mock<IServiceBusHandler>();
            var _configurationMock = new Mock<IEdreamsConfiguration>();
            var _securityContextMock = new Mock<ISecurityContext>();
            var _exceptionFactory = new ExceptionFactory();

            // Create an instance of the "TransactionQueueManager" using the mocked dependencies.
            ITransactionQueueManager transactionQueueManager = new TransactionQueueManager(
                transactionHelperMock.Object, transactionRepositoryMock.Object, historicTransactionRepositoryMock.Object,
                _transactionMapper, _historicTransactionMapper, _serviceBusHandlerMock.Object, _configurationMock.Object,
                _securityContextMock.Object, _exceptionFactory);

            // Prepare a request to use for when calling the "GetTransactionQueueStatistics" method.
            Transaction transaction = new Transaction
            {
                Id = new Guid("46b7722d-f317-4a1d-9e69-d7592ad79c99"),
                BatchId = new Guid("46f61a38-ac53-46fe-9de3-ba80c5df5a92"),
                CorrelationId = Guid.NewGuid(),
                Status = TransactionStatus.Queued,
                ReleaseDate = DateTime.Now.AddHours(-1)
            };
            List<Transaction> transactions = new List<Transaction>() { transaction };
            Guid transactionId = new Guid("9FC3D8F8-2A11-4B39-86A8-BD73D7FA3316");
            ServiceBusQueueStatistics queueStatistics = new ServiceBusQueueStatistics()
            {
                ActiveMessageCount = 5,
                ScheduledMessageCount = 5,
                DeadLetterMessageCount = 0,
            };
            #endregion

            #region [ MOCK ]
            //Mock the configuration to setup ServiceBusConnectionString,ServiceBusQueueName values
            _configurationMock.Setup(x => x.ServiceBusConnectionString).Returns("OutlookMiddleWareConnectionString");
            _configurationMock.Setup(x => x.ServiceBusQueueName).Returns("OutlookMiddleWare");
            // Mock the "GetQueueStatistics" method on the "serviceBusHandler" 
            _serviceBusHandlerMock.Setup(x => x.GetQueueStatistics(It.IsAny<string>(), It.IsAny<string>(), System.Threading.CancellationToken.None)).ReturnsAsync(queueStatistics);
            #endregion

            #region [ ACT ]

            // Call the "GetTransactionQueueStatistics" method using the prepared request.
            GetTransactionQueueStatisticsResponse response =await transactionQueueManager.GetTransactionQueueStatistics();
            #endregion

            #region [ ASSERT ]
            // The GetTransactionQueueStatistics method returns the response it contains transaction queue statistics.
            response.Should().NotBeNull();
            response.ScheduledMessageCount.Should().Be(5);
            response.ActiveMessageCount.Should().Be(5);
            response.DeadLetterMessageCount.Should().Be(0);
            #endregion

        }
        #endregion
    }
}
