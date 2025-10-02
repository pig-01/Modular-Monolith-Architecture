namespace Scheduler.Domain.AggregateModel.PlanAggregate;

[Table("PlanFactory")]
public partial class PlanFactory : Entity
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("PlanID")]
    public int PlanId { get; set; }

    [Column("FactoryID")]
    [StringLength(20)]
    public string FactoryId { get; set; } = null!;

    [Column("TenantID")]
    [StringLength(10)]
    [Unicode(false)]
    public string TenantId { get; set; } = null!;

    [ForeignKey("PlanId")]
    [InverseProperty("PlanFactories")]
    public virtual Plan Plan { get; set; } = null!;
}