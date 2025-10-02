using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;


namespace DataHub.Domain.AggregatesModel.MailAggregate;

[Table("MailServiceRelation")]
public partial class MailServiceRelation
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

    /// <summary>
    /// 建檔日期
    /// </summary>
    [Column(TypeName = "datetime")]
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// 建檔人員
    /// </summary>
    [StringLength(50)]
    public string CreatedUser { get; set; } = null!;

    /// <summary>
    /// 最後異動日期
    /// </summary>
    [Column(TypeName = "datetime")]
    public DateTime? ModifiedDate { get; set; }

    /// <summary>
    /// 最後異動人員
    /// </summary>
    [StringLength(50)]
    public string? ModifiedUser { get; set; }

    [ForeignKey("MailServiceParameterId")]
    [InverseProperty("MailServiceRelations")]
    public virtual MailServiceParameter MailServiceParameter { get; set; } = null!;
}
