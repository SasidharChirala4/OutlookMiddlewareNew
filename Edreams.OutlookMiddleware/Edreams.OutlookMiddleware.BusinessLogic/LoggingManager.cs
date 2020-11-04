using Edreams.OutlookMiddleware.BusinessLogic.Interfaces;
using Edreams.OutlookMiddleware.Common.Configuration.Interfaces;
using Edreams.OutlookMiddleware.Common.Helpers;
using Edreams.OutlookMiddleware.DataTransferObjects.Api;
using System;
using System.Threading.Tasks;

namespace Edreams.OutlookMiddleware.BusinessLogic
{
    /// <summary>
    /// Logs a specified message or error..
    /// </summary>
    public class LoggingManager : ILoggingManager
    {
        #region <| Private Members |>

        private readonly IEdreamsConfiguration _configuration;

        #endregion

        #region <| Construction |>

        public LoggingManager(IEdreamsConfiguration configuration)
        {
            _configuration = configuration;
        }

        #endregion

        #region <|Public Methods |>

        /// <summary>
        /// Logs a specified message or error 
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        public async Task<RecordLogResponse> RecordLog(RecordLogRequest log)
        {
            string logEntryEndpoint = string.Format($"{_configuration.EdreamsExtensibilityUrl}/logEntry");
            RestHelper<RecordLogRequest> request = new RestHelper<RecordLogRequest>(logEntryEndpoint, _configuration.EdreamsTokenValue, true);
            await request.CreateNew(log);
            return new RecordLogResponse()
            {
                CorrelationId = Guid.NewGuid()
            };
        }

        #endregion
    }
}
