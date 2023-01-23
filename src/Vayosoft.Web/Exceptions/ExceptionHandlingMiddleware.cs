using System.Net;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using Vayosoft.Web.Model;

namespace Vayosoft.Web.Exceptions
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ProblemDetailsFactory _problemDetailsFactory;
        private readonly ILogger logger;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ProblemDetailsFactory problemDetailsFactory,
            ILoggerFactory loggerFactory)
        {
            this.next = next;
            _problemDetailsFactory = problemDetailsFactory;
            logger = loggerFactory.CreateLogger<ExceptionHandlingMiddleware>();
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            logger.LogError(exception, "An unhandled exception has occurred, {Message}", exception.Message);
            //context.Response.Redirect("/error");

            if (exception is ValidationException validationException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.WriteAsJsonAsync(validationException.Errors.ToProblemDetails(context.Request.Path), context.RequestAborted);
            }
            else
            {
                var codeInfo = exception.GetHttpStatusCodeInfo();
                var problemDetails = _problemDetailsFactory.CreateProblemDetails(context,
                    title: "An error occurred while processing your request.", statusCode: (int)codeInfo.Code);

                context.Response.StatusCode = (int)codeInfo.Code;
                context.Response.WriteAsJsonAsync(problemDetails, context.RequestAborted);
            }

            return Task.CompletedTask;
        }

        public static bool IsAjaxRequest(HttpContext context)
        {
            return !string.IsNullOrEmpty(context.Request.Headers["x-requested-with"]) && 
                   context.Request.Headers["x-requested-with"][0]!.Equals("xmlhttprequest", StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsAcceptMimeType(HttpContext context, string mimeType)
        {
            var acceptHeader = context.Request.GetTypedHeaders().Accept;
            var result = acceptHeader.Any(t => 
                (t.Suffix != null && t.Suffix.Equals(mimeType)) || 
                (t.SubTypeWithoutSuffix != null && t.SubTypeWithoutSuffix.Equals(mimeType)));
            return result;
        }
    }
}
