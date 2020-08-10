using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Edreams.OutlookMiddleware.DataTransferObjects.Api;
using RestSharp;

namespace Edreams.Outlook.TestPlugin.Helpers
{
    public static class HttpHelper
    {
        private static string _baseUrl = "http://localhost:65290";

        public static async Task<CreateMailResponse> CreateMail(CreateMailRequest createMailRequest)
        {
            RestClient client = new RestClient(_baseUrl);
            RestRequest request = new RestRequest("mails", Method.POST);
            request.AddJsonBody(createMailRequest);
            var response = await client.ExecuteAsync<CreateMailResponse>(request);

            return response.Data;
        }

        public static async Task CommitBatch(Guid batchId)
        {
            RestClient client = new RestClient(_baseUrl);
            RestRequest request = new RestRequest($"batches/{batchId}/commit", Method.POST);
            var response = await client.ExecuteAsync<CancelBatchResponse>(request);
        }

        public static async Task CancelBatch(Guid batchId)
        {
            RestClient client = new RestClient(_baseUrl);
            RestRequest request = new RestRequest($"batches/{batchId}/cancel", Method.DELETE);
            var response = await client.ExecuteAsync<CancelBatchResponse>(request);
        }

        public static async Task<HttpResponseMessage> UploadAsync(Stream stream, string fileName, Guid fileId)
        {
            using (HttpClient clientHttp = new HttpClient(new HttpClientHandler() { UseDefaultCredentials = true }))
            {
                clientHttp.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                MultipartFormDataContent content = new MultipartFormDataContent();
                HttpRequestMessage message = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri($"{_baseUrl}/files/{fileId}")
                };
                content.Add(new StreamContent(stream), "file", fileName);

                message.Method = HttpMethod.Put;
                message.Content = content;
                return await clientHttp.SendAsync(message);
            }
        }
    }
}