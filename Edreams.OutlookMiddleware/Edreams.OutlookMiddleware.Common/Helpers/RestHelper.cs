using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Edreams.Contracts.Data.Common;
using System.Threading.Tasks;

namespace Edreams.OutlookMiddleware.Common.Helpers
{
    public sealed class RestHelper<T>
    {
        private readonly string _webserviceEndPoint;
        private readonly string _edreamsToken;
        private readonly bool _skipResponseCheck;

        public RestHelper(string webserviceEndPoint, string edreamsToken)
        {
            _webserviceEndPoint = webserviceEndPoint;
            _edreamsToken = edreamsToken;
            _skipResponseCheck = false;
            ForceServiceToRunOverTls12();
        }

        public RestHelper(string webserviceEndPoint, string edreamsToken, bool skipResponseCheck)
        {
            _webserviceEndPoint = webserviceEndPoint;
            _edreamsToken = edreamsToken;
            _skipResponseCheck = skipResponseCheck;
            ForceServiceToRunOverTls12();
        }

        public async Task<HttpStatusCode> CheckState()
        {
            var webApiClient = new RestClient(_webserviceEndPoint)
            {
                Authenticator = new NtlmAuthenticator()
            };
            var webApiRequest = new RestRequest(Method.GET);
            var apiResult = await webApiClient.ExecuteAsync<ApiResult>(webApiRequest);
            return apiResult.StatusCode;
        }

        public async Task<string> GetContent()
        {
            var webApiClient = new RestClient(_webserviceEndPoint)
            {
                Authenticator = new NtlmAuthenticator()
            };
            var webApiRequest = new RestRequest(Method.GET);
            webApiRequest.AddHeader("edreams-token", _edreamsToken);
            var roughResult = await webApiClient.ExecuteAsync(webApiRequest);

            GoodResponse(webApiRequest.Method, roughResult);

            if (roughResult.StatusCode.Equals(HttpStatusCode.OK))
            {
                return roughResult.Content;
            }
            return null;
        }

        public async Task<List<T>> GetData()
        {
            var webApiClient = new RestClient(_webserviceEndPoint)
            {
                Authenticator = new NtlmAuthenticator()
            };
            var webApiRequest = new RestRequest(Method.GET);
            webApiRequest.AddHeader("edreams-token", _edreamsToken);
            var roughResult = await webApiClient.ExecuteAsync<ApiResult<List<T>>>(webApiRequest);

            GoodResponse(webApiRequest.Method, roughResult);

            var queryResult = roughResult.Data;

            if (queryResult == null)
            {
                List<T> content = Newtonsoft.Json.JsonConvert.DeserializeObject<List<T>>(roughResult.Content);
                return content;
            }
            return queryResult.Content;
        }

        public async Task<int> CountData()
        {
            var webApiClient = new RestClient(_webserviceEndPoint)
            {
                Authenticator = new NtlmAuthenticator()
            };
            var webApiRequest = new RestRequest(Method.HEAD);
            webApiRequest.AddHeader("edreams-token", _edreamsToken);
            var roughResult = await webApiClient.ExecuteAsync<ApiResult<List<T>>>(webApiRequest);

            GoodResponse(webApiRequest.Method, roughResult);

            var count = roughResult.Headers.Where(x => x.Name.Equals("X-total-count")).Select(y => y.Value);

            return count.Any() ? Convert.ToInt32(count.ToList()[0]) : 0;

        }

        public async Task<List<T>> GetData(RestParameter rsParameter)
        {
            return await GetData(new List<RestParameter> { rsParameter });
        }

        public async Task<List<T>> GetData(List<RestParameter> parameters)
        {
            var webApiClient = new RestClient(_webserviceEndPoint)
            {
                Authenticator = new NtlmAuthenticator()
            };
            var webApiRequest = new RestRequest(Method.GET);

            if ((parameters.FirstOrDefault(x => x.Name.Equals("edreams-token", System.StringComparison.OrdinalIgnoreCase)) == null))
            {
                webApiRequest.AddParameter("edreams-token", _edreamsToken, ParameterType.HttpHeader);
            }
            foreach (RestParameter parameter in parameters)
            {
                webApiRequest.AddParameter(parameter.Name, parameter.Value, parameter.Type);
            }
            var roughResult = await webApiClient.ExecuteAsync<ApiResult<List<T>>>(webApiRequest);
            //System.Diagnostics.Trace.WriteLine($"Environment.[{Environment.UserName}] WindowsIdentity[{System.Security.Principal.WindowsIdentity.GetCurrent().Name}]");
            GoodResponse(webApiRequest.Method, roughResult);

            if (roughResult.StatusCode.Equals(HttpStatusCode.OK))
            {
                var queryResult = roughResult.Data;
                return queryResult.Content;
            }
            return null;
        }

        public async Task<ApiResult<T>> CreateNew(T objectToCreate)
        {
            var webApiClient = new RestClient(_webserviceEndPoint)
            {
                Authenticator = new NtlmAuthenticator()
            };
            List<RestParameter> parameter = new List<RestParameter>();
            return await CreateNew(webApiClient, objectToCreate, parameter);
        }

        public async Task<ApiResult<T>> CreateBulkNew(List<T> objectsToCreate)
        {
            var webApiClient = new RestClient(_webserviceEndPoint)
            {
                Authenticator = new NtlmAuthenticator()
            };
            List<RestParameter> parameter = new List<RestParameter>();
            return await CreateBulkNew(webApiClient, objectsToCreate, parameter);
        }

        public async Task<ApiResult<T>> CreateNew(T objectToCreate, RestParameter parameter)
        {
            var webApiClient = new RestClient(_webserviceEndPoint)
            {
                Authenticator = new NtlmAuthenticator()
            };
            return await CreateNew(webApiClient, objectToCreate, new List<RestParameter> { parameter });
        }

        public async Task<List<T>> CreateNew()
        {
            var webApiClient = new RestClient(_webserviceEndPoint)
            {
                Authenticator = new NtlmAuthenticator()
            };
            var webApiRequest = new RestRequest(Method.POST);
            webApiRequest.AddHeader("edreams-token", _edreamsToken);
            var roughResult = await webApiClient.ExecuteAsync<ApiResult<List<T>>>(webApiRequest);
            GoodResponse(webApiRequest.Method, roughResult);

            var queryResult = roughResult.Data;

            if (queryResult == null)
            {
                List<T> content = Newtonsoft.Json.JsonConvert.DeserializeObject<List<T>>(roughResult.Content);
                return content;
            }
            return queryResult.Content;
        }

        /// <summary>
        /// Method for creating a transaction
        /// </summary>
        /// <returns></returns>
        public async Task<string> CreateNewTransaction()
        {
            var webApiClient = new RestClient(_webserviceEndPoint)
            {
                Authenticator = new NtlmAuthenticator()
            };
            var webApiRequest = new RestRequest(Method.POST);
            webApiRequest.AddHeader("edreams-token", _edreamsToken);
            var roughResult = await webApiClient.ExecuteAsync<ApiResult<List<T>>>(webApiRequest);
            GoodResponse(webApiRequest.Method, roughResult);

            var queryResult = roughResult.Data;
            if (roughResult.StatusCode.Equals(HttpStatusCode.OK))
            {
                return roughResult.Content;
            }
            return null;
        }

        /// <summary>
        /// With TLS 1.0 and 1.1 being depricated,
        /// need to force your .NET websites / services to run over TLS 1.2
        /// </summary>
        public void ForceServiceToRunOverTls12()
        {
            if (ServicePointManager.SecurityProtocol.HasFlag(SecurityProtocolType.Tls12) == false)
            {
                ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
            }
        }

        private async Task<ApiResult<T>> CreateNew(RestClient restClient, T objectToCreate, List<RestParameter> parameters)
        {
            var webApiRequest = new RestRequest(Method.POST)
            {
                RequestFormat = DataFormat.Json
            };
            webApiRequest.AddJsonBody(objectToCreate);

            if ((parameters.FirstOrDefault(x => x.Name.Equals("edreams-token", System.StringComparison.OrdinalIgnoreCase)) == null))
            {
                webApiRequest.AddParameter("edreams-token", _edreamsToken, ParameterType.HttpHeader);
            }
            foreach (RestParameter parameter in parameters)
            {
                webApiRequest.AddParameter(parameter.Name, parameter.Value, parameter.Type);
            }
            var roughResult = await restClient.ExecuteAsync<ApiResult<T>>(webApiRequest);
            GoodResponse(webApiRequest.Method, roughResult);
            return roughResult.Data;
        }

        private async Task<ApiResult<T>> CreateBulkNew(RestClient restClient, List<T> objectToCreate, List<RestParameter> parameters)
        {
            var webApiRequest = new RestRequest(Method.POST);
            webApiRequest.RequestFormat = DataFormat.Json;
            webApiRequest.AddJsonBody(objectToCreate);

            if ((parameters.FirstOrDefault(x => x.Name.Equals("edreams-token", System.StringComparison.OrdinalIgnoreCase)) == null))
            {
                webApiRequest.AddParameter("edreams-token", _edreamsToken, ParameterType.HttpHeader);
            }
            foreach (RestParameter parameter in parameters)
            {
                webApiRequest.AddParameter(parameter.Name, parameter.Value, parameter.Type);
            }
            var roughResult = await restClient.ExecuteAsync<ApiResult<T>>(webApiRequest);
            GoodResponse(webApiRequest.Method, roughResult);
            return roughResult.Data;
        }

        public async Task<ApiResult<T>> Update(RestParameter parameter, T objectToUpdate)
        {
            var webApiClient = new RestClient(_webserviceEndPoint)
            {
                Authenticator = new NtlmAuthenticator()
            };
            return await Update(webApiClient, new List<RestParameter> { parameter }, objectToUpdate);
        }

        public async Task<ApiResult<T>> Patch(RestParameter parameter, T objectToUpdate)
        {
            var webApiClient = new RestClient(_webserviceEndPoint)
            {
                Authenticator = new NtlmAuthenticator()
            };
            return await Patch(webApiClient, new List<RestParameter> { parameter }, objectToUpdate);
        }

        public async Task<ApiResult<T>> Update(List<RestParameter> parameters, T objectToUpdate)
        {
            var webApiClient = new RestClient(_webserviceEndPoint)
            {
                Authenticator = new NtlmAuthenticator()
            };
            return await Update(webApiClient, parameters, objectToUpdate);
        }

        public async Task<ApiResult<T>> Patch(List<RestParameter> parameters, T objectToUpdate)
        {
            var webApiClient = new RestClient(_webserviceEndPoint)
            {
                Authenticator = new NtlmAuthenticator()
            };
            return await Patch(webApiClient, parameters, objectToUpdate);
        }

        private async Task<ApiResult<T>> Update(RestClient restClient, List<RestParameter> parameters, T objectToUpdate)
        {
            var webApiRequest = new RestRequest(Method.PUT)
            {
                RequestFormat = DataFormat.Json
            };
            webApiRequest.AddJsonBody(objectToUpdate);
            if ((parameters.FirstOrDefault(x => x.Name.Equals("edreams-token", System.StringComparison.OrdinalIgnoreCase)) == null))
            {
                webApiRequest.AddParameter("edreams-token", _edreamsToken, ParameterType.HttpHeader);
            }
            foreach (RestParameter parameter in parameters)
            {
                webApiRequest.AddParameter(parameter.Name, parameter.Value, parameter.Type);
            }

            var roughResult = await restClient.ExecuteAsync<ApiResult<T>>(webApiRequest);
            GoodResponse(webApiRequest.Method, roughResult);
            return roughResult.Data;
        }

        private async Task<ApiResult<T>> Patch(RestClient restClient, List<RestParameter> parameters, T objectToUpdate)
        {
            var webApiRequest = new RestRequest(Method.PATCH)
            {
                RequestFormat = DataFormat.Json
            };
            webApiRequest.AddJsonBody(objectToUpdate);
            if ((parameters.FirstOrDefault(x => x.Name.Equals("edreams-token", System.StringComparison.OrdinalIgnoreCase)) == null))
            {
                webApiRequest.AddParameter("edreams-token", _edreamsToken, ParameterType.HttpHeader);
            }

            foreach (RestParameter parameter in parameters)
            {
                webApiRequest.AddParameter(parameter.Name, parameter.Value, parameter.Type);
            }
            //webApiRequest. = "";
            var roughResult = await restClient.ExecuteAsync<ApiResult<T>>(webApiRequest);

            GoodResponse(webApiRequest.Method, roughResult);

            return roughResult.Data;
        }

        public async Task<bool> Delete(RestParameter parameter)
        {
            return await Delete(new List<RestParameter> { parameter });
        }

        public async Task<bool> Delete(List<RestParameter> parameters)
        {
            var webApiClient = new RestClient(_webserviceEndPoint)
            {
                Authenticator = new NtlmAuthenticator()
            };
            var webApiRequest = new RestRequest(Method.DELETE)
            {
                RequestFormat = DataFormat.Json
            };
            if ((parameters.FirstOrDefault(x => x.Name.Equals("edreams-token", System.StringComparison.OrdinalIgnoreCase)) == null))
            {
                webApiRequest.AddParameter("edreams-token", _edreamsToken, ParameterType.HttpHeader);
            }
            foreach (RestParameter parameter in parameters)
            {
                webApiRequest.AddParameter(parameter.Name, parameter.Value, parameter.Type);
            }
            var roughResult = await webApiClient.ExecuteAsync<ApiResult<T>>(webApiRequest);
            GoodResponse(webApiRequest.Method, roughResult);
            return (roughResult.StatusCode == HttpStatusCode.OK);
        }

        private bool GoodResponse(Method method, IRestResponse result)
        {
            if (result.StatusCode != HttpStatusCode.OK && (!string.IsNullOrEmpty(result.ErrorMessage) || result.ErrorException != null))
            {
                if (!_skipResponseCheck)
                {
                    throw new Exception($"Error in RestResponse Method[{method}] StatusCode[{result.StatusCode}] Message[{result.ErrorMessage}] Exception[{result.ErrorException}]");
                }
                return false;
            }
            if (result.StatusCode == HttpStatusCode.Forbidden || result.StatusCode == HttpStatusCode.BadRequest || result.StatusCode == HttpStatusCode.InternalServerError)
            {
                if (!_skipResponseCheck)
                {
                    throw new Exception($"[{result.StatusCode}] in RestResponse Method[{method}] StatusCode[{result.StatusCode}] Content[{result.Content}]");
                }
                return false;
            }
            return true;
        }

        public RestParameter GetParam(string name, string value, ParameterType paramType)
        {
            return new RestParameter() { Name = name, Value = value, Type = paramType };
        }
    }
}
