using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataHub.Domain.SeedWork;


namespace DataHub.Domain.AggregatesModel.UserAggregate;

[Table("SCUserRoles")]
public partial class ScuserRole : Entity
{
    /// <summary>
    /// 識別欄位
    /// </summary>
    [Key]
    [Column("UserRolesID")]
    public long UserRolesId { get; set; }

    /// <summary>
    /// 使用者ID
    /// </summary>
    [Column("UserTenantID")]
    public long UserTenantId { get; set; }

    /// <summary>
    /// 角色ID
    /// </summary>
    [Column("RoleID")]
    public long RoleId { get; set; }

    [ForeignKey("RoleId")]
    [InverseProperty("ScuserRoles")]
    public virtual Scrole Role { get; set; } = null!;

    [ForeignKey("UserTenantId")]
    [InverseProperty("ScuserRoles")]
    public virtual ScuserTenant UserTenant { get; set; } = null!;

    public static ScuserRole Create(long roleId, long userTenantId, string userId) => new()
    {
        RoleId = roleId,
        UserTenantId = userTenantId,
        CreatedUser = userId,
        ModifiedUser = userId
    };
}
