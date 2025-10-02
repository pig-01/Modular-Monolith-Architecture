using Base.Domain.SeedWorks;

namespace Base.Domain.Exceptions;

/// <summary>
/// API 錯誤訊息
/// </summary>
/// <param name="message">Message</param>
/// <param name="stackTrace">StackTrace</param>
/// <param name="exception">Exception</param>
public class ApiException(string message, string? stackTrace, Exception? exception = null) : BaseException
{
    public new string Message { get; set; } = message;

    public new string? StackTrace { get; set; } = stackTrace;

    public new ApiException? InnerException { get; set; } = GetApiException(exception);

    /// <summary>
    /// 取得 API Inner錯誤訊息
    /// </summary>
    /// <param name="exception"></param>
    /// <returns></returns>
    private static ApiException? GetApiException(Exception? exception)
    {
        if (exception is null || exception.InnerException is null) return null;

        return new ApiException(exception.InnerException.Message, exception.StackTrace, exception.InnerException);
    }
}
