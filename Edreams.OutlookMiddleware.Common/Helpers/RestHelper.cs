using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Edreams.Contracts.Data.Common;
using System.Threading.Tasks;
using Edreams.OutlookMiddleware.Common.Configuration.Interfaces;
using Edreams.OutlookMiddleware.Common.Helpers.Interfaces;
using Edreams.OutlookMiddleware.Common.Exceptions;

namespace Edreams.OutlookMiddleware.Common.Helpers
{
    /// <summary>
    /// Helper class for all rest calls
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class RestHelper<T> : IRestHelper<T>
    {
        #region <| Private Members |>

        private readonly IEdreamsConfiguration _configuration;

        #endregion

        #region <| Construction |>

        public RestHelper(IEdreamsConfiguration configuration)
        {
            _configuration = configuration;
            ForceServiceToRunOverTls12();
        }

        #endregion

        #region <|Public Methods |>

        /// <summary>
        /// Method to check status
        /// </summary>
        /// <param name="resourceUrl"></param>
        /// <returns></returns>
        public async Task<HttpStatusCode> CheckState(string resourceUrl)
        {
            string webServiceEndPoint = string.Format($"{_configuration.EdreamsExtensibilityUrl}/{resourceUrl}");
            var webApiClient = new RestClient(webServiceEndPoint)
            {
                Authenticator = new NtlmAuthenticator()
            };
            var webApiRequest = new RestRequest(Method.GET);
            var apiResult = await webApiClient.ExecuteAsync<ApiResult>(webApiRequest);
            return apiResult.StatusCode;
        }

        /// <summary>
        /// Method to get content
        /// </summary>
        /// <param name="resourceUrl"></param>
        /// <param name="skipResponseCheck"></param>
        /// <returns></returns>
        public async Task<string> GetContent(string resourceUrl, bool skipResponseCheck = false)
        {
            string webServiceEndPoint = string.Format($"{_configuration.EdreamsExtensibilityUrl}/{resourceUrl}");
            var webApiClient = new RestClient(webServiceEndPoint)
            {
                Authenticator = new NtlmAuthenticator()
            };
            var webApiRequest = new RestRequest(Method.GET);
            webApiRequest.AddHeader(_configuration.EdreamsTokenKey, _configuration.EdreamsTokenValue);
            var roughResult = await webApiClient.ExecuteAsync(webApiRequest);

            GoodResponse(webApiRequest.Method, roughResult, skipResponseCheck);

            if (roughResult.StatusCode.Equals(HttpStatusCode.OK))
            {
                return roughResult.Content;
            }
            return null;
        }

        /// <summary>
        /// Method to get data
        /// </summary>
        /// <param name="resourceUrl"></param>
        /// <param name="rsParameter"></param>
        /// <param name="skipResponseCheck"></param>
        /// <returns></returns>
        public async Task<List<T>> GetData(string resourceUrl, RestParameter rsParameter, bool skipResponseCheck = false)
        {
            return await GetData(resourceUrl, new List<RestParameter> { rsParameter }, skipResponseCheck);
        }

        /// <summary>
        /// Method to get data with the following parameters
        /// </summary>
        /// <param name="resourceUrl"></param>
        /// <param name="parameters"></param>
        /// <param name="skipResponseCheck"></param>
        /// <returns></returns>
        public async Task<List<T>> GetData(string resourceUrl, List<RestParameter> parameters, bool skipResponseCheck = false)
        {
            string webServiceEndPoint = string.Format($"{_configuration.EdreamsExtensibilityUrl}/{resourceUrl}");
            var webApiClient = new RestClient(webServiceEndPoint)
            {
                Authenticator = new NtlmAuthenticator()
            };
            var webApiRequest = new RestRequest(Method.GET);

            if ((parameters.FirstOrDefault(x => x.Name.Equals(_configuration.EdreamsTokenKey, System.StringComparison.OrdinalIgnoreCase)) == null))
            {
                webApiRequest.AddHeader(_configuration.EdreamsTokenKey, _configuration.EdreamsTokenValue);
            }
            foreach (RestParameter parameter in parameters)
            {
                webApiRequest.AddParameter(parameter.Name, parameter.Value, parameter.Type);
            }
            var roughResult = await webApiClient.ExecuteAsync<ApiResult<List<T>>>(webApiRequest);
            GoodResponse(webApiRequest.Method, roughResult, skipResponseCheck);

            if (roughResult.StatusCode.Equals(HttpStatusCode.OK))
            {
                var queryResult = roughResult.Data;
                return queryResult.Content;
            }
            return null;
        }

        /// <summary>
        /// Method to get data
        /// </summary>
        /// <param name="resourceUrl"></param>
        /// <param name="skipResponseCheck"></param>
        /// <returns></returns>
        public async Task<List<T>> GetData(string resourceUrl, bool skipResponseCheck = false)
        {
            string webServiceEndPoint = string.Format($"{_configuration.EdreamsExtensibilityUrl}/{resourceUrl}");
            var webApiClient = new RestClient(webServiceEndPoint)
            {
                Authenticator = new NtlmAuthenticator()
            };
            var webApiRequest = new RestRequest(Method.GET);
            webApiRequest.AddHeader(_configuration.EdreamsTokenKey, _configuration.EdreamsTokenValue);
            var roughResult = await webApiClient.ExecuteAsync<ApiResult<List<T>>>(webApiRequest);

            GoodResponse(webApiRequest.Method, roughResult, skipResponseCheck);

            var queryResult = roughResult.Data;

            if (queryResult == null)
            {
                List<T> content = Newtonsoft.Json.JsonConvert.DeserializeObject<List<T>>(roughResult.Content);
                return content;
            }
            return queryResult.Content;
        }

        /// <summary>
        /// Method to get data count
        /// </summary>
        /// <param name="resourceUrl"></param>
        /// <param name="skipResponseCheck"></param>
        /// <returns></returns>
        public async Task<int> CountData(string resourceUrl, bool skipResponseCheck = false)
        {
            string webServiceEndPoint = string.Format($"{_configuration.EdreamsExtensibilityUrl}/{resourceUrl}");
            var webApiClient = new RestClient(webServiceEndPoint)
            {
                Authenticator = new NtlmAuthenticator()
            };
            var webApiRequest = new RestRequest(Method.HEAD);
            webApiRequest.AddHeader(_configuration.EdreamsTokenKey, _configuration.EdreamsTokenValue);
            var roughResult = await webApiClient.ExecuteAsync<ApiResult<List<T>>>(webApiRequest);

            GoodResponse(webApiRequest.Method, roughResult, skipResponseCheck);

            var count = roughResult.Headers.Where(x => x.Name.Equals("X-total-count")).Select(y => y.Value);

            return count.Any() ? Convert.ToInt32(count.ToList()[0]) : 0;

        }

        /// <summary>
        /// Method to create new object
        /// </summary>
        /// <param name="resourceUrl"></param>
        /// <param name="objectToCreate"></param>
        /// <param name="skipResponseCheck"></param>
        /// <returns></returns>
        public async Task<ApiResult<T>> CreateNew(string resourceUrl, T objectToCreate, bool skipResponseCheck = false)
        {
            string webServiceEndPoint = string.Format($"{_configuration.EdreamsExtensibilityUrl}/{resourceUrl}");
            var webApiClient = new RestClient(webServiceEndPoint)
            {
                Authenticator = new NtlmAuthenticator()
            };
            List<RestParameter> parameter = new List<RestParameter>();
            return await CreateNew(webApiClient, objectToCreate, parameter, skipResponseCheck);
        }

        /// <summary>
        /// Method to create new object with the following parameters
        /// </summary>
        /// <param name="resourceUrl"></param>
        /// <param name="objectToCreate"></param>
        /// <param name="parameter"></param>
        /// <param name="skipResponseCheck"></param>
        /// <returns></returns>
        public async Task<ApiResult<T>> CreateNew(string resourceUrl, T objectToCreate, RestParameter parameter, bool skipResponseCheck = false)
        {
            string webServiceEndPoint = string.Format($"{_configuration.EdreamsExtensibilityUrl}/{resourceUrl}");
            var webApiClient = new RestClient(webServiceEndPoint)
            {
                Authenticator = new NtlmAuthenticator()
            };
            return await CreateNew(webApiClient, objectToCreate, new List<RestParameter> { parameter }, skipResponseCheck);
        }

        // TODO : Need to check with Johnny/Sasi wheather this method required / customize existing method.   

        /// <summary>
        /// Method to create new object
        /// </summary>
        /// <param name="resourceUrl"></param>
        /// <param name="skipResponseCheck"></param>
        /// <param name="fileParameter"></param>
        /// <returns></returns>
        public async Task<ApiResult<T>> CreateFile(string resourceUrl, FileParameter fileParameter, bool skipResponseCheck = false)
        {
            string webServiceEndPoint = string.Format($"{_configuration.EdreamsExtensibilityUrl}/{resourceUrl}");
            var webApiClient = new RestClient(webServiceEndPoint)
            {
                Authenticator = new NtlmAuthenticator()
            };
            var webApiRequest = new RestRequest(Method.POST);
            webApiRequest.AddHeader(_configuration.EdreamsTokenKey, _configuration.EdreamsTokenValue);
            webApiRequest.Files.Add(fileParameter);
            var roughResult = await webApiClient.ExecuteAsync<ApiResult<T>>(webApiRequest);
            GoodResponse(webApiRequest.Method, roughResult, skipResponseCheck);

            var queryResult = roughResult.Data;
            return queryResult;
        }

        /// <summary>
        /// Method to create new object
        /// </summary>
        /// <param name="resourceUrl"></param>
        /// <param name="skipResponseCheck"></param>
        /// <returns></returns>
        public async Task<List<T>> CreateNew(string resourceUrl, bool skipResponseCheck = false)
        {
            string webServiceEndPoint = string.Format($"{_configuration.EdreamsExtensibilityUrl}/{resourceUrl}");
            var webApiClient = new RestClient(webServiceEndPoint)
            {
                Authenticator = new NtlmAuthenticator()
            };
            var webApiRequest = new RestRequest(Method.POST);
            webApiRequest.AddHeader(_configuration.EdreamsTokenKey, _configuration.EdreamsTokenValue);
            var roughResult = await webApiClient.ExecuteAsync<ApiResult<List<T>>>(webApiRequest);
            GoodResponse(webApiRequest.Method, roughResult, skipResponseCheck);

            var queryResult = roughResult.Data;

            if (queryResult == null)
            {
                List<T> content = Newtonsoft.Json.JsonConvert.DeserializeObject<List<T>>(roughResult.Content);
                return content;
            }
            return queryResult.Content;
        }
        /// <summary>
        /// Method to create bulk objects
        /// </summary>
        /// <param name="resourceUrl"></param>
        /// <param name="objectsToCreate"></param>
        /// <param name="skipResponseCheck"></param>
        /// <returns></returns>
        public async Task<ApiResult<T>> CreateBulkNew(string resourceUrl, List<T> objectsToCreate, bool skipResponseCheck = false)
        {
            string webServiceEndPoint = string.Format($"{_configuration.EdreamsExtensibilityUrl}/{resourceUrl}");
            var webApiClient = new RestClient(webServiceEndPoint)
            {
                Authenticator = new NtlmAuthenticator()
            };
            List<RestParameter> parameter = new List<RestParameter>();
            return await CreateBulkNew(webApiClient, objectsToCreate, parameter, skipResponseCheck);
        }

        /// <summary>
        /// Method to create a transaction
        /// </summary>
        /// <returns></returns>
        public async Task<string> CreateNewTransaction(string resourceUrl, bool skipResponseCheck = false)
        {
            string webServiceEndPoint = string.Format($"{_configuration.EdreamsExtensibilityUrl}/{resourceUrl}");
            var webApiClient = new RestClient(webServiceEndPoint)
            {
                Authenticator = new NtlmAuthenticator()
            };
            var webApiRequest = new RestRequest(Method.POST);
            webApiRequest.AddHeader(_configuration.EdreamsTokenKey, _configuration.EdreamsTokenValue);
            var roughResult = await webApiClient.ExecuteAsync<ApiResult<List<T>>>(webApiRequest);
            GoodResponse(webApiRequest.Method, roughResult, skipResponseCheck);

            if (roughResult.StatusCode.Equals(HttpStatusCode.OK))
            {
                return roughResult.Content;
            }
            return null;
        }

        /// <summary>
        /// Method to update object
        /// </summary>
        /// <param name="resourceUrl"></param>
        /// <param name="parameter"></param>
        /// <param name="objectToUpdate"></param>
        /// <param name="skipResponseCheck"></param>
        /// <returns></returns>
        public async Task<ApiResult<T>> Update(string resourceUrl, RestParameter parameter, T objectToUpdate, bool skipResponseCheck = false)
        {
            string webServiceEndPoint = string.Format($"{_configuration.EdreamsExtensibilityUrl}/{resourceUrl}");
            var webApiClient = new RestClient(webServiceEndPoint)
            {
                Authenticator = new NtlmAuthenticator()
            };
            return await Update(webApiClient, new List<RestParameter> { parameter }, objectToUpdate, skipResponseCheck);
        }

        /// <summary>
        /// Method to update object
        /// </summary>
        /// <param name="resourceUrl"></param>
        /// <param name="parameters"></param>
        /// <param name="objectToUpdate"></param>
        /// <param name="skipResponseCheck"></param>
        /// <returns></returns>
        public async Task<ApiResult<T>> Update(string resourceUrl, List<RestParameter> parameters, T objectToUpdate, bool skipResponseCheck = false)
        {
            string webServiceEndPoint = string.Format($"{_configuration.EdreamsExtensibilityUrl}/{resourceUrl}");
            var webApiClient = new RestClient(webServiceEndPoint)
            {
                Authenticator = new NtlmAuthenticator()
            };
            return await Update(webApiClient, parameters, objectToUpdate, skipResponseCheck);
        }

        /// <summary>
        /// Method to patch object
        /// </summary>
        /// <param name="resourceUrl"></param>
        /// <param name="parameter"></param>
        /// <param name="objectToUpdate"></param>
        /// <param name="skipResponseCheck"></param>
        /// <returns></returns>
        public async Task<ApiResult<T>> Patch(string resourceUrl, RestParameter parameter, T objectToUpdate, bool skipResponseCheck = false)
        {
            string webServiceEndPoint = string.Format($"{_configuration.EdreamsExtensibilityUrl}/{resourceUrl}");
            var webApiClient = new RestClient(webServiceEndPoint)
            {
                Authenticator = new NtlmAuthenticator()
            };
            return await Patch(webApiClient, new List<RestParameter> { parameter }, objectToUpdate, skipResponseCheck);
        }

        /// <summary>
        /// Method to patch object
        /// </summary>
        /// <param name="resourceUrl"></param>
        /// <param name="parameters"></param>
        /// <param name="objectToUpdate"></param>
        /// <param name="skipResponseCheck"></param>
        /// <returns></returns>
        public async Task<ApiResult<T>> Patch(string resourceUrl, List<RestParameter> parameters, T objectToUpdate, bool skipResponseCheck = false)
        {
            string webServiceEndPoint = string.Format($"{_configuration.EdreamsExtensibilityUrl}/{resourceUrl}");
            var webApiClient = new RestClient(webServiceEndPoint)
            {
                Authenticator = new NtlmAuthenticator()
            };
            return await Patch(webApiClient, parameters, objectToUpdate, skipResponseCheck);
        }

        /// <summary>
        /// Method to delete object
        /// </summary>
        /// <param name="resourceUrl"></param>
        /// <param name="parameter"></param>
        /// <param name="skipResponseCheck"></param>
        /// <returns></returns>
        public async Task<bool> Delete(string resourceUrl, RestParameter parameter, bool skipResponseCheck = false)
        {
            return await Delete(resourceUrl, new List<RestParameter> { parameter }, skipResponseCheck);
        }

        /// <summary>
        /// Method to delete object
        /// </summary>
        /// <param name="resourceUrl"></param>
        /// <param name="parameters"></param>
        /// <param name="skipResponseCheck"></param>
        /// <returns></returns>
        public async Task<bool> Delete(string resourceUrl, List<RestParameter> parameters, bool skipResponseCheck = false)
        {
            string webServiceEndPoint = string.Format($"{_configuration.EdreamsExtensibilityUrl}/{resourceUrl}");
            var webApiClient = new RestClient(webServiceEndPoint)
            {
                Authenticator = new NtlmAuthenticator()
            };
            var webApiRequest = new RestRequest(Method.DELETE)
            {
                RequestFormat = DataFormat.Json
            };
            if ((parameters.FirstOrDefault(x => x.Name.Equals(_configuration.EdreamsTokenKey, System.StringComparison.OrdinalIgnoreCase)) == null))
            {
                webApiRequest.AddHeader(_configuration.EdreamsTokenKey, _configuration.EdreamsTokenValue);
            }
            foreach (RestParameter parameter in parameters)
            {
                webApiRequest.AddParameter(parameter.Name, parameter.Value, parameter.Type);
            }
            var roughResult = await webApiClient.ExecuteAsync<ApiResult<T>>(webApiRequest);
            GoodResponse(webApiRequest.Method, roughResult, skipResponseCheck);
            return (roughResult.StatusCode == HttpStatusCode.OK);
        }

        #endregion

        #region <|Private Methods |>

        private async Task<ApiResult<T>> CreateNew(RestClient restClient, T objectToCreate, List<RestParameter> parameters, bool skipResponseCheck)
        {
            var webApiRequest = new RestRequest(Method.POST)
            {
                RequestFormat = DataFormat.Json
            };
            webApiRequest.AddJsonBody(objectToCreate);

            if ((parameters.FirstOrDefault(x => x.Name.Equals(_configuration.EdreamsTokenKey, System.StringComparison.OrdinalIgnoreCase)) == null))
            {
                webApiRequest.AddHeader(_configuration.EdreamsTokenKey, _configuration.EdreamsTokenValue);
            }
            foreach (RestParameter parameter in parameters)
            {
                webApiRequest.AddParameter(parameter.Name, parameter.Value, parameter.Type);
            }
            var roughResult = await restClient.ExecuteAsync<ApiResult<T>>(webApiRequest);
            GoodResponse(webApiRequest.Method, roughResult, skipResponseCheck);
            return roughResult.Data;
        }

        private async Task<ApiResult<T>> CreateBulkNew(RestClient restClient, List<T> objectToCreate, List<RestParameter> parameters, bool skipResponseCheck)
        {
            var webApiRequest = new RestRequest(Method.POST)
            {
                RequestFormat = DataFormat.Json
            };
            webApiRequest.AddJsonBody(objectToCreate);

            if ((parameters.FirstOrDefault(x => x.Name.Equals(_configuration.EdreamsTokenKey, System.StringComparison.OrdinalIgnoreCase)) == null))
            {
                webApiRequest.AddHeader(_configuration.EdreamsTokenKey, _configuration.EdreamsTokenValue);
            }
            foreach (RestParameter parameter in parameters)
            {
                webApiRequest.AddParameter(parameter.Name, parameter.Value, parameter.Type);
            }
            var roughResult = await restClient.ExecuteAsync<ApiResult<T>>(webApiRequest);
            GoodResponse(webApiRequest.Method, roughResult, skipResponseCheck);
            return roughResult.Data;
        }

        private async Task<ApiResult<T>> Update(RestClient restClient, List<RestParameter> parameters, T objectToUpdate, bool skipResponseCheck)
        {
            var webApiRequest = new RestRequest(Method.PUT)
            {
                RequestFormat = DataFormat.Json
            };
            webApiRequest.AddJsonBody(objectToUpdate);
            if ((parameters.FirstOrDefault(x => x.Name.Equals(_configuration.EdreamsTokenKey, System.StringComparison.OrdinalIgnoreCase)) == null))
            {
                webApiRequest.AddHeader(_configuration.EdreamsTokenKey, _configuration.EdreamsTokenValue);
            }
            foreach (RestParameter parameter in parameters)
            {
                webApiRequest.AddParameter(parameter.Name, parameter.Value, parameter.Type);
            }

            var roughResult = await restClient.ExecuteAsync<ApiResult<T>>(webApiRequest);
            GoodResponse(webApiRequest.Method, roughResult, skipResponseCheck);
            return roughResult.Data;
        }

        private async Task<ApiResult<T>> Patch(RestClient restClient, List<RestParameter> parameters, T objectToUpdate, bool skipResponseCheck)
        {
            var webApiRequest = new RestRequest(Method.PATCH)
            {
                RequestFormat = DataFormat.Json
            };
            webApiRequest.AddJsonBody(objectToUpdate);
            if ((parameters.FirstOrDefault(x => x.Name.Equals(_configuration.EdreamsTokenKey, System.StringComparison.OrdinalIgnoreCase)) == null))
            {
                webApiRequest.AddHeader(_configuration.EdreamsTokenKey, _configuration.EdreamsTokenValue);
            }

            foreach (RestParameter parameter in parameters)
            {
                webApiRequest.AddParameter(parameter.Name, parameter.Value, parameter.Type);
            }
            var roughResult = await restClient.ExecuteAsync<ApiResult<T>>(webApiRequest);

            GoodResponse(webApiRequest.Method, roughResult, skipResponseCheck);

            return roughResult.Data;
        }

        private bool GoodResponse(Method method, IRestResponse result, bool skipResponseCheck)
        {
            if (result.StatusCode != HttpStatusCode.OK && (!string.IsNullOrEmpty(result.ErrorMessage) || result.ErrorException != null))
            {
                if (!skipResponseCheck)
                {
                    throw new EdreamsException(EdreamsExceptionCode.UnknownFault, $"Error in RestResponse Method[{method}] StatusCode[{result.StatusCode}] Message[{result.ErrorMessage}] Exception[{result.ErrorException}]");
                }
                return false;
            }
            if (result.StatusCode == HttpStatusCode.Forbidden || result.StatusCode == HttpStatusCode.BadRequest || result.StatusCode == HttpStatusCode.InternalServerError)
            {
                if (!skipResponseCheck)
                {
                    throw new EdreamsException(EdreamsExceptionCode.UnknownFault, $"[{result.StatusCode}] in RestResponse Method[{method}] StatusCode[{result.StatusCode}] Content[{result.Content}]");
                }
                return false;
            }
            return true;
        }

        public RestParameter GetParam(string name, string value, ParameterType paramType)
        {
            return new RestParameter() { Name = name, Value = value, Type = paramType };
        }

        /// <summary>
        /// With TLS 1.0 and 1.1 being depricated,
        /// need to force your .NET websites / services to run over TLS 1.2
        /// </summary>
        private void ForceServiceToRunOverTls12()
        {
            if (!ServicePointManager.SecurityProtocol.HasFlag(SecurityProtocolType.Tls12))
            {
                ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
            }
        }

        #endregion
    }
}
