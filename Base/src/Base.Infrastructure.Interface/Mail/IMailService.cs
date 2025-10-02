using System.Net.Mail;
using Base.Domain.Models.Mail;

namespace Base.Infrastructure.Interface.Mail;

/// <summary>
/// 提供各模組實作的郵件發送服務介面
/// </summary>
public interface IMailService
{
    /// <summary>
    /// 取得郵件範本資訊
    /// </summary>
    /// <param name="functionCode"></param>
    /// <param name="mailType"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<MailInfomation> GetMailTemplate(string functionCode, string mailType, CancellationToken cancellationToken = default);

    /// <summary>
    /// 發送郵件
    /// </summary>
    /// <param name="mailInfomation">郵件信息</param>
    /// <param name="cancellationToken">取消權杖</param>
    Task SendAsync(MailInfomation mailInfomation, CancellationToken cancellationToken = default);

    /// <summary>
    /// 發送郵件
    /// </summary>
    /// <param name="mailMessage">郵件消息</param>
    /// <param name="cancellationToken">取消權杖</param>
    Task SendAsync(MailMessage mailMessage, CancellationToken cancellationToken = default);
}
