using IdentityModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Spoiler.Api.Helpers
{
    public class ClaimsHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user">the logged in user.</param>
        /// <returns></returns>
        public static Claim[] GetClaims(User? user)
        {
            if (user is null)
                return Array.Empty<Claim>();

            var roles = new[] {
                "Admin",
                //"Developer"
            };

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username!),
                new Claim(JwtClaimTypes.Name, user.Username!),
                new Claim(JwtClaimTypes.IssuedAt, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
                new Claim(JwtClaimTypes.Subject, user.Id!),
                new Claim(JwtClaimTypes.Email, user.Email ?? ""),
                new Claim(JwtRegisteredClaimNames.Amr, "pwd"),
            };

            claims.AddRange(roles.Select(role => new Claim(JwtClaimTypes.Role, role)));

            return claims.ToArray();
        }
    }
}
