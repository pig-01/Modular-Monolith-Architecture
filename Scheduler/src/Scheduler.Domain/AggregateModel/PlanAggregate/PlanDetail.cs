namespace Scheduler.Domain.AggregateModel.PlanAggregate;

[Table("PlanDetail")]
public partial class PlanDetail : Entity
{
    [Key]
    [Column("PlanDetailID")]
    public int PlanDetailId { get; set; }

    [Column("PlanID")]
    public int PlanId { get; set; }

    [Column("PlanTemplateID")]
    public int PlanTemplateId { get; set; }

    [StringLength(200)]
    public string PlanDetailName { get; set; } = null!;

    [Column("FormID")]
    public long FormId { get; set; }

    [Column("GroupID")]
    [StringLength(50)]
    public string GroupId { get; set; } = null!;

    public bool Show { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? EndDate { get; set; }

    public int? RowNumber { get; set; }

    [StringLength(10)]
    public string? CycleType { get; set; }

    public int? CycleMonth { get; set; }

    public int? CycleDay { get; set; }

    public bool? CycleMonthLast { get; set; }

    [StringLength(200)]
    public string? PlanDetailChName { get; set; }

    [StringLength(300)]
    public string? PlanDetailEnName { get; set; }

    [StringLength(300)]
    public string? PlanDetailJpName { get; set; }

    [StringLength(100)]
    public string AcceptDataSource { get; set; } = null!;

    public int? NetzeroReportId { get; set; }

    public int? ApiConnectionId { get; set; }

    [StringLength(120)]
    public string? NetzeroReportName { get; set; }

    public bool ShowHint { get; set; }

    [StringLength(500)]
    public string? ResponsibleList { get; set; }

    [ForeignKey("PlanId")]
    [InverseProperty("PlanDetails")]
    public virtual Plan Plan { get; set; } = null!;

    [InverseProperty("PlanDetail")]
    public virtual ICollection<PlanDocument> PlanDocuments { get; set; } = [];
}