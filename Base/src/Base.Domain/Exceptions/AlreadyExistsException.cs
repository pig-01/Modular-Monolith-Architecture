using Base.Domain.SeedWorks;

namespace Base.Domain.Exceptions;

/// <summary>
/// 已存在資料的例外狀況
/// </summary>
[Serializable]
public class AlreadyExistsException : BaseException
{
    public AlreadyExistsException() { }
    public AlreadyExistsException(string message) : base(message) { }
    public AlreadyExistsException(string message, Exception inner) : base(message, inner) { }
}
