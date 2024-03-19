using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Vayosoft.Web.Identity.Authentication
{
    /// <summary>
    /// https://referbruv.com/blog/implementing-custom-authentication-scheme-and-handler-in-aspnet-core-3x/
    /// </summary>
    public class CustomAuthenticationHandler : AuthenticationHandler<CustomSchemeOptions>
    {
        public CustomAuthenticationHandler(IOptionsMonitor<CustomSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder)
            : base(options, logger, encoder)
        { }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            throw new NotImplementedException();
        }
    }
}
