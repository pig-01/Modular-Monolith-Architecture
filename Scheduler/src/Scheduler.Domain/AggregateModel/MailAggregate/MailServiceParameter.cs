namespace Scheduler.Domain.AggregateModel.MailAggregate;

[Table("MailServiceParameter")]
public partial class MailServiceParameter : Entity
{
    /// <summary>
    /// 識別欄位
    /// </summary>
    [Key]
    [Column("MailServiceParameterID")]
    public long MailServiceParameterId { get; set; }

    /// <summary>
    /// 服務類型(SystemCode CodeType=&apos;MailServiceType&apos;)
    /// </summary>
    [StringLength(50)]
    [Unicode(false)]
    public string ServiceType { get; set; } = null!;

    /// <summary>
    /// 網域
    /// </summary>
    [StringLength(100)]
    [Unicode(false)]
    public string? Domain { get; set; }

    /// <summary>
    /// 帳號
    /// </summary>
    [StringLength(50)]
    [Unicode(false)]
    public string? Account { get; set; }

    /// <summary>
    /// 密碼
    /// </summary>
    [StringLength(100)]
    [Unicode(false)]
    public string? Password { get; set; }

    /// <summary>
    /// Tenant代號
    /// </summary>
    [Column("TenantID")]
    [StringLength(20)]
    [Unicode(false)]
    public string TenantId { get; set; } = null!;

    [Column("EnableSSL")]
    public bool EnableSsl { get; set; }

    [InverseProperty("MailServiceParameter")]
    public virtual ICollection<MailServiceRelation> MailServiceRelations { get; set; } = [];
}