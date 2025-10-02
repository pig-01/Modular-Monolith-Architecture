using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Main.Domain.AggregatesModel.TenantAggregate;
using Main.Domain.AggregatesModel.UserAggregate;
using Main.Domain.SeedWork;

namespace Main.Domain.AggregatesModel.PlanPermissionAggregate;

[Table("PlanPermissionRelatedItem")]
public partial class PlanPermissionRelatedItem : Entity
{
    [Key]
    [Column("PlanPermissionRelatedItemID")]
    public int PlanPermissionRelatedItemId { get; set; }
    [Required]
    [Column("PlanPermissionID")]
    public int PlanPermissionId { get; set; }

    [Required]
    public required string RelatedType { get; set; }

    [Column("RelatedID")]
    public long RelatedId { get; set; }

    [ForeignKey("PlanPermissionId")]
    [InverseProperty("PlanPermissionRelatedItems")]
    public virtual PlanPermission PlanPermission { get; set; } = null!;

    // Navigation properties - 不使用 ForeignKey，改在 Configuration 中設定
    public virtual ScuserTenant? UserTenant { get; set; }

    public virtual CompanyEvent? CompanyEvent { get; set; }

    public virtual Organization? Organization { get; set; }
}