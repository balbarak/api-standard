using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Spoiler.Api.Helpers;
using Spoiler.Api.Services;
using System.Security.Principal;

namespace Spoiler.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    [GeneralExceptionFilter]
    public class AccountController : ControllerBase
    {
        private readonly IJwtAuthService _jwtAuthService;
        
        public AccountController(IJwtAuthService jwtAuthService)
        {
            _jwtAuthService = jwtAuthService;
        }

        /// <summary>
        /// Login Endpoint
        /// </summary>
        /// <param name="credentials">User's credentials.</param>
        /// <returns>Jwt Token.</returns>
        /// <response code="200">Login Success, returns the Tokens information.</response>
        /// <response code="400">Login Failed, business logic/validation errors.</response>
        /// <response code="500">Login Failed, internal server error.</response>
        [ProducesResponseType(typeof(JwtAuthResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("login", Name = nameof(Login))]
        [AllowAnonymous]
        public IActionResult Login([FromBody] LoginRequest credentials)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(statusCode: StatusCodes.Status400BadRequest);

            var user = new User()
            {
                //Id = Guid.NewGuid().ToString(),
                Id = credentials.Username,
                Username = credentials.Username,
            };

            var jwtResult = _jwtAuthService.GenerateTokens(user.Id, ClaimsHelper.GetClaims(user)!);

            return Ok(jwtResult);
        }

        /// <summary>
        /// Refresh access token
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <response code="200">Returns the Tokens information.</response>
        /// <response code="400">Business logic/validation errors.</response>
        /// <response code="401">Not Authorized.</response>
        /// <response code="500">Internal server error.</response>
        /// <returns></returns>
        [ProducesResponseType(typeof(JwtAuthResult), StatusCodes.Status200OK)]
        [HttpPost("refresh-token", Name = nameof(RefreshToken))]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest refreshToken)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(statusCode: StatusCodes.Status400BadRequest);

            string? accessToken = await GetAccessTokenFromHeader();

            var (claimPrincipal, _) = _jwtAuthService.DecodeJwtToken(accessToken, validateLifeTime: false);

            var user = GetUserFromPrincipal(claimPrincipal);
            var claims = ClaimsHelper.GetClaims(user);

            var jwtResult = _jwtAuthService.GenerateTokens(user?.Id, claims, requestRefreshToken: refreshToken.RefreshToken);

            return Ok(jwtResult);
        }

        /// <summary>
        /// Revoke User's Refresh Token
        /// </summary>
        /// <param name="refreshToken">User's refresh token.</param>
        /// <response code="204">Returns Ok, success indicator.</response>
        /// <response code="400">Business logic/validation errors.</response>
        /// <response code="401">Not Authorized.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPost("revoke-token", Name = nameof(RevokeToken))]
        public async Task<IActionResult> RevokeToken([FromBody] RefreshTokenRequest refreshToken)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(statusCode: StatusCodes.Status400BadRequest);

            var accessToken = await GetAccessTokenFromHeader();
            var (claimPrincipal, _) = _jwtAuthService.DecodeJwtToken(accessToken);

            _jwtAuthService.RevokeToken(claimPrincipal.GetUserId(), refreshToken.RefreshToken);

            return NoContent();
        }

        private async Task<string?> GetAccessTokenFromHeader()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            if (string.IsNullOrEmpty(accessToken)
                && (Request.Headers.TryGetValue("Authorization", out StringValues accessTokenHeader)
                || Request.Headers.TryGetValue("access_token", out accessTokenHeader)))
            {
                accessToken = accessTokenHeader.ToString();

                if (!string.IsNullOrEmpty(accessTokenHeader))
                {
                    if (accessToken.Contains("bearer ", StringComparison.InvariantCultureIgnoreCase))
                        accessToken = accessToken["bearer ".Length..].Trim();
                }
            }

            return accessToken;
        }

        private static User? GetUserFromPrincipal(IPrincipal principal)
        {
            if (principal is null)
                return null;

            return new User() {
                //Id = principal.GetUserId(),
                Id = principal.GetUserName(),
                Username = principal.GetUserName()
            };
        }

    }
}
