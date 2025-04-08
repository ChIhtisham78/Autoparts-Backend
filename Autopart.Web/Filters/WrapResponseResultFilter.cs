using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;
using Autopart.API.Infrastructure;
using Autopart.Application.Infrastructure.Models.Responses;

namespace Autopart.API.Filters
{
    public class WrapResponseResultFilter : IResultFilter
    {
        public void OnResultExecuting(ResultExecutingContext context)
        {
            if (context?.Result is BadRequestObjectResult badObjResult)
            {
                if (!(badObjResult.Value is ErrorResponse))
                {
                    if (badObjResult.Value is ValidationProblemDetails details)
                    {
                        ErrorResponse errorResponse = new(details.Status, details.Title, details.Type, Activity.Current?.Id ?? context.HttpContext?.TraceIdentifier);
                        if (details.Errors.Count > 0)
                        {
                            errorResponse = new ErrorListResponse(errorResponse, details.Errors);
                        }

                        context.Result = new BadRequestObjectResult(details)
                        {
                            Value = errorResponse,
                            StatusCode = badObjResult.StatusCode
                        };
                    }
                    else if (badObjResult.Value is string msg)
                    {
                        context.Result = new BadRequestObjectResult(new ApiError
                        {
                            Success = false,
                            Message = msg,
                            StatusCode = badObjResult.StatusCode.Value
                        })
                        {
                            StatusCode = badObjResult.StatusCode
                        };
                    }
                }
            }
            else if (context?.Result is CreatedAtActionResult createdAtResult)
            {
                if (!(createdAtResult.Value is BaseResponse))
                {
                    bool success = createdAtResult.StatusCode >= 200 && createdAtResult.StatusCode <= 299;
                    context.Result = new CreatedAtActionResult(
                        createdAtResult.ActionName,
                        createdAtResult.ControllerName,
                        createdAtResult.RouteValues,
                        new ResultResponse(createdAtResult.Value, success: success))
                    {
                        StatusCode = createdAtResult.StatusCode,
                        UrlHelper = createdAtResult.UrlHelper
                    };
                }
            }
            else if (context?.Result is ObjectResult objResult)
            {
                if (!(objResult.Value is BaseResponse))
                {
                    bool success = objResult.StatusCode >= 200 && objResult.StatusCode <= 299;
                    context.Result = objResult.Value switch
                    {
                        string message => new ObjectResult(new MessageResponse(message, success: success))
                        {
                            StatusCode = objResult.StatusCode
                        },
                        _ => new ObjectResult(new ResultResponse(objResult.Value, success: success))
                        {
                            StatusCode = objResult.StatusCode
                        }
                    };
                }
            }
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {
        }
    }
}
