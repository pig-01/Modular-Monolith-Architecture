using System.Net.Mail;
using Base.Domain.Exceptions;
using Base.Domain.Models.Mail;
using Base.Infrastructure.Interface.Mail;
using Base.Infrastructure.Toolkits.Extensions;
using Base.Infrastructure.Toolkits.Resources;
using Scheduler.Application.Queries.Mails;
using Scheduler.Domain.Enums;
using MailAggregate = Scheduler.Domain.AggregateModel.MailAggregate;

namespace Scheduler.Infrastructure;

public class MailService(
    ILogger<MailService> logger,
    IMailSendAdapter mailSendAdapter,
    IMailQuery mailQuery) : IMailService
{
    /// <summary>
    /// 取得郵件範本資訊
    /// </summary>
    /// <param name="functionCode"></param>
    /// <param name="mailType"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<MailInfomation> GetMailTemplate(string functionCode, string mailType, CancellationToken cancellationToken = default)
    {
        (string tenantId, string culture) = await GetSchedulerInfoAsync(cancellationToken);
        return await GetMailTemplate(functionCode, mailType, tenantId, culture);
    }

    /// <summary>
    /// 發送郵件
    /// </summary>
    /// <param name="mailInfomation"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task SendAsync(MailInfomation mailInfomation, CancellationToken cancellationToken = default)
    {
        MailServiceParameter mailServiceParameter = await GetMailService(cancellationToken);
        await mailSendAdapter.SendMail(mailInfomation, mailServiceParameter, cancellationToken);
    }

    /// <summary>
    /// 發送郵件
    /// </summary>
    /// <param name="mailMessage"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task SendAsync(MailMessage mailMessage, CancellationToken cancellationToken = default)
    {
        MailServiceParameter mailServiceParameter = await GetMailService(cancellationToken);
        await mailSendAdapter.SendMail(mailMessage, mailServiceParameter, cancellationToken);
    }

    /// <summary>
    /// 取得目前使用者的郵件伺服器參數
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task<MailServiceParameter> GetMailService(CancellationToken cancellationToken = default)
    {
        (string tenantId, string culture) = await GetSchedulerInfoAsync(cancellationToken);
        return await GetMailServiceParameter(tenantId);
    }

    /// <summary>
    /// 取得排程器的租戶ID、公司Party ID和文化資訊
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    private async Task<(string, string)> GetSchedulerInfoAsync(CancellationToken cancellationToken = default)
    {
        // 使用排程器專用的預設值
        string tenantId = TenantEnum.SuperTenant.Name;
        string culture = "zh-CHT";
        logger.LogInformation("Scheduler Info - TenantId: {TenantId}, Culture: {Culture}, Guid: {$Guid}", tenantId, culture, cancellationToken);
        await Task.CompletedTask;
        return (tenantId, culture);
    }

    /// <summary>
    /// 取得郵件模板，根據系統功能編號、信件類型、租戶編號和產品編號
    /// </summary>
    /// <param name="functionCode">系統功能編號</param>
    /// <param name="mailType">信件類型</param>
    /// <param name="tenantId">租戶編號</param>
    /// <param name="culture">語系</param>
    /// <remarks>根據不同的語言環境，預設為繁體中文</remarks>
    /// <returns>MailInfomation 發信資訊</returns>
    /// <exception cref="MailSendException">當郵件模板不存在時拋出異常</exception>
    private async Task<MailInfomation> GetMailTemplate(string functionCode, string mailType, string tenantId, string culture)
    {
        // 取得郵件發信機參數
        MailTemplate mailTemplate = await mailQuery.GetMailTemplate(functionCode, mailType, tenantId)
            ?? throw new MailSendException(MessageResource.MailSendExceptionMessage.SetCustomerMessage("信件套表異常"));

        // 取得郵件預設發信人
        MailAggregate.MailSender mailSender = await mailQuery.GetMailSender(tenantId);

        // 處理郵件主題和內容，根據不同的語言環境，預設為繁體中文
        return new MailInfomation
        {
            // 郵件主題如有未設定值，則返回空字串
            Subject = GetSubject(mailTemplate, culture) ?? "",
            Body = GetBody(mailTemplate, culture),
            Sender = new MailAddress(mailSender.MailAddress, mailSender.SenderName),
        };
    }

    /// <summary>
    /// 取得郵件發信機參數，根據租戶編號、產品編號和語系
    /// </summary>
    /// <param name="tenantId">租戶編號</param>
    /// <returns>郵件發信機參數</returns>
    /// <exception cref="MailSendException">當郵件發信機參數不存在時拋出異常</exception>
    private async Task<MailServiceParameter> GetMailServiceParameter(string tenantId)
    {
        IEnumerable<MailServiceParameter> mailServiceParameter = await mailQuery.GetMailServiceParameters(tenantId);

        return IsListnullOrCountZero(mailServiceParameter)
            ? throw new MailSendException(MessageResource.MailSendExceptionMessage.SetCustomerMessage("發信機參數異常"))
            : mailServiceParameter.FirstOrDefault() ??
                throw new MailSendException(MessageResource.MailSendExceptionMessage.SetCustomerMessage("發信機參數異常"));
    }

    private static string? GetBody(MailTemplate mailTemplate, string culture) => culture switch
    {
        "zh-CHT" => mailTemplate.ZhChtbody,
        "en-US" => mailTemplate.EnUsbody,
        "zh-CHS" => mailTemplate.ZhChsbody,
        "ja-JP" => mailTemplate.JaJpbody,
        _ => mailTemplate.ZhChtbody,
    };

    private static string? GetSubject(MailTemplate mailTemplate, string culture) => culture switch
    {
        "zh-CHT" => mailTemplate.ZhChtsubject,
        "en-US" => mailTemplate.EnUssubject,
        "zh-CHS" => mailTemplate.ZhChssubject,
        "ja-JP" => mailTemplate.JaJpsubject,
        _ => mailTemplate.ZhChtsubject,
    };

    private static bool IsListnullOrCountZero<T>(IEnumerable<T> list) => list == null || !list.Any();
}
