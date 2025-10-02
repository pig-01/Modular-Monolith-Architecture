using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Main.Domain.AggregatesModel.PlanAggregate;

[Table("PlanIndustry")]
public partial class PlanIndustry
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("PlanID")]
    public int PlanId { get; set; }

    [Column("IndustryID")]
    [StringLength(100)]
    public required string IndustryId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedDate { get; set; }

    [Required]
    [StringLength(50)]
    public required string CreatedUser { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedDate { get; set; }

    [StringLength(50)]
    public required string ModifiedUser { get; set; }

    [ForeignKey("PlanId")]
    [InverseProperty("PlanIndustries")]
    public virtual Plan? Plan { get; set; }
}
