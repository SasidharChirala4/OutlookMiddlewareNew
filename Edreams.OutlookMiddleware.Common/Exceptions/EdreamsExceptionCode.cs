using System.ComponentModel;

namespace Edreams.OutlookMiddleware.Common.Exceptions
{
    public enum EdreamsExceptionCode
    {
        #region <| Common |>

        [Description("An unknown fault has occurred. Please contact support for further assistance.")]
        UNKNOWN_FAULT = 0,

        #endregion

        #region <| Azure KeyVault |>

        [Description("The Azure KeyVault service is not available. Please check your configuration or contact support for further assistance.")]
        KEYVAULT_REQUEST_FAULT = 7001,

        [Description("There was an authentication issue with the Azure KeyVault service. Please check your configuration or contact support for further assistance.")]
        KEYVAULT_AUTHENTICATION_FAULT = 7002,

        #endregion

        #region <| Azure ServiceBus |>

        [Description("The connection string to connect to Azure ServiceBus is missing. Please check your configuration or contact support for further assistance.")]
        SERVICEBUS_CONNECTIONSTRING_MISSING = 8001,

        [Description("The queue name to connect to Azure ServiceBus is missing. Please check your configuration or contact support for further assistance.")]
        SERVICEBUS_QUEUENAME_MISSING = 8002,

        [Description("The connection to Azure ServiceBus was successful, but the configured Queue cannot be found. Please check your configuration or contact support for further assistance.")]
        SERVICEBUS_QUEUE_NOT_FOUND = 8003,

        [Description("The connection to Azure ServiceBus has failed due to an authentication error. Please check your configuration or contact support for further assistance.")]
        SERVICEBUS_UNAUTHORIZED = 8004,

        [Description("The connection to Azure ServiceBus has failed due to a connection error. Please check your configuration or contact support for further assistance.")]
        SERVICEBUS_CONNECTION_ERROR = 8005

        #endregion
    }
}