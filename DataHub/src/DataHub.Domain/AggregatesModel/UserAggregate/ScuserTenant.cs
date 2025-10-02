using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataHub.Domain.AggregatesModel.TenantAggregate;
using DataHub.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;


namespace DataHub.Domain.AggregatesModel.UserAggregate;

[Table("SCUserTenant")]
public partial class ScuserTenant(string tenantId, string userId) : Entity
{

    /// <summary>
    /// 使用者ID
    /// </summary>
    [Key]
    [Column("UserTenantID")]
    public long UserTenantId { get; set; }

    /// <summary>
    /// 使用者代號
    /// </summary>
    [Column("UserID")]
    [StringLength(50)]
    public string UserId { get; set; } = userId;

    /// <summary>
    /// 帳號是否有效
    /// </summary>
    public bool? IsEnable { get; set; } = true;

    /// <summary>
    /// 密碼永遠有效
    /// </summary>
    public bool PasswordNeverExpire { get; set; }

    /// <summary>
    /// 需變更密碼
    /// </summary>
    public bool ChangingPassword { get; set; }

    /// <summary>
    /// Tenant代號
    /// </summary>
    [Column("TenantID")]
    [StringLength(20)]
    [Unicode(false)]
    public string TenantId { get; set; } = tenantId;

    [InverseProperty("UserTenant")]
    public virtual ICollection<ScuserRole> ScuserRoles { get; set; } = [];

    [ForeignKey("TenantId")]
    [InverseProperty("ScuserTenants")]
    public virtual Tenant Tenant { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("ScuserTenants")]
    public virtual Scuser User { get; set; } = null!;
}
