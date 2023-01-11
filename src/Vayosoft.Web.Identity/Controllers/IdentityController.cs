using LanguageExt.Pipes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Vayosoft.Caching;
using Vayosoft.Identity.Authentication;
using Vayosoft.Identity.Extensions;
using Vayosoft.Utilities;
using Vayosoft.Web.Identity.Authorization;
using Vayosoft.Web.Model;
using Vayosoft.Web.Model.Authentication;

namespace Vayosoft.Web.Identity.Controllers
{
    [PermissionAuthorization]
    [Produces("application/json")]
    [ProducesErrorResponseType(typeof(void))]
    [Route("api/identity")]
    [ApiController]
    [ApiVersion("1.0")]
    public sealed class IdentityController : ControllerBase
    {
        private readonly IAuthenticationService _authService;
        private readonly IDistributedMemoryCache _cache;
        private readonly IHostEnvironment _env;

        public IdentityController(IAuthenticationService authService, IDistributedMemoryCache cache, IHostEnvironment env)
        {
            _authService = authService;
            _cache = cache;
            _env = env;
        }

        [ProducesResponseType(typeof(AuthenticationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var authResult = await _authService.AuthenticateAsync(model.Email, model.Password, IpAddress(), cancellationToken);
            await HttpContext.Session.SetAsync("_roles", authResult.Roles);
            SetTokenCookie(authResult.RefreshToken);
            var response = new AuthenticationResponse(
                authResult.User.Username,
                authResult.Token,
                authResult.TokenExpirationTime);

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            //await HttpContext.SignOutAsync(".session");
            HttpContext.Session.Clear();
            await Task.CompletedTask;
            return Ok();
        }

        [ProducesResponseType(typeof(AuthenticationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(TokenRequest model, CancellationToken cancellationToken)
        {
            var refreshToken = model.Token ?? Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
            {
                ModelState.AddModelError(nameof(refreshToken), "Token is required.");
                return BadRequest(ModelState);
            }

            var authResult = await _cache.GetOrCreateExclusiveAsync(CacheKey.With<TokenRequest>(model.Token), async options =>
            {
                options.AbsoluteExpirationRelativeToNow = TimeSpans.Minute;
                var response = await _authService.RefreshTokenAsync(refreshToken, IpAddress(), cancellationToken);
                return response;
            });

            SetTokenCookie(authResult.RefreshToken);
            var response = new AuthenticationResponse(
                authResult.User.Username,
                authResult.Token,
                authResult.TokenExpirationTime);

            return Ok(response);
        }

        [ProducesResponseType(typeof(HttpErrorWrapper), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("revoke-token")]
        public async Task<IActionResult> RevokeToken(TokenRequest model, CancellationToken cancellationToken)
        {
            // accept refresh token in request body or cookie
            var refreshToken = model.Token ?? Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
            {
                ModelState.AddModelError(nameof(refreshToken), "Token is required.");
                return BadRequest(ModelState);
            }

            await _authService.RevokeTokenAsync(refreshToken, IpAddress(), cancellationToken);
            return Ok(new { message = "Token revoked" });
        }

        private void SetTokenCookie(string token)
        {
            // append cookie with refresh token to the http response
            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.UtcNow.AddDays(7),

                // Set the secure flag, which Chrome's changes will require for SameSite none.
                // Note this will also require you to be running on HTTPS.
                Secure = _env.IsProduction(),

                // Set the cookie to HTTP only which is good practice unless you really do need
                // to access it client side in scripts.
                HttpOnly = true,

                // Add the SameSite attribute, this will emit the attribute with a value of none.
                SameSite = SameSiteMode.Strict
                //SameSite = SameSiteMode.None

                // The client should follow its default cookie policy.
                // SameSite = SameSiteMode.Unspecified
            };
            Response.Cookies.Append(".refreshToken", token, cookieOptions);
        }

        private string IpAddress()
        {
            // get source ip address for the current request
            if (Request.Headers.TryGetValue("X-Forwarded-For", out var header))
                return header;
            else
                return HttpContext.Connection.RemoteIpAddress!.MapToIPv4().ToString();
        }

    }
}
