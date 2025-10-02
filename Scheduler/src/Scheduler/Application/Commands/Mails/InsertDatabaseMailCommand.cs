using System;
using MediatR;

namespace Scheduler.Application.Commands.Mails;

/// <summary>
/// 插入資料庫郵件命令
/// </summary>
public class InsertDatabaseMailCommand(List<string> recipients, string subject, string body, bool isBodyHtml) : IRequest<bool>
{
    /// <summary>
    /// 收件者
    /// </summary>
    /// <value></value>
    public List<string> Recipients { get; set; } = recipients;

    /// <summary>
    /// 主旨
    /// </summary>
    /// <value></value>
    public string Subject { get; set; } = subject;

    /// <summary>
    /// 本文
    /// </summary>
    /// <value></value>
    public string Body { get; set; } = body;

    /// <summary>
    /// 本文是否為HTML
    /// </summary>
    /// <value></value>
    public bool IsBodyHtml { get; set; } = isBodyHtml;
}
