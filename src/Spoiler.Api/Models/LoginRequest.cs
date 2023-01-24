using System.ComponentModel.DataAnnotations;

namespace Spoiler.Api.Models
{
    public record LoginRequest
    {
        [Required(ErrorMessage = "username is required")]
        public string? Username { get; set; }
        
        [Required(ErrorMessage = "password is required")]
        public string? Password { get; set; }
    }

    /// <summary>
    /// Refresh Token Dto
    /// </summary>
    public class RefreshTokenRequest
    {
        /// <summary>
        /// User Refresh Token
        /// </summary>
        [Required(ErrorMessage = "Refresh token is required.")]
        public string? RefreshToken { get; set; }
    }
}
