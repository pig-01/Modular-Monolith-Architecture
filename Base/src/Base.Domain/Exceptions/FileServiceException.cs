using Base.Domain.SeedWorks;

namespace Base.Domain.Exceptions;

/// <summary>
/// 當檔案服務例外時拋出的例外
/// </summary>
[Serializable]
public class FileServiceException : BaseException
{
    public FileServiceException() { }
    public FileServiceException(string message) : base(message) { }
    public FileServiceException(string message, Exception inner) : base(message, inner) { }
}
