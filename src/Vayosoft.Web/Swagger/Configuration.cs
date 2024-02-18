using Asp.Versioning;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Vayosoft.Web.Swagger
{
    public static class Configuration
    {
        public static IServiceCollection AddSwaggerService(this IServiceCollection services)
        {
            services.AddSwaggerGen();
            services.ConfigureOptions<ConfigureSwaggerGenOptions>();

            services.AddApiVersioningService();

            return services;
        }

        public static IApplicationBuilder UseSwaggerService(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            //app.UseSwagger();
            app.UseSwagger(c => c.RouteTemplate = "/swagger/api/{documentName}/swagger.json");

            // Enable middleware to serve swagger-ui (HTML, JS, CSS etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/api/v1/swagger.json", "Vayosoft API V1");
                c.InjectStylesheet("/css/swagger.css");
                c.RoutePrefix = "api";
            });

            return app;
        }

        public static IServiceCollection AddApiVersioningService(this IServiceCollection services)
        {

            //https://christian-schou.dk/how-to-use-api-versioning-in-net-core-web-api/
            services.AddApiVersioning(opt =>
            {
                opt.DefaultApiVersion = new ApiVersion(1, 0);
                opt.AssumeDefaultVersionWhenUnspecified = true;
                opt.ReportApiVersions = true;//api-support-versions
                //opt.ApiVersionReader = new UrlSegmentApiVersionReader();
                opt.ApiVersionReader = ApiVersionReader.Combine(
                    new UrlSegmentApiVersionReader(),
                    //new QueryStringApiVersionReader("x-api-version"),
                    //new MediaTypeApiVersionReader("x-api-version"), //accept
                    new HeaderApiVersionReader("x-api-version"));
            }).AddApiExplorer(setup =>
            {
                setup.GroupNameFormat = "'v'VVV";
                setup.SubstituteApiVersionInUrl = true;
            });

            return services;
        }
    }
}
