using Newtonsoft.Json;

namespace Assignment.Dtos.Account
{
    public class LoginDto
    {
        [JsonProperty("accessToken")]
        public string AccessToken { get; set; } = string.Empty;

        [JsonProperty("refreshToken")]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
