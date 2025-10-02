namespace Base.Domain.Exceptions;

/// <summary>
/// 當領域處理出現例外時拋出的例外
/// </summary>
[Serializable]
public class DomainException : Exception
{
    public DomainException() { }
    public DomainException(string message) : base(message) { }
    public DomainException(string message, Exception innerException) : base(message, innerException) { }
}
