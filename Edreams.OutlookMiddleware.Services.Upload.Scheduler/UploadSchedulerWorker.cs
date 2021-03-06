using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Edreams.Common.AzureServiceBus.Constants;
using Edreams.Common.AzureServiceBus.Contracts;
using Edreams.Common.AzureServiceBus.Interfaces;
using Edreams.Common.DataAccess.Constants;
using Edreams.Common.Exceptions;
using Edreams.Common.Exceptions.Constants;
using Edreams.Common.Exceptions.Factories.Interfaces;
using Edreams.Common.Logging.Interfaces;
using Edreams.Common.Security.Interfaces;
using Edreams.OutlookMiddleware.BusinessLogic.Interfaces;
using Edreams.OutlookMiddleware.Common.Configuration.Interfaces;
using Edreams.OutlookMiddleware.DataTransferObjects;
using Edreams.OutlookMiddleware.Enums;
using Microsoft.Azure.ServiceBus;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Hosting;

namespace Edreams.OutlookMiddleware.Services.Upload.Scheduler
{
    public class UploadSchedulerWorker : BackgroundService
    {
        private readonly ITransactionQueueManager _transactionQueueManager;
        private readonly IServiceBusHandler _serviceBusHandler;
        private readonly IExceptionFactory _exceptionFactory;
        private readonly IEdreamsConfiguration _configuration;
        private readonly ISecurityContext _securityContext;
        private readonly IEdreamsLogger<UploadSchedulerWorker> _logger;

        public UploadSchedulerWorker(
            ITransactionQueueManager transactionQueueManager,
            IServiceBusHandler serviceBusHandler,
            IExceptionFactory exceptionFactory,
            IEdreamsConfiguration configuration,
            ISecurityContext securityContext,
            IEdreamsLogger<UploadSchedulerWorker> logger)
        {
            _transactionQueueManager = transactionQueueManager;
            _serviceBusHandler = serviceBusHandler;
            _exceptionFactory = exceptionFactory;
            _configuration = configuration;
            _securityContext = securityContext;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Run continuously as long as the Windows Service is running. If the Windows Service
            // is stopped, the cancellation token will be cancelled and this loop will be stopped.
            while (!stoppingToken.IsCancellationRequested)
            {
                // Get the scheduling interval in seconds from the application
                // configuration and convert to milliseconds.
                int schedulingInterval = _configuration.TransactionSchedulingIntervalInSeconds * 1000;

                // Start a stopwatch for future reference when calculating the time we need to delay.
                Stopwatch stopwatch = Stopwatch.StartNew();

                try
                {
                    // Schedule the next transaction if available.
                    await ScheduleNextTransaction(stoppingToken);
                }
                catch (EdreamsException ex)
                {
                    _logger.LogError(ex.Message);
                }

                // Stop the stopwatch and subtract the number of milliseconds it recorded from the scheduling
                // interval. This will make sure the time between scheduling transactions is always consistent
                // and independent from the time it takes to process scheduling a transaction.
                stopwatch.Stop();
                schedulingInterval -= (int)stopwatch.ElapsedMilliseconds;
                if (schedulingInterval > 0)
                {
                    await Task.Delay(schedulingInterval, stoppingToken);
                }
            }
        }

        private async Task ScheduleNextTransaction(CancellationToken cancellationToken)
        {
            try
            {
                // Get the next transaction from the database.
                TransactionDto nextTransaction = await _transactionQueueManager.GetNextUploadTransaction();

                // If a transaction was found...
                if (nextTransaction != null)
                {
                    // Refresh the security context correlation identifier based on the transaction correlation identifier.
                    // If the transaction already has an assigned correlation identifier, we need to reuse that in order
                    // to keep all the actions correlated.
                    if (nextTransaction.CorrelationId != Guid.Empty)
                    {
                        _securityContext.RefreshCorrelationId(nextTransaction.CorrelationId);
                    }
                    else
                    {
                        _securityContext.RefreshCorrelationId();
                    }

                    // Prepare a ServiceBus message that contains a reference to the transaction
                    // and a reference to the batch that needs to be processed.
                    ServiceBusMessage<TransactionMessage> serviceBusMessage = new ServiceBusMessage<TransactionMessage>
                    {
                        CorrelationId = _securityContext.CorrelationId,
                        Data = new TransactionMessage
                        {
                            TransctionId = nextTransaction.Id,
                            BatchId = nextTransaction.BatchId,
                            CorrelationId = _securityContext.CorrelationId
                        },
                        QueuedOn = DateTime.UtcNow
                    };

                    // Post the prepared message to the ServiceBus queue so that it can be processed by the upload engine.
                    await _serviceBusHandler.PostMessage(_configuration.ServiceBusQueueName,
                        _configuration.ServiceBusConnectionString, serviceBusMessage, cancellationToken);

                    // Change the transaction status to scheduled.
                    await _transactionQueueManager.UpdateTransactionStatus(
                        nextTransaction.Id, TransactionStatus.Scheduled, _configuration.ServiceName);
                }
            }
            catch (SqlException ex) when (ex.Number == -1)
            {
                throw _exceptionFactory.CreateEdreamsExceptionFromCode(EdreamsDataAccessExceptionCode.SqlClientServerNotFoundFault, ex);
            }
            catch (SqlException ex) when (ex.Number == -2)
            {
                throw _exceptionFactory.CreateEdreamsExceptionFromCode(EdreamsDataAccessExceptionCode.SqlClientTimeoutFault, ex);
            }
            catch (SqlException ex) when (ex.Number == 4060)
            {
                throw _exceptionFactory.CreateEdreamsExceptionFromCode(EdreamsDataAccessExceptionCode.SqlClientDatabaseNotFoundFault, ex);
            }
            catch (SqlException ex)
            {
                throw _exceptionFactory.CreateEdreamsExceptionFromCode(EdreamsDataAccessExceptionCode.SqlClientUnknowFault, ex);
            }
            catch (MessagingEntityNotFoundException ex)
            {
                throw _exceptionFactory.CreateEdreamsExceptionFromCode(EdreamsServiceBusExceptionCode.ServiceBusQueueNotFound, ex);
            }
            catch (UnauthorizedException ex)
            {
                throw _exceptionFactory.CreateEdreamsExceptionFromCode(EdreamsServiceBusExceptionCode.ServiceBusUnauthorized, ex);
            }
            catch (ServiceBusException ex)
            {
                throw _exceptionFactory.CreateEdreamsExceptionFromCode(EdreamsServiceBusExceptionCode.ServiceBusConnectionError, ex);
            }
            catch (Exception ex)
            {
                throw _exceptionFactory.CreateEdreamsExceptionFromCode(EdreamsExceptionCode.UnknownFault, ex);
            }
        }
    }
}