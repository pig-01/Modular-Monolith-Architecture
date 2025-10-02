namespace Scheduler.Domain.AggregateModel.UserAggregate;

[Table("SCRole")]
[Index("RoleCode", "TenantId", Name = "UQ_SCRole_RoleCode", IsUnique = true)]
public partial class Scrole : Entity
{
    /// <summary>
    /// 角色ID
    /// </summary>
    [Key]
    [Column("RoleID")]
    public long RoleId { get; set; }

    /// <summary>
    /// 角色代號
    /// </summary>
    [Required]
    [StringLength(50)]
    public required string RoleCode { get; set; }

    /// <summary>
    /// 角色名稱
    /// </summary>
    [StringLength(50)]
    public required string RoleName { get; set; }

    /// <summary>
    /// 角色說明
    /// </summary>
    [StringLength(255)]
    public required string RoleDesc { get; set; }

    /// <summary>
    /// 角色類型  (UserDefinedCode)
    /// 100=組織
    /// 200=人事
    /// 700=保險
    /// S00=系統管理
    /// </summary>
    [Column("SCRoleType")]
    [StringLength(100)]
    [Unicode(false)]
    public required string ScroleType { get; set; }

    /// <summary>
    /// 是否有效
    /// </summary>
    public bool IsManager { get; set; }

    /// <summary>
    /// 是否有效
    /// </summary>
    public bool IsEffective { get; set; }

    /// <summary>
    /// Tenant代號：若為系統自訂的 Role，則本欄位空白
    /// </summary>
    [Required]
    [Column("TenantID")]
    [StringLength(20)]
    [Unicode(false)]
    public required string TenantId { get; set; }

    [InverseProperty("Role")]
    public virtual ICollection<ScuserRole> ScuserRoles { get; set; } = [];

    [InverseProperty("Role")]
    public virtual ICollection<ScroleFunction> ScroleFunctions { get; set; } = [];
}
