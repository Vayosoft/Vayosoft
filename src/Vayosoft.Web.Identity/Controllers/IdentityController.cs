using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Vayosoft.Caching;
using Vayosoft.Identity.Authentication;
using Vayosoft.Utilities;
using Vayosoft.Web.Extensions;
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

            var authResult = await _authService.AuthenticateAsync(model.Email, model.Password, HttpContext.GetIpAddress(), cancellationToken);
            await HttpContext.Session.SetAsync("_roles", authResult.Roles);
            HttpContext.SetTokenCookie(authResult.RefreshToken, _env.IsProduction());
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

            var authResult = await _cache.GetOrCreateExclusiveAsync(CacheKey.With<TokenRequest>(refreshToken), async options =>
            {
                options.AbsoluteExpirationRelativeToNow = TimeSpans.Minute;
                var response = await _authService.RefreshTokenAsync(refreshToken, HttpContext.GetIpAddress(), cancellationToken);
                return response;
            });

            HttpContext.SetTokenCookie(authResult.RefreshToken, _env.IsProduction());
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

            await _authService.RevokeTokenAsync(refreshToken, HttpContext.GetIpAddress(), cancellationToken);
            return Ok(new { message = "Token revoked" });
        }
    }
}
