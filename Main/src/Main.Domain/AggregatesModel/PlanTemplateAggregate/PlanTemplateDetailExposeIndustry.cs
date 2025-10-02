using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Main.Domain.AggregatesModel.PlanTemplateAggregate;

[Table("PlanTemplateDetailExposeIndustry")]
public partial class PlanTemplateDetailExposeIndustry
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("PlanTemplateDetailID")]
    public int PlanTemplateDetailId { get; set; }

    [Required]
    [Column("IndustryID")]
    [StringLength(30)]
    public string IndustryId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedDate { get; set; }

    [Required]
    [StringLength(50)]
    public string CreatedUser { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedDate { get; set; }

    [StringLength(50)]
    public string ModifiedUser { get; set; }

    [ForeignKey("PlanTemplateDetailId")]
    [InverseProperty("PlanTemplateDetailExposeIndustry")]
    public virtual PlanTemplateDetail? PlanTemplateDetail { get; set; }
}
