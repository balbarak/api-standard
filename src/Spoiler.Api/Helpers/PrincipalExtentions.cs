using IdentityModel;
using System.Security.Claims;
using System.Security.Principal;

namespace Spoiler.Api.Helpers
{
    public static class PrincipalExtentions
    {
        public static string? GetUserName(this IPrincipal principal)
        {
            string result = "";

            if (principal == null)
                return null;

            if (principal is ClaimsPrincipal identity)
            {
                var found = identity.FindFirst(a => a.Type == JwtClaimTypes.Name);

                if (found != null && !string.IsNullOrEmpty(found.Value))
                {
                    result = found.Value;

                    return result;
                }

                return null;

            }
            else
                return null;
        }

        public static string? GetUserId(this IPrincipal principal)
        {
            string result = "";

            if (principal == null)
                return null;

            if (principal is ClaimsPrincipal identity)
            {
                var found = identity.FindFirst(a => a.Type == ClaimTypes.NameIdentifier || a.Type == JwtClaimTypes.Subject);

                if (found != null && !string.IsNullOrEmpty(found.Value))
                {
                    result = found.Value;

                    return result;
                }

                return null;

            }
            else
                return null;
        }
    }
}
