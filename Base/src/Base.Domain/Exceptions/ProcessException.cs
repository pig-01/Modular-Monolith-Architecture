using Base.Domain.SeedWorks;

namespace Base.Domain.Exceptions;

/// <summary>
/// 處理呼叫程序時發生例外
/// </summary>
[Serializable]
public class ProcessException : BaseException
{
    public ProcessException() : base() { }
    public ProcessException(string message) : base(message) { }
    public ProcessException(string message, params string[] args) : base(string.Format(message, args)) { }
    public ProcessException(string message, Exception innerException) : base(message, innerException) { }
}
