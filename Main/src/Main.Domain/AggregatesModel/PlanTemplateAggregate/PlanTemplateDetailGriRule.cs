using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Main.Domain.AggregatesModel.PlanTemplateAggregate;

[Table("PlanTemplateDetailGriRule")]
public partial class PlanTemplateDetailGriRule
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("PlanTemplateDetailID")]
    public int PlanTemplateDetailId { get; set; }

    [Column("GriRuleID")]
    public int GriRuleId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedDate { get; set; }

    [Required]
    [StringLength(50)]
    public required string CreatedUser { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedDate { get; set; }

    [StringLength(50)]
    public required string ModifiedUser { get; set; }

    [ForeignKey("GriRuleId")]
    [InverseProperty("PlanTemplateDetailGriRules")]
    public virtual GriRule? GriRule { get; set; }

    [ForeignKey("PlanTemplateDetailId")]
    [InverseProperty("PlanTemplateDetailGriRules")]
    public virtual PlanTemplateDetail? PlanTemplateDetail { get; set; }
}
