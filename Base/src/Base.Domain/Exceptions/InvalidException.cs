using System.Globalization;
using Base.Domain.SeedWorks;

namespace Base.Domain.Exceptions;

/// <summary>
/// 當驗證失敗時拋出的例外
/// </summary>
[Serializable]
public class InvalidException : BaseException
{
    public InvalidException() : base() { }
    public InvalidException(string message) : base(message) { }
    public InvalidException(string message, params string[] args) : base(string.Format(CultureInfo.CurrentCulture, message, args)) { }
    public InvalidException(string message, Exception innerException) : base(message, innerException) { }
}
