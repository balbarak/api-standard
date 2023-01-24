namespace Spoiler.Api.Models
{
    public class JwtSettings
    {
        public string? SecretKey { get; set; }
        public string? Issuer { get; set; }
        public string? Audiences { get; set; }
        public int AccessTokenLifetimeInMinutes { get; set; } = 60;
        public int RefereshTokenLifetimeInMintues { get; set; } = 43200;
    }
}
