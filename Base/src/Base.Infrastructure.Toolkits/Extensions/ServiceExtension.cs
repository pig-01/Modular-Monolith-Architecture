using Base.Domain.Models.Results;
using static Base.Domain.Enums.ServiceEnum;

namespace Base.Infrastructure.Toolkits.Extensions;


public static class ServiceResultExtensions
{
    public static ServiceResult<T> SetSuccess<T>(this ServiceResult<T> result, T data, string? message = null)
    {
        result.State = ServiceState.Success;
        result.Message = message ?? $"{nameof(T)} data process success";
        result.Data = data;
        return result;
    }
    public static ServiceResult<T> SetSuccess<T>(this ServiceResult<T> result, string? message, T data)
    {
        result.State = ServiceState.Success;
        result.Message = message ?? $"{nameof(T)} data process success";
        result.Data = data;
        return result;
    }

    public static ServiceResult<T> SetNotFound<T>(this ServiceResult<T> result, string? message = null)
    {
        result.State = ServiceState.NotFound;
        result.Message = message ?? $"{nameof(T)} data notfound";
        return result;
    }

    public static ServiceResult<T> SetFailure<T>(this ServiceResult<T> result, Exception exception)
    {
        result.State = ServiceState.Failure;
        result.Exception = exception;
        result.Message = exception.Message;
        return result;
    }

    public static ServiceResult<T> SetFailure<T>(this ServiceResult<T> result, string message, Exception exception)
    {
        result.State = ServiceState.Failure;
        result.Exception = exception;
        result.Message = message;
        return result;
    }

    public static ServiceResult SetSuccess(this ServiceResult result, string? message = null)
    {
        result.State = ServiceState.Success;
        result.Message = message ?? $"data process success";
        return result;
    }

    public static ServiceResult SetNotFound(this ServiceResult result, string? message = null)
    {
        result.State = ServiceState.NotFound;
        result.Message = message ?? $"data notfound";
        return result;
    }

    public static ServiceResult SetFailure(this ServiceResult result, Exception exception)
    {
        result.State = ServiceState.Failure;
        result.Exception = exception;
        result.Message = exception.Message;
        return result;
    }

    public static ServiceResult SetFailure(this ServiceResult result, string message, Exception exception)
    {
        result.State = ServiceState.Failure;
        result.Exception = exception;
        result.Message = message;
        return result;
    }
}


