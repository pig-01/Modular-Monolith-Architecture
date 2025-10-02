using Base.Domain.SeedWorks;

namespace Base.Domain.Exceptions;

/// <summary>
/// 當設定檔為空時拋出的例外
/// </summary>
[Serializable]
public class ConfigNullException : BaseException
{
    public ConfigNullException() { }
    public ConfigNullException(string message) : base(message) { }
    public ConfigNullException(string message, Exception inner) : base(message, inner) { }
}
