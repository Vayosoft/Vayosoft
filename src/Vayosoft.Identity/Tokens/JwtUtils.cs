using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Vayosoft.Identity.Tokens
{
    public static class JwtUtils
    {
        public static TokenResult GenerateToken(string signingKey, IEnumerable<Claim> claims, TimeSpan timeout)
        {
            var signingCredentials = new SigningCredentials(
                key: new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
                algorithm: SecurityAlgorithms.HmacSha256);

            var jwtDate = DateTime.Now;

            var jwt = new JwtSecurityToken(
                audience: "Vayosoft", // must match the audience in AddJwtBearer()
                issuer: "Vayosoft", // must match the issuer in AddJwtBearer()

                // Add whatever claims you'd want the generated token to include
                claims: claims ?? new List<Claim> {
                    new(ClaimTypes.Name, "default"),
                },
                notBefore: jwtDate,
                expires: jwtDate.Add(timeout), // Should be short lived. For logins, it's may be fine to use 24h

                // Provide a cryptographic key used to sign the token.
                // When dealing with symmetric keys then this must be
                // the same key used to validate the token.
                signingCredentials: signingCredentials
            );

            // Generate the actual token as a string
            var token = new JwtSecurityTokenHandler().WriteToken(jwt);

            // Return some agreed upon or documented structure.
            return new TokenResult
            {
                Token = token,
                // Even if the expiration time is already a part of the token, it's common to be 
                // part of the response body.
                UnixTimeExpiresAt = new DateTimeOffset(jwtDate).ToUnixTimeMilliseconds()
            };
        }
    }
}
