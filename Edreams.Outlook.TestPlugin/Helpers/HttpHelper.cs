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
        private static string _baseUrl = "https://localhost:44361";

        public static async Task<CreateMailResponse> CreateMail(CreateMailRequest createMailRequest)
        {
            RestClient client = new RestClient(_baseUrl);
            RestRequest request = new RestRequest("mails", Method.POST);
            request.AddJsonBody(createMailRequest);
            var response = await client.ExecuteAsync<CreateMailResponse>(request);

            return response.Data;
        }

        public static async Task CommitBatch(CommitBatchRequest commitBatchRequest)
        {
            RestClient client = new RestClient(_baseUrl);
            RestRequest request = new RestRequest($"/batches/{commitBatchRequest.BatchId}/commit", Method.POST);
            request.AddJsonBody(commitBatchRequest);
            var response = await client.ExecuteAsync<CommitBatchResponse>(request);
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

                message.Method = HttpMethod.Post;
                message.Content = content;
                return await clientHttp.SendAsync(message);
            }
        }
    }
}