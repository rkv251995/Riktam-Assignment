namespace Assignment.Services.Configuration.Interface
{
    public interface IJwtConfiguration
    {
        string SecurityKey { get; set; }
        string Issuer { get; set; }
        string Audience { get; set; }
        int ExpiresInDay { get; set; }
        int RefreshTokenExpiresInDay { get; set; }
    }
}
