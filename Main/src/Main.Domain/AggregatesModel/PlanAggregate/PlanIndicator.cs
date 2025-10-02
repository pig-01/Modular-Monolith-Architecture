using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Main.Domain.AggregatesModel.PlanAggregate;

[Table("PlanIndicator")]
public partial class PlanIndicator
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("PlanID")]
    public int PlanId { get; set; }

    [Column("RequestUnitID")]
    public long? RequestUnitId { get; set; }

    [Column("VersionID")]
    public long? VersionId { get; set; }

    [Column("IndicatorID")]
    public string? IndicatorId { get; set; }

    [Column("IndicatorType")]
    [StringLength(10)]
    public string? IndicatorType { get; set; }

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
    [InverseProperty("PlanIndicators")]
    public virtual Plan? Plan { get; set; }
}