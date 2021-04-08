using System;
using System.Threading.Tasks;
using Edreams.OutlookMiddleware.BusinessLogic;
using Edreams.OutlookMiddleware.BusinessLogic.Interfaces;
using Edreams.OutlookMiddleware.BusinessLogic.Transactions.Interfaces;
using Edreams.OutlookMiddleware.Common.Configuration.Interfaces;
using Edreams.OutlookMiddleware.Common.Exceptions;
using Edreams.OutlookMiddleware.Common.Exceptions.Interfaces;
using Edreams.OutlookMiddleware.Common.Security.Interfaces;
using Edreams.Common.AzureServiceBus.Interfaces;
using Edreams.OutlookMiddleware.DataAccess.Repositories.Interfaces;
using Edreams.OutlookMiddleware.DataTransferObjects;
using Edreams.OutlookMiddleware.Mapping.Interfaces;
using Edreams.OutlookMiddleware.Model;
using FluentAssertions;
using Moq;
using Xunit;

// ReSharper disable once CheckNamespace
namespace Edreams.OutlookMiddleware.Tests.BusinessLogic._TransactionQueueManager
{
    public class GetTransactionQueueStatisticsTests
    {
        private Mock<ITransactionHelper> _transactionHelperMock;
        private Mock<IRepository<Transaction>> _transactionRepositoryMock;
        private Mock<IRepository<HistoricTransaction>> _historicTransactionRepositoryMock;
        private Mock<IMapper<Transaction, TransactionDto>> _transactionMapperMock;
        private Mock<IMapper<Transaction, HistoricTransaction>> _historicTransactionMapper;
        private Mock<IServiceBusHandler> _serviceBusHandlerMock;
        private Mock<IEdreamsConfiguration> _edreamsConfigurationMock;
        private Mock<ISecurityContext> _securityContextMock;
        private Mock<IExceptionFactory> _exceptionFactoryMock;

        public GetTransactionQueueStatisticsTests()
        {
            // Create mocks for all dependencies
            _transactionHelperMock = new Mock<ITransactionHelper>();
            _transactionRepositoryMock = new Mock<IRepository<Transaction>>();
            _historicTransactionRepositoryMock = new Mock<IRepository<HistoricTransaction>>();
            _transactionMapperMock = new Mock<IMapper<Transaction, TransactionDto>>();
            _historicTransactionMapper = new Mock<IMapper<Transaction, HistoricTransaction>>();
            _serviceBusHandlerMock = new Mock<IServiceBusHandler>();
            _edreamsConfigurationMock = new Mock<IEdreamsConfiguration>();
            _securityContextMock = new Mock<ISecurityContext>();
            _exceptionFactoryMock = new Mock<IExceptionFactory>();
        }

        [Fact]
        public async Task TransactionQueueManager_GetTransactionQueueStatisticsTests_Should_Throw_If_ServiceBusConnectionString_Is_Empty()
        {
            #region [ ARRANGE ]

            // Create an instance of the "Subject Under Test" using the mocked dependencies.
            ITransactionQueueManager transactionManager = new TransactionQueueManager(
                _transactionHelperMock.Object, _transactionRepositoryMock.Object, _historicTransactionRepositoryMock.Object,
                _transactionMapperMock.Object, _historicTransactionMapper.Object, _serviceBusHandlerMock.Object,
                _edreamsConfigurationMock.Object, _securityContextMock.Object, _exceptionFactoryMock.Object);

            #endregion

            #region [ MOCK ]

            // Mock the "Find" method on the "Repository" and run the predicate lambda expression on
            // an empty list of "FilePreload" objects.
            _edreamsConfigurationMock.Setup(x => x.ServiceBusConnectionString).Returns(string.Empty);
            _edreamsConfigurationMock.Setup(x => x.ServiceBusQueueName).Returns("QUEUE_NAME");
            _exceptionFactoryMock.Setup(x => x.CreateEdreamsExceptionFromCode(It.IsAny<EdreamsExceptionCode>())).Returns(new EdreamsException());

            #endregion

            #region [ ACT ]

            Func<Task> act = async () => { await transactionManager.GetTransactionQueueStatistics(); };

            #endregion

            #region [ ASSERT ]

            await act.Should().ThrowAsync<EdreamsException>();
            _edreamsConfigurationMock.Verify(x => x.ServiceBusConnectionString, Times.Once);
            _edreamsConfigurationMock.Verify(x => x.ServiceBusQueueName, Times.Once);
            _exceptionFactoryMock.Verify(x => x.CreateEdreamsExceptionFromCode(It.Is<EdreamsExceptionCode>(x => x == EdreamsExceptionCode.ServiceBusConnectionStringMissing), It.IsAny<object[]>()));

            #endregion
        }

        [Fact]
        public async Task TransactionQueueManager_GetTransactionQueueStatisticsTests_Should_Throw_If_ServiceBusQueueName_Is_Empty()
        {
            #region [ ARRANGE ]

            // Create an instance of the "Subject Under Test" using the mocked dependencies.
            ITransactionQueueManager transactionManager = new TransactionQueueManager(
                _transactionHelperMock.Object, _transactionRepositoryMock.Object, _historicTransactionRepositoryMock.Object,
                _transactionMapperMock.Object, _historicTransactionMapper.Object, _serviceBusHandlerMock.Object,
                _edreamsConfigurationMock.Object, _securityContextMock.Object, _exceptionFactoryMock.Object);

            #endregion

            #region [ MOCK ]

            // Mock the "Find" method on the "Repository" and run the predicate lambda expression on
            // an empty list of "FilePreload" objects.
            _edreamsConfigurationMock.Setup(x => x.ServiceBusConnectionString).Returns("CONNECTION_STRING");
            _edreamsConfigurationMock.Setup(x => x.ServiceBusQueueName).Returns(string.Empty);
            _exceptionFactoryMock.Setup(x => x.CreateEdreamsExceptionFromCode(It.IsAny<EdreamsExceptionCode>())).Returns(new EdreamsException());

            #endregion

            #region [ ACT ]

            Func<Task> act = async () => { await transactionManager.GetTransactionQueueStatistics(); };

            #endregion

            #region [ ASSERT ]

            await act.Should().ThrowAsync<EdreamsException>();
            _edreamsConfigurationMock.Verify(x => x.ServiceBusConnectionString, Times.Once);
            _edreamsConfigurationMock.Verify(x => x.ServiceBusQueueName, Times.Once);
            _exceptionFactoryMock.Verify(x => x.CreateEdreamsExceptionFromCode(It.Is<EdreamsExceptionCode>(x => x == EdreamsExceptionCode.ServiceBusQueueNameMissing), It.IsAny<object[]>()));

            #endregion
        }
    }
}