using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataHub.Domain.AggregatesModel.CustomerAggregate;
using DataHub.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;


namespace DataHub.Domain.AggregatesModel.UserAggregate;

[Table("SCUser")]
public partial class Scuser : Entity, IAggregateRoot
{
    /// <summary>
    /// 使用者代號
    /// </summary>
    [Key]
    [Column("UserID")]
    [StringLength(50)]
    public string UserId { get; set; } = null!;

    /// <summary>
    /// 使用者名稱
    /// </summary>
    [StringLength(50)]
    public string UserName { get; set; } = null!;

    /// <summary>
    /// 使用者密碼
    /// </summary>
    [StringLength(255)]
    public string PasswordHash { get; set; } = null!;

    /// <summary>
    /// 使用者類別
    /// </summary>
    [StringLength(1)]
    [Unicode(false)]
    public string UserType { get; set; } = null!;

    /// <summary>
    /// 使用者說明
    /// </summary>
    [StringLength(255)]
    public string? UserDesc { get; set; }

    /// <summary>
    /// 使用者電子郵件
    /// </summary>
    [StringLength(100)]
    public string? UserEmail { get; set; }

    /// <summary>
    /// 購買 Tenant 上限數
    /// </summary>
    public int MaxTenantNo { get; set; }

    /// <summary>
    /// 最近一次成功登入日期時間
    /// </summary>
    [Column(TypeName = "datetime")]
    public DateTime? LastSuccessfulLoginDate { get; set; }

    /// <summary>
    /// 最近一次失敗登入日期時間
    /// </summary>
    [Column(TypeName = "datetime")]
    public DateTime? LastFailedLoginDate { get; set; }

    /// <summary>
    /// 連續登入失敗次數
    /// </summary>
    public short RepeatedFailloginTimes { get; set; }

    /// <summary>
    /// 預設語系
    /// </summary>
    [StringLength(20)]
    [Unicode(false)]
    public string? Culture { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string? UserTel { get; set; }

    [StringLength(5)]
    [Unicode(false)]
    public string? UserTelArea { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string? UserTelExt { get; set; }

    [InverseProperty("User")]
    public virtual CurrentTenant? CurrentTenant { get; set; }

    [InverseProperty("User")]
    public virtual CustomerScuserRelation? CustomerScuserRelation { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<ScuserTenant> ScuserTenants { get; set; } = [];
}
