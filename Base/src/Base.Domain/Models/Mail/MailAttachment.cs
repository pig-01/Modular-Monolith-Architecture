namespace Base.Domain.Models.Mail;

public class MailAttachment
{
    public required Stream FileStream { get; set; }
    public string? FileName { get; set; }
    public string? Extension { get; set; }
    public bool IsInline { get; set; }
}
