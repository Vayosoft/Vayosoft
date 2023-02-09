using System.Net;
using FluentValidation;
using LanguageExt.Common;
using Microsoft.AspNetCore.Mvc;
using Vayosoft.Commons.Models.Pagination;
using Vayosoft.Web.Exceptions;
using Vayosoft.Web.Model;

namespace Vayosoft.Web.Controllers
{
    [ApiController]
    public abstract class ApiControllerBase : ControllerBase
    {
        protected IActionResult Result<TResult>(Result<TResult> result) {
            return result.Match(obj => Ok(obj), Problem);
        }

        protected IActionResult Result<TResult, TContract>(Result<TResult> result, Func<TResult, TContract> mapper) {
            return result.Match(obj => Ok(mapper(obj)), Problem);
        }

        protected IActionResult Page<TResult>(IPagedEnumerable<TResult> collection, long pageSize)
        {
            return Ok(new PagedResponse<TResult>(collection, pageSize));
        }

        protected IActionResult List<TResult>(IEnumerable<TResult> collection)
        {
            return Ok(new ListResponse<TResult>(collection ?? Array.Empty<TResult>()));
        }

        protected IActionResult Problem(Exception exception)
        {
            switch (exception)
            {
                case ValidationException validationException:
                {
                    return BadRequest(validationException.Errors.ToProblemDetails(Request.Path));
                }
                default:
                {
                    var codeInfo = exception?.GetHttpStatusCodeInfo();
                    return Problem(title: "An error occurred while processing your request.",
                        statusCode: (int)(codeInfo?.Code ?? HttpStatusCode.InternalServerError));
                }
            }
        }
    }
}
