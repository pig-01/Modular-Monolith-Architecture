using Base.Domain.SeedWorks;

namespace Base.Domain.Exceptions;

/// <summary>
/// 當 Claim 為空時拋出的例外
/// </summary>
[Serializable]
public class ClaimNullException : BaseException
{
    public ClaimNullException() { }
    public ClaimNullException(string message) : base(message) { }
    public ClaimNullException(string message, Exception inner) : base(message, inner) { }
}
