using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Main.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;

namespace Main.Domain.AggregatesModel.PlanTemplateAggregate;

[Table("PlanTemplateForm")]
[Index("PlanTemplateId", "FormId", Name = "UQ_PlanTemplateForm", IsUnique = true)]
public partial class PlanTemplateForm : Entity
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("PlanTemplateID")]
    public int PlanTemplateId { get; set; }

    [Column("FormID")]
    public long FormId { get; set; }

    [Required]
    [Column("TenantID")]
    [StringLength(10)]
    public required string TenantId { get; set; }

    [ForeignKey("PlanTemplateId")]
    [InverseProperty("PlanTemplateForms")]
    public virtual PlanTemplate? PlanTemplate { get; set; }
}
