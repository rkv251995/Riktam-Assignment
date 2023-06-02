using Newtonsoft.Json;

namespace Assignment.Models.Account
{
    public class LoginModel
    {
        [JsonProperty("email")]
        public string Email { get; set; } = string.Empty;

        [JsonProperty("password")]
        public string Password { get; set; } = string.Empty;
    }
}
