namespace Scheduler.Domain.AggregateModel.MailAggregate;

[Table("MailServiceRelation")]
public partial class MailServiceRelation : Entity
{
    /// <summary>
    /// 識別欄位
    /// </summary>
    [Key]
    [Column("MailServiceRelationID")]
    public long MailServiceRelationId { get; set; }

    /// <summary>
    /// Mail服務參數識別欄位
    /// </summary>
    [Column("MailServiceParameterID")]
    public long MailServiceParameterId { get; set; }

    /// <summary>
    /// Tenant代號
    /// </summary>
    [Column("TenantID")]
    [StringLength(20)]
    [Unicode(false)]
    public string TenantId { get; set; } = null!;

    [ForeignKey("MailServiceParameterId")]
    [InverseProperty("MailServiceRelations")]
    public virtual MailServiceParameter MailServiceParameter { get; set; } = null!;
}