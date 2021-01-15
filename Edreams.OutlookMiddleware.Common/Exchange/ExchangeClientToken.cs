using System.Text.Json.Serialization;

namespace Edreams.OutlookMiddleware.Common.Exchange
{
    public class ExchangeClientToken
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
    }
}