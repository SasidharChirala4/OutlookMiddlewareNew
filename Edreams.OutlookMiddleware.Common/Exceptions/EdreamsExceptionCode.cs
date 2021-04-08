using System.ComponentModel;

namespace Edreams.OutlookMiddleware.Common.Exceptions
{
    public enum EdreamsExceptionCode
    {
        #region <| Common |>
        
        UnhandledException = -2,

        [Description("An unknown fault has occurred. Please contact support for further assistance.")]
        UnknownFault = -1,

        #endregion

        #region <| SqlClient |>

        [Description("An unknown database error has occured. Please contact support for further assistance.")]
        SqlClientUnknowFault = 2001,

        [Description("Communication with database server is not possible. Please check your configuration or contact support for further assistance.")]
        SqlClientServerNotFoundFault = 2002,

        [Description("A database timeout has occured. Please try again or contact support for further assistance.")]
        SqlClientTimeoutFault = 2003,

        [Description("Communication with database is not possible. Please check your configuration or contact support for further assistance.")]
        SqlClientDatabaseNotFoundFault = 2004,

        #endregion

        #region <| Azure KeyVault |>

        [Description("The Azure KeyVault service is not available. Please check your configuration or contact support for further assistance.")]
        KeyVaultRequestFault = 3001,

        [Description("There was an authentication issue with the Azure KeyVault service. Please check your configuration or contact support for further assistance.")]
        KeyVaultAuthenticationFault = 3002,

        #endregion

        #region <| Azure ServiceBus |>

        [Description("The connection string to connect to Azure ServiceBus is missing. Please check your configuration or contact support for further assistance.")]
        ServiceBusConnectionStringMissing = 4001,

        [Description("The queue name to connect to Azure ServiceBus is missing. Please check your configuration or contact support for further assistance.")]
        ServiceBusQueueNameMissing = 4002,

        [Description("The connection to Azure ServiceBus was successful, but the configured Queue cannot be found. Please check your configuration or contact support for further assistance.")]
        ServiceBusQueueNotFound = 4003,

        [Description("The connection to Azure ServiceBus has failed due to an authentication error. Please check your configuration or contact support for further assistance.")]
        ServiceBusUnauthorized = 4004,

        [Description("The connection to Azure ServiceBus has failed due to a connection error. Please check your configuration or contact support for further assistance.")]
        ServiceBusConnectionError = 4005,

        #endregion

        #region <| Outlook Middleware |>

        [Description("The specified batch was not found.")]
        OutlookMiddlewareBatchNotFound = 9001,

        [Description("The upload to e-DReaMS has failed.")]
        OutlookMiddlewareUploadToEdreamsFailed = 9101,

        #endregion
        
    }
}