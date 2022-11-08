using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Vayosoft.Identity.Tokens;

namespace Vayosoft.Web.Authorization
{
    public class TokenAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly TokenSettings _tokenSettings;

        public TokenAuthorizationMiddleware(RequestDelegate next, IOptions<TokenSettings> appSettings)
        {
            _next = next;
            _tokenSettings = appSettings.Value;
        }

        public async Task Invoke(HttpContext context, ITokenService jwtUtils)
        {
            string token;
            if (context.Request.Path.ToString().StartsWith("/stream/"))
            {
                token = context.Request.Query["access_token"];
            }
            else
            {
                token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            }

            if (token != null)
            {
                var principal = jwtUtils.GetPrincipalFromToken(token);
                context.User = (ClaimsPrincipal)principal;
            }

            await _next(context);
        }
    }
}
