using Edreams.OutlookMiddleware.BusinessLogic.Interfaces;
using Edreams.OutlookMiddleware.Common.Configuration.Interfaces;
using Edreams.OutlookMiddleware.DataTransferObjects.Api;
using Microsoft.Exchange.WebServices.Data;
using System;
using System.Threading.Tasks;
using System.Net;
using Task = System.Threading.Tasks.Task;
using Edreams.OutlookMiddleware.Common.Exceptions;
using RestSharp;
using System.Web;
using Edreams.OutlookMiddleware.Model;
using System.Linq;

namespace Edreams.OutlookMiddleware.BusinessLogic
{
    public class ConfigurationManager : IConfigurationManager
    {
        #region <| Private Variable |>

        private readonly IEdreamsConfiguration _edreamsConfiguration;
        private readonly ILoggingManager _logger;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationManager" /> class.
        /// </summary>        
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public ConfigurationManager(IEdreamsConfiguration edreamsConfiguration, ILoggingManager logger)
        {
            _edreamsConfiguration = edreamsConfiguration;
            _logger = logger;
        }

        /// <summary>
        /// Gets the Outlook Middleware shared mailbox.
        /// </summary>
        /// <returns>The Outlook Middleware shared mailbox, if available. <see cref="System.String.Empty"/> otherwise.</returns>
        public async Task<GetSharedMailBoxResponse> GetSharedMailBox()
        {
            try
            {
                (ExchangeService exchangeService, string userName) = await BuildExchangeService();

                // Resolve the mailbox for the current user.
                NameResolutionCollection resolvedMailBox = await exchangeService.ResolveName(userName);

                if (resolvedMailBox.Count != 1)
                {
                    string errorMessage = $"Exchange MailBox resolution failed: {resolvedMailBox.Count}!";
                    Exception exception = new EdreamsException(errorMessage);
                    await _logger.RecordLog(new RecordLogRequest { Level = "Error", Message = errorMessage });
                    throw exception;
                }

                return await Task.FromResult(new GetSharedMailBoxResponse
                {
                    Email = resolvedMailBox.Single().Mailbox.Address
                });
            }
            catch (EdreamsException)
            {
                throw;
            }
            catch (Exception ex)
            {
                await _logger.RecordLog(new RecordLogRequest { Level = "Error", Message = "An error occured during EWS communication!",ExceptionDetails = ex.Message });
                return new GetSharedMailBoxResponse();
            }
        }

        #region Private Methods

        private async Task<(ExchangeService, string)> BuildExchangeService()
        {
            // Get variables from application configuration.
            string authority = _edreamsConfiguration.ExchangeAuthority;
            string resource = _edreamsConfiguration.ExchangeResourceId;
            string clientId = _edreamsConfiguration.ExchangeClientId;
            string webUri = _edreamsConfiguration.ExchangeOnlineServer;
            string credentialsTarget = _edreamsConfiguration.SharedMailBoxCredentials;


            if (string.IsNullOrEmpty(credentialsTarget))
            {
                string errorMessage = "Invalid application configuration!";
                Exception ex = new EdreamsException(errorMessage);
                await _logger.RecordLog(new RecordLogRequest { Level = "Error", Message = errorMessage});
                throw ex;
            }

            // Ignoring CredentialStore as of now, planning to implement Azure key walts in this place
            // TODO : Remove once Azure keywalt code is ready 
            // Temporary Code
            NetworkCredential credentials = new NetworkCredential()
            {
                UserName = "bkonijeti@deloitte.com",
                Password = "Accompanying57"
            };

            if (credentials == null)
            {
                // TODO : Change message once Azure keywalt logic is ready 
                string errorMessage = "No credentials found in the Windows Credential Manager!";
                Exception exception = new EdreamsException(errorMessage);
                await _logger.RecordLog(new RecordLogRequest { Level = "Error", Message = errorMessage });
                throw exception;
            }

            string accessToken = await GetAuthenticationToken(authority, resource, clientId, credentials);

            return (new ExchangeService
            {
                Credentials = new OAuthCredentials(accessToken),
                Url = new Uri(webUri)
            }, credentials.UserName);
        }
        private async Task<string> GetAuthenticationToken(string authority, string resource, string clientId, NetworkCredential credentials)
        {
            RestClient client = new RestClient(authority);
            RestRequest request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("application/x-www-form-urlencoded",
                $"grant_type=password&resource={resource}&client_id={clientId}&username={HttpUtility.UrlEncode(credentials.UserName)}&password={HttpUtility.UrlEncode(credentials.Password)}",
                ParameterType.RequestBody);

            var authenticationResult = await client.ExecuteAsync<ExchangeAuthenticationTokenResponse>(request);

            if (authenticationResult.StatusCode != HttpStatusCode.OK || authenticationResult.Data == null)
            {
                string errorMessage = $"Exchange authentication failed: {authenticationResult.Content}!";
                Exception exception = new EdreamsException(errorMessage);
                await _logger.RecordLog(new RecordLogRequest { Level = "Error", Message = errorMessage });
                throw exception;
            }
            return authenticationResult.Data.AccessToken;
        }

        #endregion
    }
}
