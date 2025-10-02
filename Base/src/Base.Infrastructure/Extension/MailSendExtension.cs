using Base.Infrastructure.Interface.Mail;
using Base.Mail.Adapter;
using Microsoft.Extensions.DependencyInjection;

namespace Base.Infrastructure.Extension;

public static class MailSendExtension
{
    /// <summary>
    /// 加入郵件服務 <see cref="MailSendAdapter"/>
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="MailSendAdapter"/> to.</param>
    public static void AddMailService(this IServiceCollection services)
    {
        services.AddScoped<IMailSendAdapter, MailSendAdapter>();
    }
}
