using System.Globalization;
using Scheduler.Domain.Enums;

namespace Scheduler.Domain.AggregateModel.MailAggregate;

/// <summary>
/// 發信佇列
/// </summary>
[Table("MailQueue")]
public partial class MailQueue : Entity, IAggregateRoot
{
    /// <summary>
    /// 佇列識別碼
    /// </summary>
    [Key]
    [Column("ID")]
    public long Id { get; set; }

    /// <summary>
    /// 收件者
    /// </summary>
    [StringLength(1023)]
    [Unicode(false)]
    public string Recipient { get; set; } = null!;

    /// <summary>
    /// 副本
    /// </summary>
    [Column("CC")]
    [StringLength(1023)]
    [Unicode(false)]
    public string? Cc { get; set; }

    /// <summary>
    /// 密件副本
    /// </summary>
    [Column("BCC")]
    [StringLength(1023)]
    [Unicode(false)]
    public string? Bcc { get; set; }

    /// <summary>
    /// 主旨
    /// </summary>
    [StringLength(1023)]
    public string Subject { get; set; } = null!;

    /// <summary>
    /// 本文
    /// </summary>
    public string? Body { get; set; }

    /// <summary>
    /// 本文是否為HTML
    /// </summary>
    public bool IsBodyHtml { get; set; }

    /// <summary>
    /// 本文編碼
    /// </summary>
    [StringLength(255)]
    [Unicode(false)]
    public string? Encoding { get; set; } = "UTF-8";

    /// <summary>
    /// 發信狀態
    /// </summary>
    [StringLength(1)]
    [Unicode(false)]
    public string Status { get; set; } = MailQueueStatus.Pending.Id.ToString(CultureInfo.InvariantCulture);

    /// <summary>
    /// 站台識別碼
    /// </summary>
    [Column("TenantID")]
    [StringLength(20)]
    [Unicode(false)]
    public string TenantId { get; set; } = TenantEnum.SuperTenant.Name;

    public MailQueue() { }

    /// <summary>
    /// 建立發信佇列
    /// </summary>
    /// <param name="recipients">收件者清單</param>
    /// <param name="subject">信件主旨</param>
    /// <param name="body">信件本文</param>
    /// <param name="createdUser">建立者</param>
    /// <param name="isHtml">是否為HTML格式</param>
    public MailQueue(List<string> recipients, string subject, string body, string createdUser, bool isHtml = false) : this()
    {
        Recipient = string.Join(';', recipients.ToArray());
        Subject = subject;
        Body = body;
        IsBodyHtml = isHtml;
        SetCreateMetadata(createdUser, createdUser);
    }

    /// <summary>
    /// 建立發信佇列
    /// </summary>
    /// <param name="recipients">收件者清單</param>
    /// <param name="subject">信件主旨</param>
    /// <param name="body">信件本文</param>
    /// <param name="tenantId">租戶識別碼</param>
    /// <param name="createdUser">建立者</param>
    /// <param name="isHtml">是否為HTML格式</param>
    /// <returns></returns>
    public MailQueue(List<string> recipients, string subject, string body, string tenantId, string createdUser, bool isHtml = false) : this(recipients, subject, body, createdUser, isHtml) => TenantId = tenantId;

    /// <summary>
    /// 建立發信佇列
    /// </summary>
    /// <param name="recipients">收件者清單</param>
    /// <param name="cc">副本清單</param>
    /// <param name="subject">信件主旨</param>
    /// <param name="body">信件本文</param>
    /// <param name="tenantId">租戶識別碼</param>
    /// <param name="createdUser">建立者</param>
    /// <param name="isHtml">是否為HTML格式</param>
    /// <returns></returns>
    public MailQueue(List<string> recipients, List<string> cc, string subject, string body, string tenantId, string createdUser, bool isHtml = false) : this(recipients, subject, body, tenantId, createdUser, isHtml) => Cc = string.Join(';', cc.ToArray());

    /// <summary>
    /// 建立發信佇列
    /// </summary>
    /// <param name="recipients">收件者清單</param>
    /// <param name="cc">副本清單</param>
    /// <param name="subject">信件主旨</param>
    /// <param name="body">信件本文</param>
    /// <param name="tenantId">租戶識別碼</param>
    /// <param name="createdUser">建立者</param>
    /// <param name="isHtml">是否為HTML格式</param>
    /// <returns></returns>
    public MailQueue(List<string> recipients, List<string> cc, List<string> bcc, string subject, string body, string tenantId, string createdUser, bool isHtml = false) : this(recipients, cc, subject, body, tenantId, createdUser, isHtml) => Bcc = string.Join(';', bcc.ToArray());

}