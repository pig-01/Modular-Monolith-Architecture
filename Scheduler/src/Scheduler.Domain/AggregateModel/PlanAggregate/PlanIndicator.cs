namespace Scheduler.Domain.AggregateModel.PlanAggregate;

[Table("PlanIndicator")]
public partial class PlanIndicator : Entity
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("PlanID")]
    public int PlanId { get; set; }

    [Column("IndicatorID")]
    [StringLength(10)]
    public string IndicatorId { get; set; } = null!;

    [ForeignKey("PlanId")]
    [InverseProperty("PlanIndicators")]
    public virtual Plan Plan { get; set; } = null!;
}