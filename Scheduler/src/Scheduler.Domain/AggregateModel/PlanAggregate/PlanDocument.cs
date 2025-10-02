namespace Scheduler.Domain.AggregateModel.PlanAggregate;

[Table("PlanDocument")]
public partial class PlanDocument : Entity
{
    /// <summary>
    /// 計畫明細表單識別碼
    /// </summary>
    [Key]
    [Column("PlanDocumentID")]
    public int PlanDocumentId { get; set; }

    /// <summary>
    /// 計畫明細識別碼
    /// </summary>
    [Column("PlanDetailID")]
    public int PlanDetailId { get; set; }

    /// <summary>
    /// 表單識別碼
    /// </summary>
    [Column("DocumentID")]
    public int? DocumentId { get; set; }

    /// <summary>
    /// 負責人
    /// </summary>
    [StringLength(50)]
    public string? Responsible { get; set; }

    /// <summary>
    /// 審核人
    /// </summary>
    [StringLength(50)]
    public string? Approve { get; set; }

    /// <summary>
    /// 進度代碼
    /// </summary>
    [StringLength(3)]
    public string? FormStatus { get; set; }

    /// <summary>
    /// 開始日期
    /// </summary>
    [Column(TypeName = "datetime")]
    public DateTime StartDate { get; set; }

    /// <summary>
    /// 結束日期
    /// </summary>
    [Column(TypeName = "datetime")]
    public DateTime EndDate { get; set; }

    /// <summary>
    /// 年度
    /// </summary>
    public int? Year { get; set; }

    /// <summary>
    /// 季度
    /// </summary>
    public int? Quarter { get; set; }

    /// <summary>
    /// 月份
    /// </summary>
    public int? Month { get; set; }

    /// <summary>
    /// 區域對應欄位
    /// </summary>
    [Column("PlanFactoryID")]
    public int? PlanFactoryId { get; set; }

    [ForeignKey("PlanDetailId")]
    [InverseProperty("PlanDocuments")]
    public virtual PlanDetail PlanDetail { get; set; } = null!;
}