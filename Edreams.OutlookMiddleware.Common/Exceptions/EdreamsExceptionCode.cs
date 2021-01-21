using System.ComponentModel;

namespace Edreams.OutlookMiddleware.Common.Exceptions
{
    public enum EdreamsExceptionCode
    {
        [Description("An unknown fault has occurred. Please contact support for further assistance.")]
        UNKNOWN_FAULT = 0,

        [Description("The Azure KeyVault service is not available. Please check your configuration or contact support for further assistance.")]
        KEYVAULT_REQUEST_FAULT = 7001,

        [Description("There was an authentication issue with the Azure KeyVault service. Please check your configuration or contact support for further assistance.")]
        KEYVAULT_AUTHENTICATION_FAULT = 7002,
    }
}