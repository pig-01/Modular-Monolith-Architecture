using Base.Domain.Models.Mail;

namespace Base.Infrastructure.Interface.Mail;

public interface IMailRepository
{
    Task<IEnumerable<MailServiceParameter>> GetMailServiceParameters(string tenantId);
    Task<IEnumerable<MailServiceParameter>> GetMailServiceParameterByID(string mailServiceParameterID);
    Task<MailTemplate?> GetMailTemplate(string functionCode, string mailType, string tenantId);
}
