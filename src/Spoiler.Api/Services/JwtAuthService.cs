using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Spoiler.Api.Helpers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Spoiler.Api.Services
{
    public interface IJwtAuthService
    {
        (ClaimsPrincipal, JwtSecurityToken) DecodeJwtToken(string? token, bool validateLifeTime = true);
        JwtAuthResult GenerateTokens(string? userId, Claim[] claims, string? requestRefreshToken = null);
        RefreshToken RefreshToken(string userId, DateTime expiryDate, string token);
        void RevokeToken(string? userId, string? token);
    }

    public class JwtAuthService : IJwtAuthService
    {
        private static readonly Dictionary<string, List<RefreshToken>> _refreshTokens = new();
        private readonly JwtSettings _jwtSettings;
        private readonly byte[] _secret;

        /// <summary>
        /// ctor.
        /// </summary>
        /// <param name="jwtSettings"></param>
        public JwtAuthService(IOptionsMonitor<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.CurrentValue;
            _secret = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey!);
        }

        /// <summary>
        /// Generates new jwt.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="claims">The claims to be included in the jwt.</param>
        /// <param name="requestRefreshToken">The refresh token to revoke.</param>
        /// <returns>JWT result.</returns>
        public JwtAuthResult GenerateTokens(string? userId, Claim[] claims, string? requestRefreshToken = null)
        {
            DateTime now = DateTime.Now;

            var shouldAddAudienceClaim = string.IsNullOrWhiteSpace(claims?.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Aud)?.Value);
            var expiry = now.AddMinutes(_jwtSettings.AccessTokenLifetimeInMinutes);
            var refreshTokenExpiryDate = now.AddMinutes(_jwtSettings.RefereshTokenLifetimeInMintues);

            var jwtToken = new JwtSecurityToken(
                    _jwtSettings.Issuer,
                    shouldAddAudienceClaim ? _jwtSettings.Audiences : string.Empty,
                    claims,
                    expires: expiry,
                    signingCredentials: new SigningCredentials(new SymmetricSecurityKey(_secret), SecurityAlgorithms.HmacSha256Signature));
            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            var refreshToken = RefreshToken(userId, refreshTokenExpiryDate, requestRefreshToken);

            return new JwtAuthResult
            {
                AccessToken = accessToken,
                ExpiryDate = expiry,
                RefreshToken = refreshToken.Token,
                RefreshTokenExpiryDate = refreshToken.Expires
            };
        }

        /// <summary>
        /// The jwt decoder.
        /// </summary>
        /// <param name="token">The jwt to be decoded.</param>
        /// <param name="validateLifeTime">A flag to validate the lifetime of the token or not. Default is true.</param>
        /// <returns>A tuple of the included Claims and security token.</returns>
        public (ClaimsPrincipal, JwtSecurityToken) DecodeJwtToken(string? token, bool validateLifeTime = true)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new BusinessException("Please provide a valid access token!");

            ClaimsPrincipal principal;

            principal = new JwtSecurityTokenHandler()
                .ValidateToken(token,
                new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(_secret),
                    ValidAudience = _jwtSettings.Audiences,
                    ValidateAudience = true,
                    ValidateLifetime = validateLifeTime,
                    ClockSkew = TimeSpan.FromMinutes(1)
                }, out SecurityToken validatedToken);

            return (principal, validatedToken as JwtSecurityToken)!;
        }

        public RefreshToken RefreshToken(string? userId, DateTime expiryDate, string token)
        {

            var newRefreshToken = GenerateRefreshToken(userId, expiryDate);

            var atLeastOneTokenExists = _refreshTokens.TryGetValue(userId, out var userTokens);

            userTokens ??= new List<RefreshToken>();

            if (!string.IsNullOrEmpty(token))
            {
                var refreshToken = userTokens?.Where(a => a.UserId == userId && a.Token == token).FirstOrDefault();

                if (refreshToken is null)
                    throw new BusinessException("Refresh token is null or already revoked!");

                if (!refreshToken.IsActive)
                    throw new BusinessException("Refresh token is already revoked!");

                userTokens?.Remove(refreshToken);
            }

            userTokens?.Add(newRefreshToken);

            if (!atLeastOneTokenExists)
                _refreshTokens.TryAdd(userId, userTokens!);

            return newRefreshToken;
        }

        public void RevokeToken(string? userId, string? token)
        {
            _refreshTokens.TryGetValue(userId, out List<RefreshToken>? refreshTokens);

            if (refreshTokens is null || !refreshTokens.Any())
                throw new BusinessException("Referesh token not found!");

            var tokenToRevoke = refreshTokens.Where(a => a.Token == token).FirstOrDefault();
            if (tokenToRevoke == null)
                return;

            if (!tokenToRevoke.IsActive)
                throw new BusinessException("Refresh token is already revoked!");

            tokenToRevoke.Revoked = DateTime.Now;
        }

        private RefreshToken GenerateRefreshToken(string? userId, DateTime expiryDate)
        {
            using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                var randomBytes = new byte[64];
                rngCryptoServiceProvider.GetBytes(randomBytes);
                return new RefreshToken
                {
                    Token = Convert.ToBase64String(randomBytes),
                    Expires = expiryDate,
                    UserId = userId
                };
            }
        }
    }
}
