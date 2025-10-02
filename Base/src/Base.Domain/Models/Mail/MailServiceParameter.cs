namespace Base.Domain.Models.Mail;

public class MailServiceParameter
{
    public string? MailServiceParameterID { get; set; }
    public string? ServiceType { get; set; }
    public string? Domain { get; set; }
    public string? Account { get; set; }
    public string? Password { get; set; }
    public string? TenantID { get; set; }
    public bool EnableSSL { get; set; }

    public string GetMailSender() => $"{Account}@{Domain}";
}
