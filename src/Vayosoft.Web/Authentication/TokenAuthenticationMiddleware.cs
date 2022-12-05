using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Vayosoft.Identity.Tokens;

namespace Vayosoft.Web.Authentication
{
    public class TokenAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly TokenSettings _tokenSettings;

        public TokenAuthenticationMiddleware(RequestDelegate next, IOptions<TokenSettings> appSettings)
        {
            _next = next;
            _tokenSettings = appSettings.Value;
        }

        public async Task Invoke(HttpContext context, ITokenService<ClaimsPrincipal> tokenService)
        {
            string token;
            var authorizationHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            if (authorizationHeader != null)
            {
                token = authorizationHeader.Split(" ").Last();
            }
            else
            {
                token = context.Request.Query["access_token"];
            }

            if (!string.IsNullOrEmpty(token))
            {
                context.User = tokenService.GetPrincipalFromToken(token);
            }

            await _next(context);
        }
    }
}
