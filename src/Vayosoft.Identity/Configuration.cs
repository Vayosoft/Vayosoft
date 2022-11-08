using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Vayosoft.Identity
{
    public static class Configuration
    {
        //https://blog.devgenius.io/jwt-authentication-in-asp-net-core-e67dca9ae3e8
        public static IServiceCollection AddTokenAuthentication(this IServiceCollection services, string symmetricKey)
        {
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(symmetricKey));
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.IncludeErrorDetails = true; // <- for debugging

                    options.TokenValidationParameters =
                        new TokenValidationParameters
                        {
                            ValidateActor = false,

                            ValidateAudience = false,
                            ValidAudience = "Vayosoft",

                            ValidateIssuer = false,
                            ValidIssuer = "Vayosoft",

                            RequireExpirationTime = true, // <- JWTs are required to have "exp" property set
                            ValidateLifetime = true, // <- the "exp" will be validated

                            RequireSignedTokens = true,
                            IssuerSigningKey = signingKey,
                        };
                });

            return services;
        }
        
        public static IServiceCollection AddTokenAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy(JwtBearerDefaults.AuthenticationScheme, policy =>
                {
                    policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
                    policy.RequireClaim(ClaimTypes.Name);
                });
            });

            return services;
        }
    }
}
