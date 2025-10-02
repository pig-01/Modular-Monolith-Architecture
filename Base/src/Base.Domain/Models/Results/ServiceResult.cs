using static Base.Domain.Enums.ServiceEnum;

namespace Base.Domain.Models.Results;

public class ServiceResult : IDisposable
{
    public ServiceState State { get; set; } = ServiceState.Initial;
    public string Message { get; set; } = string.Empty;
    public Exception? Exception { get; set; } = default!;

    public ServiceResult() { }

    public ServiceResult(ServiceState state, string message)
    {
        State = state;
        Message = message;
    }

    public void Dispose() => GC.SuppressFinalize(this);
}

public class ServiceResult<T> : ServiceResult, IDisposable
{
    public T Data { get; set; } = default!;

    public ServiceResult() { }

    public ServiceResult(T data) => Data = data;

    public ServiceResult(ServiceState state, string message, T data)
    {
        State = state;
        Message = message;
        Data = data;
    }
}
