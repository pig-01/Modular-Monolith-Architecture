using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataHub.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;


namespace DataHub.Domain.AggregatesModel.UserAggregate;

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
    [StringLength(50)]
    public string RoleCode { get; set; } = null!;

    /// <summary>
    /// 角色名稱
    /// </summary>
    [StringLength(50)]
    public string? RoleName { get; set; }

    /// <summary>
    /// 角色說明
    /// </summary>
    [StringLength(255)]
    public string? RoleDesc { get; set; }

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
    public string? ScroleType { get; set; }

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
    [Column("TenantID")]
    [StringLength(20)]
    [Unicode(false)]
    public string TenantId { get; set; } = null!;

    [InverseProperty("Role")]
    public virtual ICollection<ScuserRole> ScuserRoles { get; set; } = [];

    [InverseProperty("Role")]
    public virtual ICollection<ScroleFunction> ScroleFunctions { get; set; } = [];

    /// <summary>
    /// 建立第一個新的 Scrole 實例
    /// </summary>
    /// <param name="pricingRole"></param>
    /// <returns></returns>
    public static Scrole Create(string roleCode, string roleName, string tenantId, string userId) =>
        // 從定價角色中資料建立的角色都是Manager角色(預設角色)
        new()
        {
            RoleCode = roleCode,
            RoleName = roleName,
            RoleDesc = "定價方案預設角色",
            ScroleType = "S00",
            IsManager = true,
            IsEffective = true,
            TenantId = tenantId,
            CreatedUser = userId,
            ModifiedUser = userId
        };
}
