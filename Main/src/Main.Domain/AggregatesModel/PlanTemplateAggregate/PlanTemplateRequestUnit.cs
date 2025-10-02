using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Main.Domain.SeedWork;

namespace Main.Domain.AggregatesModel.PlanTemplateAggregate;

[Table("PlanTemplateRequestUnit")]
public partial class PlanTemplateRequestUnit : Entity
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("PlanTemplateID")]
    public int PlanTemplateId { get; set; }

    [Required]
    [StringLength(10)]
    public required string UnitCode { get; set; }

    [ForeignKey("PlanTemplateId")]
    [InverseProperty("PlanTemplateRequestUnits")]
    public virtual PlanTemplate? PlanTemplate { get; set; }
}
