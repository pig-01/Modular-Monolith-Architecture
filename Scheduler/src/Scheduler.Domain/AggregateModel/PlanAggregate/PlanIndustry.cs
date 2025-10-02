namespace Scheduler.Domain.AggregateModel.PlanAggregate;

[Table("PlanIndustry")]
public partial class PlanIndustry : Entity
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("PlanID")]
    public int PlanId { get; set; }

    [Column("IndustryID")]
    [StringLength(100)]
    public string IndustryId { get; set; } = null!;

    [ForeignKey("PlanId")]
    [InverseProperty("PlanIndustries")]
    public virtual Plan Plan { get; set; } = null!;
}