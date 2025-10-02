using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Main.Domain.SeedWork;

namespace Main.Domain.AggregatesModel.PlanPermissionAggregate;

[Table("PlanPermission")]
public partial class PlanPermission : Entity, IAggregateRoot
{
    [Key]
    [Column("PlanPermissionID")]
    public int PlanPermissionID { get; set; }
    [Required]
    [Column("PlanID")]
    public int PlanId { get; set; }
    [Required]
    public required string Type { get; set; }
    public bool IsAll { get; set; }
    public bool OnlyCreated { get; set; }
    public bool OnlyResponseible { get; set; }
    public bool OnlyApprove { get; set; }
    public bool IsManager { get; set; }

    [InverseProperty("PlanPermission")]
    public virtual ICollection<PlanPermissionRelatedItem> PlanPermissionRelatedItems { get; set; } = [];
}
