using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Main.Domain.AggregatesModel.PlanTemplateAggregate;

[Table("PlanTemplateDetail")]
public partial class PlanTemplateDetail
{
    [Key]
    [Column("PlanTemplateDetailID")]
    public int PlanTemplateDetailId { get; set; }

    [Column("PlanTemplateID")]
    public int PlanTemplateId { get; set; }

    [Required]
    [StringLength(200)]
    public required string Title { get; set; }

    [StringLength(200)]
    public string? ChTitle { get; set; }

    [StringLength(255)]
    public string? EnTitle { get; set; }

    [StringLength(255)]
    public string? JpTitle { get; set; }

    public int? SortSequence { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedDate { get; set; }

    [Required]
    [StringLength(50)]
    public required string CreatedUser { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedDate { get; set; }

    [StringLength(50)]
    public required string ModifiedUser { get; set; }

    [ForeignKey("PlanTemplateId")]
    [InverseProperty("PlanTemplateDetails")]
    public virtual PlanTemplate? PlanTemplate { get; set; }

    [InverseProperty("PlanTemplateDetail")]
    public virtual ICollection<PlanTemplateDetailGriRule> PlanTemplateDetailGriRules { get; set; } = [];

    [InverseProperty("PlanTemplateDetail")]
    public virtual ICollection<PlanTemplateDetailExposeIndustry> PlanTemplateDetailExposeIndustry { get; set; } = [];
}
