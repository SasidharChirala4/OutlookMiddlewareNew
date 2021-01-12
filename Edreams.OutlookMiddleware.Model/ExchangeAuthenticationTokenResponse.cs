using System.Text.Json.Serialization;

namespace Edreams.OutlookMiddleware.Model
{
   public class ExchangeAuthenticationTokenResponse
    {
       
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
    }
}
