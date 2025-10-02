namespace Scheduler.Domain.AggregateModel.PlanAggregate;

[Table("Plan")]
public partial class Plan : Entity, IAggregateRoot
{
    [Key]
    [Column("PlanID")]
    public int PlanId { get; set; }

    [Column("TenantID")]
    [StringLength(20)]
    [Unicode(false)]
    public string TenantId { get; set; } = null!;

    [StringLength(200)]
    public string PlanName { get; set; } = null!;

    [StringLength(4)]
    [Unicode(false)]
    public string? Year { get; set; }

    public bool Show { get; set; }

    public bool Archived { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ArchivedDate { get; set; }

    [StringLength(50)]
    public string? ArchivedUser { get; set; }

    [Column("CompanyID")]
    public long CompanyId { get; set; }

    [StringLength(30)]
    public string? PlanTemplateVersion { get; set; }

    [InverseProperty("Plan")]
    public virtual ICollection<PlanDetail> PlanDetails { get; set; } = [];

    [InverseProperty("Plan")]
    public virtual ICollection<PlanFactory> PlanFactories { get; set; } = [];

    [InverseProperty("Plan")]
    public virtual ICollection<PlanIndicator> PlanIndicators { get; set; } = [];

    [InverseProperty("Plan")]
    public virtual ICollection<PlanIndustry> PlanIndustries { get; set; } = [];
}