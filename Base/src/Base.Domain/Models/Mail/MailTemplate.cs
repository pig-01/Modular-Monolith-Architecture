namespace Base.Domain.Models.Mail;

public partial class MailTemplate
{
    /// <summary>
    /// 識別欄位
    /// </summary>
    public long MailTemplateId { get; set; }

    /// <summary>
    /// 公司PartyID
    /// </summary>
    public long CompanyPartyId { get; set; }

    /// <summary>
    /// 功能代號
    /// </summary>
    public string FunctionCode { get; set; } = null!;

    /// <summary>
    /// mail類別
    /// </summary>
    public string MailType { get; set; } = null!;

    /// <summary>
    /// 中文主旨
    /// </summary>
    public string? ZhChtsubject { get; set; }

    /// <summary>
    /// 中文內文
    /// </summary>
    public string? ZhChtbody { get; set; }

    /// <summary>
    /// 英文主旨
    /// </summary>
    public string? EnUssubject { get; set; }

    /// <summary>
    /// 英文內文
    /// </summary>
    public string? EnUsbody { get; set; }

    /// <summary>
    /// 簡體主旨
    /// </summary>
    public string? ZhChssubject { get; set; }

    /// <summary>
    /// 簡體內文
    /// </summary>
    public string? ZhChsbody { get; set; }

    /// <summary>
    /// 日文主旨
    /// </summary>
    public string? JaJpsubject { get; set; }

    /// <summary>
    /// 日文內文
    /// </summary>
    public string? JaJpbody { get; set; }

    /// <summary>
    /// Tenant代號
    /// </summary>
    public string TenantId { get; set; } = null!;

    /// <summary>
    /// 建檔日期
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// 建檔人員
    /// </summary>
    public string CreatedUser { get; set; } = null!;

    /// <summary>
    /// 最後修改日期
    /// </summary>
    public DateTime ModifiedDate { get; set; }

    /// <summary>
    /// 最後修改人員
    /// </summary>
    public string ModifiedUser { get; set; } = null!;

    public string? MailTemplateName { get; set; }
}
