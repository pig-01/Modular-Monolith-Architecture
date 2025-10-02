using Base.Domain.SeedWorks;

namespace Base.Domain.Exceptions;

/// <summary>
/// 當寄送郵件失敗時拋出的例外
/// </summary>
[Serializable]
public class MailSendException : BaseException
{
    public MailSendException() { }
    public MailSendException(string message) : base(message) { }
    public MailSendException(string message, Exception inner) : base(message, inner) { }
}
