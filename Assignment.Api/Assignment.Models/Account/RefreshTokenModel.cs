using Newtonsoft.Json;

namespace Assignment.Models.Account
{
    public class RefreshTokenModel
    {
        [JsonProperty("accessToken")]
        public string AccessToken { get; set; } = string.Empty;

        [JsonProperty("refreshToken")]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
