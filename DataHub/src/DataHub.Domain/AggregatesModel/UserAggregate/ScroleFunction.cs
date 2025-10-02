using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataHub.Domain.SeedWork;


namespace DataHub.Domain.AggregatesModel.UserAggregate;

[Table("SCRoleFunctions")]
public partial class ScroleFunction : Entity
{
    /// <summary>
    /// 識別欄位
    /// </summary>
    [Key]
    [Column("RoleFunctionsID")]
    public long RoleFunctionsId { get; set; }

    /// <summary>
    /// 角色ID
    /// </summary>
    [Column("RoleID")]
    public long RoleId { get; set; }

    /// <summary>
    /// 功能項目代號
    /// </summary>
    [Column("FunctionID")]
    public long FunctionId { get; set; }

    [ForeignKey("RoleId")]
    [InverseProperty("ScroleFunctions")]
    public virtual Scrole Role { get; set; } = null!;
}
