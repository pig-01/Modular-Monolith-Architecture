using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;


namespace DataHub.Domain.AggregatesModel.MailAggregate;

[Table("MailServiceParameter")]
public partial class MailServiceParameter
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
    /// 最後修改日期
    /// </summary>
    [Column(TypeName = "datetime")]
    public DateTime? ModifiedDate { get; set; }

    /// <summary>
    /// 最後修改人員
    /// </summary>
    [StringLength(50)]
    public string? ModifiedUser { get; set; }

    [Column("EnableSSL")]
    public bool EnableSsl { get; set; }

    [InverseProperty("MailServiceParameter")]
    public virtual ICollection<MailServiceRelation> MailServiceRelations { get; set; } = [];
}
