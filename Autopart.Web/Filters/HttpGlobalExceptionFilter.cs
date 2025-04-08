using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using Autopart.Domain.Exceptions;

namespace Autopart.API.Filters
{
    public class HttpGlobalExceptionFilter : IExceptionFilter
    {
        private readonly IHostEnvironment _env;
        private readonly ILogger<HttpGlobalExceptionFilter> _logger;

        public HttpGlobalExceptionFilter(IHostEnvironment env, ILogger<HttpGlobalExceptionFilter> logger)
        {
            _env = env;
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {

            if (context.Exception is DomainException)
            {
                context.Result = new BadRequestObjectResult(context.Exception.Message);
            }
            else
            {
                _logger.LogError(new EventId(context.Exception.HResult),
                    context.Exception,
                    context.Exception.Message
                );

                if (_env.IsDevelopment())
                {
                    context.Result = new JsonResult(new
                    {
                        context.Exception.Message,
                        DeveloperMessage = context.Exception
                    });
                }
                context.Result = new JsonResult(new
                {
                    context.Exception.Message
                });
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            context.ExceptionHandled = true;
        }
    }
}
