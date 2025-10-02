using Base.Domain.SeedWorks;

namespace Base.Domain.Exceptions;

/// <summary>
/// 當警告時拋出的例外，通常只記錄錯誤，不會中斷程式執行
/// </summary>
[Serializable]
public class WarningException : BaseException
{
    public WarningException() : base() { }
    public WarningException(string message) : base(message) { }
    public WarningException(string message, params string[] args) : base(string.Format(message, args)) { }
    public WarningException(string message, Exception innerException) : base(message, innerException) { }
}
