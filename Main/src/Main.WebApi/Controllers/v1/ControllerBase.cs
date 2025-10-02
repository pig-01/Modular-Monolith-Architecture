using Asp.Versioning;
using Base.Domain.Exceptions;
using Base.Domain.Models.Results;
using Main.Domain.Exceptions;
using static Base.Domain.Enums.ServiceEnum;

namespace Main.WebApi.Controllers.v1;

[Authorize]
[ApiController]
[ApiVersion(1.0)]
[Produces("application/json")]
[Route("api/v{version:apiVersion}/[controller]")]
public class BaseController : ControllerBase
{

    protected IActionResult ActionResultBuilder<T>(ServiceResult<T> serviceResult) => serviceResult.State switch
    {
        ServiceState.Success => Ok(serviceResult.Data),
        ServiceState.NotFound => CreateNotFoundResult(serviceResult.Message, serviceResult.Exception),
        ServiceState.Initial => throw new NotImplementedException(),
        ServiceState.Failure => throw new NotImplementedException(),
        _ => BadRequest(new ApiSystemException(serviceResult.Exception ?? throw new("service error handle setting error!")))
    };

    /// <summary>
    /// 利用比對模式建構ActionResult
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="result"></param>
    /// <returns></returns>
    protected IActionResult ActionResultBuilder<T>(T result) => result switch
    {
        null => NotFound(new ApiSystemException(new NotFoundException("查無資料"))),
        ServiceResult<T> serviceResult => ActionResultBuilder(serviceResult),
        ServiceResult serviceResult => serviceResult.State switch
        {
            ServiceState.Success => Ok(result),
            ServiceState.NotFound => CreateNotFoundResult(serviceResult.Message, serviceResult.Exception),
            ServiceState.Initial => throw new NotImplementedException(),
            ServiceState.Failure => throw new NotImplementedException(),
            _ => BadRequest(new ApiSystemException(serviceResult.Exception ?? throw new HandleException("service error handle setting error!")))
        },
        _ when result is IEnumerable<T> enumerable && !enumerable.Any() => NotFound(new ApiSystemException(new NotFoundException("查無資料"))),
        _ => Ok(result)
    };

    private NotFoundObjectResult CreateNotFoundResult(string message, Exception? exception) => exception is null ? NotFound(new ApiSystemException(new NotFoundException(message))) : NotFound(new ApiSystemException(exception));
}
