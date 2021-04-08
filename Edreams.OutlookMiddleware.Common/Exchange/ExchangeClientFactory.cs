using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Edreams.OutlookMiddleware.Common.Exceptions;
using Edreams.OutlookMiddleware.Common.Exchange.Interfaces;
using Microsoft.Exchange.WebServices.Data;
using RestSharp;

namespace Edreams.OutlookMiddleware.Common.Exchange
{
    public class ExchangeClientFactory : IExchangeClientFactory
    {
        public async Task<IExchangeClient> AuthenticateAndCreateClient(ExchangeClientOptions clientOptions)
        {
            ExchangeClientToken exchangeClientToken = await GetAuthenticationToken(clientOptions);
            ExchangeService exchangeService = new ExchangeService
            {
                Credentials = new OAuthCredentials(exchangeClientToken.AccessToken),
                Url = new Uri(clientOptions.WebUri)
            };

            return new ExchangeClient(exchangeService, exchangeClientToken, clientOptions);
        }

        private async Task<ExchangeClientToken> GetAuthenticationToken(ExchangeClientOptions clientOptions)
        {
            StringBuilder formDataBuilder = new StringBuilder();
            formDataBuilder.Append($"grant_type=password&resource={clientOptions.Resource}");
            formDataBuilder.Append($"&client_id={clientOptions.ClientId}");
            formDataBuilder.Append($"&username={HttpUtility.UrlEncode(clientOptions.UserName)}");
            formDataBuilder.Append($"&password={HttpUtility.UrlEncode(clientOptions.Password)}");

            RestClient client = new RestClient(clientOptions.Authority);
            RestRequest request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("application/x-www-form-urlencoded", formDataBuilder.ToString(), ParameterType.RequestBody);

            var authenticationResult = await client.ExecuteAsync<ExchangeClientToken>(request);

            if (authenticationResult.StatusCode != HttpStatusCode.OK || authenticationResult.Data == null)
            {
                string errorMessage = $"Exchange authentication failed: {authenticationResult.Content}!";
                Exception exception = new EdreamsException(EdreamsExceptionCode.UnknownFault, errorMessage);
                throw exception;
            }

            return authenticationResult.Data;
        }
    }
}