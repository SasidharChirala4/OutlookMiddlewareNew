using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Edreams.OutlookMiddleware.BusinessLogic.Interfaces;
using Edreams.OutlookMiddleware.BusinessLogic.Transactions.Interfaces;
using Edreams.OutlookMiddleware.Common.Configuration.Interfaces;
using Edreams.OutlookMiddleware.Common.Exceptions;
using Edreams.OutlookMiddleware.Common.Exceptions.Interfaces;
using Edreams.OutlookMiddleware.Common.Security.Interfaces;
using Edreams.Common.AzureServiceBus.Contracts;
using Edreams.Common.AzureServiceBus.Interfaces;
using Edreams.OutlookMiddleware.DataAccess.Repositories.Interfaces;
using Edreams.OutlookMiddleware.DataTransferObjects;
using Edreams.OutlookMiddleware.DataTransferObjects.Api;
using Edreams.OutlookMiddleware.Enums;
using Edreams.OutlookMiddleware.Mapping.Interfaces;
using Edreams.OutlookMiddleware.Model;
using Edreams.OutlookMiddleware.Model.Interfaces;
using Microsoft.Azure.ServiceBus;

namespace Edreams.OutlookMiddleware.BusinessLogic
{
    public class TransactionQueueManager : ITransactionQueueManager
    {
        private readonly ITransactionHelper _transactionHelper;
        private readonly IRepository<Transaction> _transactionRepository;
        private readonly IRepository<HistoricTransaction> _historicTransactionRepository;
        private readonly IMapper<Transaction, TransactionDto> _transactionMapper;
        private readonly IMapper<Transaction, HistoricTransaction> _historicTransactionMapper;
        private readonly IServiceBusHandler _serviceBusHandler;
        private readonly IEdreamsConfiguration _configuration;
        private readonly ISecurityContext _securityContext;
        private readonly IExceptionFactory _exceptionFactory;

        public TransactionQueueManager(
            ITransactionHelper transactionHelper,
            IRepository<Transaction> transactionRepository,
            IRepository<HistoricTransaction> historicTransactionRepository,
            IMapper<Transaction, TransactionDto> transactionMapper,
            IMapper<Transaction, HistoricTransaction> historicTransactionMapper,
            IServiceBusHandler serviceBusHandler,
            IEdreamsConfiguration configuration,
            ISecurityContext securityContext,
            IExceptionFactory exceptionFactory)
        {
            _transactionHelper = transactionHelper;
            _transactionRepository = transactionRepository;
            _historicTransactionRepository = historicTransactionRepository;
            _transactionMapper = transactionMapper;
            _historicTransactionMapper = historicTransactionMapper;
            _serviceBusHandler = serviceBusHandler;
            _configuration = configuration;
            _securityContext = securityContext;
            _exceptionFactory = exceptionFactory;
        }

        public Task<bool> IsTransactionAvailable(Guid transactionId)
        {
            // Check the database and look for a transaction with the specified transaction ID.
            return _transactionRepository.Exists(x => x.Id == transactionId);
        }

        public async Task<TransactionDto> GetTransaction(Guid transactionId)
        {
            // Check the database and retrieve the transaction with the specified transaction ID.
            Transaction transaction = await _transactionRepository.GetSingle(x => x.Id == transactionId);

            // Return a mapped DTO.
            return _transactionMapper.Map(transaction);
        }

        public Task<TransactionStatus?> GetTransactionStatus(Guid transactionId)
        {
            // Check the database and retrieve the status for the transaction with the specified transaction ID.
            return _transactionRepository.GetSingleAndProject(
                x => x.Id == transactionId, proj => (TransactionStatus?)proj.Status);
        }

        /// <summary>
        /// Gets the next upload transaction to process.
        /// </summary>
        /// <returns>The requested upload transaction.</returns>
        public Task<TransactionDto> GetNextUploadTransaction()
        {
            return GetNextTransaction(TransactionType.Upload);
        }

        /// <summary>
        /// Gets the next categorization transaction to process.
        /// </summary>
        /// <returns>The requested categorization transaction.</returns>
        public Task<TransactionDto> GetNextCategorizationTransaction()
        {
            return GetNextTransaction(TransactionType.Categorization);
        }

        /// <summary>
        /// Creates a new upload transaction for a specific batch.
        /// </summary>
        /// <param name="batchId">The unique identifier for the batch that needs to be uploaded.</param>
        public Task CreateUploadTransaction(Guid batchId)
        {
            return CreateTransaction(batchId, TransactionType.Upload);
        }

        /// <summary>
        /// Creates a new categorization transaction for a specific batch.
        /// </summary>
        /// <param name="batchId">The unique identifier for the batch that needs to be categorized.</param>
        public Task CreateCategorizationTransaction(Guid batchId)
        {
            return CreateTransaction(batchId, TransactionType.Categorization);
        }

        public Task<TransactionDto> UpdateTransactionStatus(Guid transactionId, TransactionStatus status)
        {
            // Call the UpdateTransactionStatus method with an empty engine name.
            return UpdateTransactionStatus(transactionId, status, null);
        }

        public async Task<TransactionDto> UpdateTransactionStatus(Guid transactionId, TransactionStatus status, string engine)
        {
            // Check the database and retrieve the transaction with the specified transaction ID.
            Transaction transactionToUpdate = await _transactionRepository.GetSingle(x => x.Id == transactionId);

            // If the transaction was found...
            if (transactionToUpdate != null)
            {
                // Update the transaction status.
                UpdateTransactionStatus(transactionToUpdate, status);

                // If the engine name is not empty...
                if (!string.IsNullOrEmpty(engine))
                {
                    // Update the processing engine name.
                    transactionToUpdate.ProcessingEngine = engine;
                }

                // Update the transaction in the database.
                await _transactionRepository.Update(transactionToUpdate);
            }

            // Return a mapped DTO.
            return _transactionMapper.Map(transactionToUpdate);
        }

        public Task<TransactionDto> UpdateTransactionStatusAndArchive(Guid transactionId, TransactionStatus status)
        {
            // Call the UpdateTransactionStatusAndArchive method with an empty engine name.
            return UpdateTransactionStatusAndArchive(transactionId, status, null);
        }

        public async Task<TransactionDto> UpdateTransactionStatusAndArchive(Guid transactionId, TransactionStatus status, string engine)
        {
            // Force a database transaction scope to make sure multiple
            // operations are combined as a single atomic operation.
            using ITransactionScope transactionScope = _transactionHelper.CreateScope();

            // Check the database and retrieve the transaction with the specified transaction ID.
            Transaction transactionToArchive = await _transactionRepository.GetSingle(x => x.Id == transactionId);

            // If the transaction was found...
            if (transactionToArchive != null)
            {
                // Delete the original transaction from the database.
                await _transactionRepository.Delete(transactionToArchive);

                // Map the transaction into a historic transaction and update the transaction status.
                HistoricTransaction historicTransaction = _historicTransactionMapper.Map(transactionToArchive);
                UpdateTransactionStatus(historicTransaction, status);

                // If the engine name is not empty...
                if (!string.IsNullOrEmpty(engine))
                {
                    // Update the processing engine name.
                    historicTransaction.ProcessingEngine = engine;
                }

                // Create the historic transaction in the database.
                await _historicTransactionRepository.Create(historicTransaction);

                // Commit the database transaction.
                transactionScope.Commit();
            }

            // Return a mapped DTO.
            return _transactionMapper.Map(transactionToArchive);
        }

        public async Task<GetTransactionQueueStatisticsResponse> GetTransactionQueueStatistics()
        {
            // Get connection string and queue name from configuration.
            string connectionString = _configuration.ServiceBusConnectionString;
            string queueName = _configuration.ServiceBusQueueName;

            // If connection string is empty, throw exception.
            if (string.IsNullOrEmpty(connectionString))
            {
                throw _exceptionFactory.CreateEdreamsExceptionFromCode(EdreamsExceptionCode.ServiceBusConnectionStringMissing);
            }

            // If queue name is empty, throw exception.
            if (string.IsNullOrEmpty(queueName))
            {
                throw _exceptionFactory.CreateEdreamsExceptionFromCode(EdreamsExceptionCode.ServiceBusQueueNameMissing);
            }

            try
            {
                // Get the queue statistics using the service bus handler.
                ServiceBusQueueStatistics queueStatistics = await _serviceBusHandler.GetQueueStatistics(connectionString, queueName);

                // Return number of active messages, scheduled messages and dead letter messages.
                return new GetTransactionQueueStatisticsResponse
                {
                    ActiveMessageCount = queueStatistics.ActiveMessageCount,
                    ScheduledMessageCount = queueStatistics.ScheduledMessageCount,
                    DeadLetterMessageCount = queueStatistics.DeadLetterMessageCount,
                    CorrelationId = _securityContext.CorrelationId
                };
            }
            catch (MessagingEntityNotFoundException ex)
            {
                // This exception gets thrown when the specified queue could not be found.
                throw _exceptionFactory.CreateEdreamsExceptionFromCode(EdreamsExceptionCode.ServiceBusQueueNotFound, ex);
            }
            catch (UnauthorizedException ex)
            {
                // This exception gets thrown when there is an issue with authorization.
                throw _exceptionFactory.CreateEdreamsExceptionFromCode(EdreamsExceptionCode.ServiceBusUnauthorized, ex);
            }
            catch (ServiceBusException ex)
            {
                // This exception gets thrown when there is an issue with connecting to Azure ServiceBus.
                throw _exceptionFactory.CreateEdreamsExceptionFromCode(EdreamsExceptionCode.ServiceBusConnectionError, ex);
            }
            catch (Exception ex)
            {
                // This handles all remaining exceptions.
                throw _exceptionFactory.CreateEdreamsExceptionFromCode(EdreamsExceptionCode.UnknownFault, ex);
            }
        }

        public async Task<TransactionDto> GetNextTransaction(TransactionType transactionType)
        {
            // Build the predicate that searches for all queued transactions that have a release date in the past.
            Expression<Func<Transaction, bool>> predicate =
                x => x.Type == transactionType && x.Status == TransactionStatus.Queued &&
                     x.ReleaseDate.HasValue && x.ReleaseDate < DateTime.UtcNow;

            // Check the database and retrieve the first transaction using the prepared predicate, ordering them by release date.
            Transaction transaction = await _transactionRepository.GetFirstAscending(predicate, o => o.ReleaseDate);

            // Return a mapped DTO.
            return _transactionMapper.Map(transaction);
        }

        private async Task CreateTransaction(Guid batchId, TransactionType transactionType)
        {
            Transaction transaction = new Transaction
            {
                BatchId = batchId,
                Status = TransactionStatus.Queued,
                Type = transactionType,
                CorrelationId = _securityContext.CorrelationId,
                ReleaseDate = DateTime.UtcNow
            };

            await _transactionRepository.Create(transaction);
        }

        private void UpdateTransactionStatus(ITransaction transaction, TransactionStatus status)
        {
            // Update the transaction status.
            transaction.Status = status;

            // Update a number of timestamps based on the transaction status.
            switch (status)
            {
                case TransactionStatus.Scheduled:
                    transaction.Scheduled = DateTime.UtcNow;
                    break;

                case TransactionStatus.ProcessingStarted:
                    transaction.ProcessingStarted = DateTime.UtcNow;
                    break;

                case TransactionStatus.ProcessingSucceeded:
                case TransactionStatus.ProcessingPartiallySucceeded:
                case TransactionStatus.ProcessingFailed:
                    transaction.ProcessingFinished = DateTime.UtcNow;
                    break;
            }
        }
    }
}