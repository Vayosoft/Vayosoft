using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Vayosoft.Commands;
using Vayosoft.Identity;
using Vayosoft.Identity.Authentication;
using Vayosoft.Identity.EntityFramework;
using Vayosoft.Identity.Persistence;
using Vayosoft.Identity.Providers;
using Vayosoft.Identity.Security.Commands;
using Vayosoft.Identity.Tokens;
using Vayosoft.Persistence.Commands;

namespace Vayosoft.Web.Identity
{
    public static class Configuration
    {
        public static IServiceCollection AddIdentityService(this IServiceCollection services, IConfiguration configuration)
        {
            // configure strongly typed settings object
            services.Configure<TokenSettings>(configuration.GetSection("TokenSettings"));

            // configure DI for application services
            services.AddScoped<ITokenService<ClaimsPrincipal>, TokenService>();
            services.AddScoped<IPasswordHasher, MD5PasswordHasher>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();

            //builder.Services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("Over18",
            //        policy => policy.Requirements.Add(new Over18Requirement()));
            //});

            //Web.Identity
            services.AddUserService();
            services.AddSecurityService();
            services.AddProviderService();

            services.AddHttpContextAccessor()
                .AddScoped<IUserContext, UserContext>();

            return services;
        }

        private static IServiceCollection AddUserService(this IServiceCollection services) =>
            services
                //.AddQueryHandler<SpecificationQuery<GetUsersSpec, IPagedEnumerable<UserEntityDto>>, IPagedEnumerable<UserEntityDto>,
                //    PagingQueryHandler<string, GetUsersSpec, UserEntity, UserEntityDto>>()
                //.AddQueryHandler<SingleQuery<UserEntityDto>, UserEntityDto, SingleQueryHandler<long, UserEntity, UserEntityDto>>()

                .AddCommandHandler<SaveUser, HandleSaveUser>()
                .AddCommandHandler<DeleteCommand<UserEntity>, DeleteCommandHandler<UserEntity>>();

        private static IServiceCollection AddSecurityService(this IServiceCollection services) =>
            services
                //.AddQueryHandler<GetPermissions, RolePermissions, HandleGetPermissions>()

                .AddCommandHandler<SavePermissions, HandleSavePermissions>()
                .AddCommandHandler<SaveRole, HandleSaveRole>();

        private static IServiceCollection AddProviderService(this IServiceCollection services) =>
            services
                .AddCommandHandler<DeleteCommand<ProviderEntity>, DeleteCommandHandler<ProviderEntity>>()
                .AddCommandHandler<CreateOrUpdateCommand<ProviderEntity>, CreateOrUpdateHandler<long, ProviderEntity, ProviderEntity>>();

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
