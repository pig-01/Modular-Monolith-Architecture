using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Main.Domain.AggregatesModel.CustomTemplateAggregate;
using Microsoft.EntityFrameworkCore;

namespace Main.Domain.AggregatesModel.PlanTemplateAggregate;

[Table("GriRule")]
public partial class GriRule
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Required]
    [StringLength(30)]
    public required string Code { get; set; }

    [StringLength(200)]
    public required string Description { get; set; }

    [StringLength(350)]
    public string? ChDescription { get; set; }

    [StringLength(350)]
    public string? EnDescription { get; set; }

    [StringLength(350)]
    public string? JpDescription { get; set; }

    [StringLength(200)]
    public required string Unit { get; set; }

    [StringLength(7)]
    [Unicode(false)]
    public required string TagColor { get; set; }

    [StringLength(500)]
    public required string Icon { get; set; }

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

    [InverseProperty("GriRule")]
    public virtual ICollection<PlanTemplateDetailGriRule> PlanTemplateDetailGriRules { get; set; } = [];
}
