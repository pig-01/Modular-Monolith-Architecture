using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Main.Domain.SeedWork;

namespace Main.Domain.AggregatesModel.PlanTemplateAggregate;

[Table("PlanTemplate")]
public partial class PlanTemplate : Entity, IAggregateRoot
{
    /// <summary>
    /// 指標計畫套版識別碼
    /// </summary>
    [Key]
    [Column("PlanTemplateID")]
    public int PlanTemplateId { get; set; }

    /// <summary>
    /// 指標計畫套版名稱
    /// </summary>
    [StringLength(200)]
    public string PlanTemplateName { get; set; } = null!;

    /// <summary>
    /// 指標計畫套版簡體中文名稱
    /// </summary>
    [StringLength(200)]
    public string? PlanTemplateChName { get; set; }

    /// <summary>
    /// 指標計畫套版英文名稱
    /// </summary>
    [StringLength(200)]
    public string? PlanTemplateEnName { get; set; }

    /// <summary>
    /// 指標計畫套版日文名稱
    /// </summary>
    [StringLength(200)]
    public string? PlanTemplateJpName { get; set; }


    [StringLength(100)]
    public string? AcceptDataSource { get; set; }

    /// <summary>
    /// 表單ID
    /// </summary>
    [Column("FormID")]
    public int? FormId { get; set; }

    /// <summary>
    /// 表單名稱
    /// </summary>
    [StringLength(200)]
    public string? FormName { get; set; }

    /// <summary>
    /// 分群識別碼
    /// </summary>
    [Column("GroupID")]
    [StringLength(50)]
    public string GroupId { get; set; } = null!;

    /// <summary>
    /// 版本
    /// </summary>
    [StringLength(10)]
    public string Version { get; set; } = null!;

    /// <summary>
    /// 指標識別碼
    /// </summary>
    [Column("IndicatorID")]
    [StringLength(100)]
    public string? IndicatorId { get; set; }

    /// <summary>
    /// 預設週期
    /// </summary>
    [StringLength(10)]
    public string? CycleType { get; set; }

    /// <summary>
    /// 是否已發佈
    /// </summary>
    public bool IsDeploy { get; set; }

    /// <summary>
    /// 指標編號
    /// </summary>
    public int? RowNumber { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public int? SortSequence { get; set; }

    [InverseProperty("PlanTemplate")]
    public virtual ICollection<PlanTemplateDetail> PlanTemplateDetails { get; set; } = [];

    [InverseProperty("PlanTemplate")]
    public virtual ICollection<PlanTemplateRequestUnit> PlanTemplateRequestUnits { get; set; } = [];

    [InverseProperty("PlanTemplate")]
    public virtual ICollection<PlanTemplateForm> PlanTemplateForms { get; set; } = [];

    // View欄位
    [NotMapped]
    public string? I18nGroupName { get; set; }
}
