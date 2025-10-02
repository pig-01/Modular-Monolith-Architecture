namespace Main.Domain.Exceptions;

public class ApiSystemException(Exception exception)
{
    public string? Message { get; set; } = exception.Message;
    public string? StackFlow { get; set; } = exception?.StackTrace;
    public ApiSystemException? InnerException { get; set; } = exception?.InnerException is null ? null : new ApiSystemException(exception.InnerException);
}
