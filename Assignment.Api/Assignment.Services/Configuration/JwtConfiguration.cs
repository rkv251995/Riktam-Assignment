using Assignment.Services.Configuration.Interface;

namespace Assignment.Services.Configuration
{
    public class JwtConfiguration : IJwtConfiguration
    {
        public string SecurityKey { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int ExpiresInDay { get; set; }
        public int RefreshTokenExpiresInDay { get; set; }
    }
}
