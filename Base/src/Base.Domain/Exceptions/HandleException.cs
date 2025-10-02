using Base.Domain.SeedWorks;

namespace Base.Domain.Exceptions;

/// <summary>
/// 當處理例外時拋出的例外，常用於IMediator的Handle中
/// </summary>
[Serializable]
public class HandleException : BaseException
{
    public HandleException() : base() { }
    public HandleException(string message) : base(message) { }
    public HandleException(string message, params string[] args) : base(string.Format(message, args)) { }
    public HandleException(string message, Exception innerException) : base(message, innerException) { }
}
