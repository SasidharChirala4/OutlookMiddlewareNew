using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Edreams.OutlookMiddleware.BusinessLogic.Interfaces;
using Edreams.OutlookMiddleware.Common.Configuration.Interfaces;
using Edreams.OutlookMiddleware.Common.Exceptions;
using Edreams.OutlookMiddleware.Common.Exceptions.Interfaces;
using Edreams.OutlookMiddleware.Common.Security.Interfaces;
using Edreams.OutlookMiddleware.Common.ServiceBus.Contracts;
using Edreams.OutlookMiddleware.Common.ServiceBus.Interfaces;
using Edreams.OutlookMiddleware.DataTransferObjects;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Edreams.OutlookMiddleware.Services.Upload.Scheduler
{
    public class UploadSchedulerWorker : BackgroundService
    {
        private readonly ITransactionQueueManager _transactionQueueManager;
        private readonly IServiceBusHandler _serviceBusHandler;
        private readonly IExceptionFactory _exceptionFactory;
        private readonly IEdreamsConfiguration _configuration;
        private readonly ISecurityContext _securityContext;
        private readonly ILogger<UploadSchedulerWorker> _logger;

        public UploadSchedulerWorker(
            ITransactionQueueManager transactionQueueManager,
            IServiceBusHandler serviceBusHandler,
            IExceptionFactory exceptionFactory,
            IEdreamsConfiguration configuration,
            ISecurityContext securityContext,
            ILogger<UploadSchedulerWorker> logger)
        {
            _transactionQueueManager = transactionQueueManager;
            _serviceBusHandler = serviceBusHandler;
            _exceptionFactory = exceptionFactory;
            _configuration = configuration;
            _securityContext = securityContext;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            // Run continuously as long as the Windows Service is running. If the Windows Service
            // is stopped, the cancellation token will be cancelled and this loop will be stopped.
            while (!cancellationToken.IsCancellationRequested)
            {
                // Get the scheduling interval in seconds from the application
                // configuration and convert to milliseconds.
                int schedulingInterval = _configuration.TransactionSchedulingIntervalInSeconds * 1000;

                // Start a stopwatch for future reference when calculating the time we need to delay.
                Stopwatch stopwatch = Stopwatch.StartNew();

                try
                {
                    // Schedule the next transaction if available.
                    await ScheduleNextTransaction(cancellationToken);
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
                    await Task.Delay(schedulingInterval, cancellationToken);
                }
            }
        }

        private async Task ScheduleNextTransaction(CancellationToken cancellationToken)
        {
            try
            {
                // Get the next transaction from the database.
                TransactionDto nextTransaction = await _transactionQueueManager.GetNextTransaction();

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
                    await _serviceBusHandler.PostMessage(_configuration.ServiceBusQueueName, serviceBusMessage, cancellationToken);
                }
            }
            catch (SqlException ex) when (ex.Number == -1)
            {
                throw _exceptionFactory.CreateFromCode(EdreamsExceptionCode.SQLCLIENT_SERVER_NOT_FOUND_FAULT, ex);
            }
            catch (SqlException ex) when (ex.Number == -2)
            {
                throw _exceptionFactory.CreateFromCode(EdreamsExceptionCode.SQLCLIENT_TIMEOUT_FAULT, ex);
            }
            catch (SqlException ex) when (ex.Number == 4060)
            {
                throw _exceptionFactory.CreateFromCode(EdreamsExceptionCode.SQLCLIENT_DATABASE_NOT_FOUND_FAULT, ex);
            }
            catch (SqlException ex)
            {
                throw _exceptionFactory.CreateFromCode(EdreamsExceptionCode.SQLCLIENT_UNKNOWN_FAULT, ex);
            }
            catch (Exception ex)
            {
                throw _exceptionFactory.CreateFromCode(EdreamsExceptionCode.UNKNOWN_FAULT, ex);
            }
        }
    }
}