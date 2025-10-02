namespace Scheduler.Domain.AggregateModel.MailAggregate;

[Table("MailTemplate")]
public partial class MailTemplate : Entity
{
    /// <summary>
    /// 識別欄位
    /// </summary>
    [Key]
    [Column("MailTemplateID")]
    public long MailTemplateId { get; set; }

    /// <summary>
    /// 功能代號
    /// </summary>
    [StringLength(50)]
    [Unicode(false)]
    public string FunctionCode { get; set; } = null!;

    /// <summary>
    /// mail類別
    /// </summary>
    [StringLength(50)]
    [Unicode(false)]
    public string MailType { get; set; } = null!;

    /// <summary>
    /// 中文主旨
    /// </summary>
    [Column("zhCHTSubject")]
    [StringLength(255)]
    public string? ZhChtsubject { get; set; }

    /// <summary>
    /// 中文內文
    /// </summary>
    [Column("zhCHTBody")]
    public string? ZhChtbody { get; set; }

    /// <summary>
    /// 英文主旨
    /// </summary>
    [Column("enUSSubject")]
    [StringLength(255)]
    public string? EnUssubject { get; set; }

    /// <summary>
    /// 英文內文
    /// </summary>
    [Column("enUSBody")]
    public string? EnUsbody { get; set; }

    /// <summary>
    /// 簡體主旨
    /// </summary>
    [Column("zhCHSSubject")]
    [StringLength(255)]
    public string? ZhChssubject { get; set; }

    /// <summary>
    /// 簡體內文
    /// </summary>
    [Column("zhCHSBody")]
    public string? ZhChsbody { get; set; }

    /// <summary>
    /// 日文主旨
    /// </summary>
    [Column("jaJPSubject")]
    [StringLength(255)]
    public string? JaJpsubject { get; set; }

    /// <summary>
    /// 日文內文
    /// </summary>
    [Column("jaJPBody")]
    public string? JaJpbody { get; set; }

    /// <summary>
    /// Tenant代號
    /// </summary>
    [Column("TenantID")]
    [StringLength(20)]
    [Unicode(false)]
    public string TenantId { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string? MailTemplateName { get; set; }
}