using System.Threading.Tasks;
using Edreams.Common.Security.Interfaces;
using Edreams.Contracts.Data.Logging;
using Edreams.OutlookMiddleware.BusinessLogic.Interfaces;
using Edreams.OutlookMiddleware.Common.Constants;
using Edreams.OutlookMiddleware.Common.Helpers.Interfaces;
using Edreams.OutlookMiddleware.Common.Validation.Interface;
using Edreams.OutlookMiddleware.DataTransferObjects.Api;
using Edreams.OutlookMiddleware.Mapping.Interfaces;

namespace Edreams.OutlookMiddleware.BusinessLogic
{
    /// <summary>
    /// Logs a specified message or error..
    /// </summary>
    public class LoggingManager : ILoggingManager
    {
        #region <| Private Members |>

        private readonly IRestHelper<LogEntry> _restHelper;
        private readonly IMapper<RecordLogRequest, LogEntry> _recordLogRequestToLogEntryMapper;
        private readonly IValidator _validator;
        private readonly ISecurityContext _securityContext;

        #endregion

        #region <| Construction |>

        public LoggingManager(IRestHelper<LogEntry> restHelper,
            IMapper<RecordLogRequest, LogEntry> recordLogRequestToLogEntryMapper,
            IValidator validator, ISecurityContext securityContext)
        {
            _restHelper = restHelper;
            _recordLogRequestToLogEntryMapper = recordLogRequestToLogEntryMapper;
            _validator = validator;
            _securityContext = securityContext;
        }

        #endregion

        #region <|Public Methods |>

        /// <summary>
        /// Logs a specified message or error 
        /// </summary>
        /// <param name="recordLogRequest"></param>
        /// <returns></returns>
        public async Task<RecordLogResponse> RecordLog(RecordLogRequest recordLogRequest)
        {
            //Validations
            _validator.ValidateString(recordLogRequest.Level, ValidationMessages.WebApi.LevelRequired);
            _validator.ValidateString(recordLogRequest.Message, ValidationMessages.WebApi.MessageRequired);

            // Map recordLogRequest to LogEntry
            LogEntry logEntry = _recordLogRequestToLogEntryMapper.Map(recordLogRequest);
            // Record log using Rest call
            _ = await _restHelper.CreateNew("logEntry", logEntry);

            return new RecordLogResponse
            {
                CorrelationId = _securityContext.CorrelationId
            };
        }

        #endregion
    }
}
