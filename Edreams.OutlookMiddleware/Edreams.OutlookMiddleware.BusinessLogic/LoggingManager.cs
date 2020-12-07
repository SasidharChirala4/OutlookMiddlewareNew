using Edreams.Contracts.Data.Common;
using Edreams.Contracts.Data.Logging;
using Edreams.OutlookMiddleware.BusinessLogic.Interfaces;
using Edreams.OutlookMiddleware.Common.Constants;
using Edreams.OutlookMiddleware.Common.Helpers.Interfaces;
using Edreams.OutlookMiddleware.Common.Validation.Interface;
using Edreams.OutlookMiddleware.DataTransferObjects.Api;
using Edreams.OutlookMiddleware.Mapping.Interfaces;
using System.Threading.Tasks;

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

        #endregion

        #region <| Construction |>

        public LoggingManager(IRestHelper<LogEntry> restHelper,
            IMapper<RecordLogRequest, LogEntry> recordLogRequestToLogEntryMapper,
            IValidator validator)
        {
            _restHelper = restHelper;
            _recordLogRequestToLogEntryMapper = recordLogRequestToLogEntryMapper;
            _validator = validator;
        }

        #endregion

        #region <|Public Methods |>

        /// <summary>
        /// Logs a specified message or error 
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        public async Task<RecordLogResponse> RecordLog(RecordLogRequest recordLogRequest)
        {
            //Validations
            _validator.ValidateString(recordLogRequest.Level, ValidationMessages.WebApi.LevelRequired);
            _validator.ValidateString(recordLogRequest.Message, ValidationMessages.WebApi.MessageRequired);

            // Map recordLogRequest to LogEntry
            LogEntry logEntry = _recordLogRequestToLogEntryMapper.Map(recordLogRequest);
            // Record log using Rest call
            ApiResult<LogEntry> response = await _restHelper.CreateNew("logEntry", logEntry, true);
            
            //TODO: Need to write a better logic to validate the response properly

            return new RecordLogResponse()
            {
                CorrelationId = recordLogRequest.CorrelationId
            };
        }

        #endregion
    }
}
