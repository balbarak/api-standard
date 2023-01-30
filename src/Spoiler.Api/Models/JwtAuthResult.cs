using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Spoiler.Api.Models
{
    /// <summary>
    /// Token DTO.
    /// </summary>
    public record JwtAuthResult
    {
        /// <summary>
        /// Access Token.
        /// </summary>
        [JsonPropertyName("accessToken")]
        public string? AccessToken { get; set; }

        /// <summary>
        /// Token expiration date.
        /// </summary>
        [JsonPropertyName("expiryDate")]
        public DateTime? ExpiryDate { get; set; }

        /// <summary>
        /// Refresh Token
        /// </summary>
        [JsonPropertyName("refreshToken")]
        public string? RefreshToken { get; set; }

        /// <summary>
        /// Refresh Token Expiry date
        /// </summary>
        [JsonPropertyName("refreshTokenExpiryDate")]
        public DateTime? RefreshTokenExpiryDate { get; set; }
    }

    public class RefreshToken
    {
        public string? Token { get; set; }
        public DateTime? Expires { get; set; }
        public bool IsExpired => DateTime.Now >= Expires;
        public DateTime? Revoked { get; set; }
        public bool IsActive => Revoked == null && !IsExpired;
        public string? UserId { get; set; }
    }
}
