using Base.Domain.Exceptions;
using Main.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Main.WebApi.Attributes;

public class CustomExceptionFilterAttribute : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        // 如果已經處理過異常，則不再處理
        if (context.ExceptionHandled)
        {
            return;
        }

        // 根據異常類型處理不同的錯誤
        switch (context.Exception)
        {
            // 處理自定義的 ApiException
            case ApiException apiException:
                context.Result = new JsonResult(new ApiSystemException(apiException))
                {
                    StatusCode = Status400BadRequest
                };
                context.ExceptionHandled = true;
                break;

            // 處理自定義的 UnauthorizedAccessException
            case UnauthorizedAccessException unauthorizedAccessException:
                context.Result = new JsonResult(new ApiSystemException(unauthorizedAccessException))
                {
                    StatusCode = Status403Forbidden
                };
                context.ExceptionHandled = true;
                break;

            // 處理自定義的 NotFoundException
            case NotFoundException notFoundException:
                context.Result = new JsonResult(new ApiSystemException(notFoundException))
                {
                    StatusCode = Status404NotFound
                };
                context.ExceptionHandled = true;
                break;

            // 處理自定義的 HandleException
            case HandleException handleException:
                context.Result = new JsonResult(new ApiSystemException(handleException))
                {
                    StatusCode = Status400BadRequest
                };
                context.ExceptionHandled = true;
                break;

            // 默認情況下繼續處理為系統異常
            default:
                Exception exception = context.Exception;

                ApiSystemException apiSystemException = new(exception);

                context.Result = new JsonResult(apiSystemException)
                {
                    StatusCode = 500
                };

                context.ExceptionHandled = true;
                break;
        }
    }
}
