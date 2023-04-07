using Microsoft.AspNetCore.Http;

namespace Vayosoft.Web.Extensions
{
    public static class HttpContextExtensions
    {
        public static void SetTokenCookie(this HttpContext context, string token, bool secure = true)
        {
            // append cookie with refresh token to the http response
            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.UtcNow.AddDays(7),

                // Set the secure flag, which Chrome's changes will require for SameSite none.
                // Note this will also require you to be running on HTTPS.
                Secure = secure,

                // Set the cookie to HTTP only which is good practice unless you really do need
                // to access it client side in scripts.
                HttpOnly = true,

                // Add the SameSite attribute, this will emit the attribute with a value of none.
                SameSite = SameSiteMode.Strict
                //SameSite = SameSiteMode.None

                // The client should follow its default cookie policy.
                // SameSite = SameSiteMode.Unspecified
            };
            context.Response.Cookies.Append("refreshToken", token, cookieOptions);
        }

        public static string GetIpAddress(this HttpContext context)
        {
            // get source ip address for the current request
            if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var header))
                return header;
            else
                return context.Connection.RemoteIpAddress!.MapToIPv4().ToString();
        }
    }
}
