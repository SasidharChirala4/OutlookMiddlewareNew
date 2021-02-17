using System.ComponentModel;

namespace Edreams.OutlookMiddleware.Common.Exceptions
{
    public enum EdreamsExceptionCode
    {
        #region <| Common |>

        [Description("An unknown fault has occurred. Please contact support for further assistance.")]
        UNKNOWN_FAULT = -1,

        #endregion

        #region <| SqlClient |>

        [Description("An unknown database error has occured. Please contact support for further assistance.")]
        SQLCLIENT_UNKNOWN_FAULT = 2001,

        [Description("Communication with database server is not possible. Please check your configuration or contact support for further assistance.")]
        SQLCLIENT_SERVER_NOT_FOUND_FAULT = 2002,

        [Description("A database timeout has occured. Please try again or contact support for further assistance.")]
        SQLCLIENT_TIMEOUT_FAULT = 2003,

        [Description("Communication with database is not possible. Please check your configuration or contact support for further assistance.")]
        SQLCLIENT_DATABASE_NOT_FOUND_FAULT = 2004,

        #endregion

        #region <| Azure KeyVault |>

        [Description("The Azure KeyVault service is not available. Please check your configuration or contact support for further assistance.")]
        KEYVAULT_REQUEST_FAULT = 3001,

        [Description("There was an authentication issue with the Azure KeyVault service. Please check your configuration or contact support for further assistance.")]
        KEYVAULT_AUTHENTICATION_FAULT = 3002,

        #endregion

        #region <| Azure ServiceBus |>

        [Description("The connection string to connect to Azure ServiceBus is missing. Please check your configuration or contact support for further assistance.")]
        SERVICEBUS_CONNECTIONSTRING_MISSING = 4001,

        [Description("The queue name to connect to Azure ServiceBus is missing. Please check your configuration or contact support for further assistance.")]
        SERVICEBUS_QUEUENAME_MISSING = 4002,

        [Description("The connection to Azure ServiceBus was successful, but the configured Queue cannot be found. Please check your configuration or contact support for further assistance.")]
        SERVICEBUS_QUEUE_NOT_FOUND = 4003,

        [Description("The connection to Azure ServiceBus has failed due to an authentication error. Please check your configuration or contact support for further assistance.")]
        SERVICEBUS_UNAUTHORIZED = 4004,

        [Description("The connection to Azure ServiceBus has failed due to a connection error. Please check your configuration or contact support for further assistance.")]
        SERVICEBUS_CONNECTION_ERROR = 4005,

        #endregion

        #region <| Outlook Middleware |>

        [Description("The specified batch was not found.")]
        OUTLOOKMIDDLEWARE_BATCH_NOT_FOUND = 9001,
        
        [Description("The upload to e-DReaMS has failed.")]
        OUTLOOKMIDDLEWARE_UPLOAD_TO_EDREAMS_FAILED = 9101,
        
        #endregion
        
    }
}