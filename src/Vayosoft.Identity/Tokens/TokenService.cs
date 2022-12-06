using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Vayosoft.Commons.Entities;
using Vayosoft.Identity.Security;
using Vayosoft.Utilities;

namespace Vayosoft.Identity.Tokens
{
    public class TokenService : ITokenService<ClaimsPrincipal>
    {
        private readonly TokenSettings _appSettings;

        public TokenService(IOptions<TokenSettings> appSettings)
        {
            Guard.NotEmpty(appSettings.Value.Secret);
            _appSettings = appSettings.Value;
        }

        public string GenerateToken(IUser user, IEnumerable<SecurityRoleEntity> roles)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()?? throw new InvalidOperationException("The User has no Id"), ClaimValueTypes.Integer64),
                new(ClaimTypes.Name, user.Username, ClaimValueTypes.String),
                new(ClaimTypes.Email, user.Email ?? string.Empty, ClaimValueTypes.Email),
                new(UserClaimType.UserType, user.Type.ToString(), ClaimValueTypes.String),
                new(UserClaimType.ProviderId, $"{(user as IProviderId)?.ProviderId ?? 0}", ClaimValueTypes.Integer64)
            };

            claims.AddRange(roles.Select(role => new Claim(ClaimsIdentity.DefaultRoleClaimType, role.Id, ClaimValueTypes.String)));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(Math.Max(_appSettings.TokenExpires, 1)),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

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

        public ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            if (token == null)
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                if (validatedToken is not JwtSecurityToken jwtSecurityToken
                    || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    return new ClaimsPrincipal();
                }

                return principal;
            }
            catch
            {
                return new ClaimsPrincipal();
            }
        }

        public RefreshToken GenerateRefreshToken(string ipAddress)
        {
            var random = new byte[64];
            using var rng = RandomNumberGenerator.Create();

            var refreshToken = new RefreshToken
            {
                Token = GetUniqueToken(),
                Expires = DateTime.UtcNow.AddDays(Math.Max(_appSettings.RefreshTokenExpires, 1)),
                Created = DateTime.UtcNow,
                CreatedByIp = ipAddress
            };

            return refreshToken;

            string GetUniqueToken()
            {
                // token is a cryptographically strong random sequence of values
                rng.GetBytes(random);
                var token = Convert.ToBase64String(random);
                // ensure token is unique by checking against db
                //var tokenIsUnique = !_context.Users.Any(u => u.RefreshTokens.Any(t => t.Token == token));

                //if (!tokenIsUnique)
                // return GetUniqueToken();

                return token;
            }
        }
    }
}
