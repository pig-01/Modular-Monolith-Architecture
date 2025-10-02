using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Main.Domain.SeedWork;

namespace Main.Domain.AggregatesModel.PlanAggregate;

[Table("PlanDetail")]
public partial class PlanDetail : Entity
{
    [Key]
    [Column("PlanDetailID")]
    public int PlanDetailId { get; set; }

    [Column("PlanID")]
    public int PlanId { get; set; }

    [Column("PlanTemplateID")]
    public int? PlanTemplateId { get; set; }

    [Required]
    [StringLength(200)]
    public required string PlanDetailName { get; set; }

    [StringLength(200)]
    public string? PlanDetailChName { get; set; }

    [StringLength(200)]
    public string? PlanDetailEnName { get; set; }

    [StringLength(200)]
    public string? PlanDetailJpName { get; set; }

    [Column("FormID")]
    public long FormId { get; set; }

    [StringLength(100)]
    public string? AcceptDataSource { get; set; }

    public int? ApiConnectionId { get; set; }

    public int? NetzeroReportId { get; set; }

    [StringLength(120)]
    public string? NetzeroReportName { get; set; }

    [Required]
    [Column("GroupID")]
    [StringLength(50)]
    public required string GroupId { get; set; }

    public bool Show { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? EndDate { get; set; }

    [StringLength(50)]
    public string? RowNumber { get; set; }

    [StringLength(10)]
    public required string CycleType { get; set; }

    public int? CycleMonth { get; set; }

    public int? CycleDay { get; set; }

    public bool? CycleMonthLast { get; set; }

    public bool? ShowHint { get; set; } = true;

    [StringLength(500)]
    public string? ResponsibleList { get; set; }
    public bool ApiConnectionFailed { get; set; }

    [Column("CustomPlanTemplateID")]
    public long? CustomPlanTemplateId { get; set; }

    [ForeignKey("PlanId")]
    [InverseProperty("PlanDetails")]
    public virtual Plan? Plan { get; set; }

    [InverseProperty("PlanDetail")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public virtual ICollection<PlanDocument> PlanDocuments { get; set; } = [];

    /// <summary>
    /// Change CycleType
    /// </summary>
    /// <remarks>
    /// year: 選擇一年中的某個日期
    /// quarter: 選擇一季中的第幾個月的第幾天或最後一天
    /// month: 選擇一個月中的第幾天或最後一天
    /// </remarks>
    /// <param name="cycleType">週期類型(year, quarter, month)</param>
    /// <param name="cycleMonth">週期月份</param>
    /// <param name="cycleDay">週期日</param>
    /// <param name="cycleMonthLast">週期月份是否為最後一個月</param>
    /// <param name="endDate">到期日</param>
    public void ChangeCycleType(string cycleType, int? cycleMonth, int? cycleDay, bool? cycleMonthLast, DateTime? endDate)
    {
        CycleType = cycleType;
        EndDate = endDate;

        switch (cycleType)
        {
            case "year":
                CycleMonth = null;
                CycleDay = null;
                CycleMonthLast = false;
                break;
            case "quarter":
                CycleMonth = cycleMonth;
                CycleDay = cycleMonthLast is null || !cycleMonthLast.Value ? cycleDay : null;
                CycleMonthLast = cycleMonthLast;
                break;
            case "month":
                CycleMonth = null;
                CycleDay = cycleMonthLast is null || !cycleMonthLast.Value ? cycleDay : null;
                CycleMonthLast = cycleMonthLast;
                break;
            default:
                break;
        }

    }

    /// <summary>
    /// 指派
    /// </summary>
    public void Assign(string responsibleList, string modifiedUser, DateTime modifiedDate)
    {
        ResponsibleList = responsibleList;
        ModifiedUser = modifiedUser;
        ModifiedDate = modifiedDate;
    }

    /// <summary>
    /// 串接NetZero
    /// </summary>
    /// <param name="apiConnectionId">年度</param>
    /// <param name="netZeroReportId">季度</param>
    public void ConnectNetZero(int apiConnectionId, int netZeroReportId, string netzeroReportName, string modifiedUser, DateTime modifiedDate)
    {
        ApiConnectionId = apiConnectionId;
        NetzeroReportId = netZeroReportId;
        NetzeroReportName = netzeroReportName;
        ModifiedUser = modifiedUser;
        ModifiedDate = modifiedDate;
    }

    /// <summary>
    /// 取消串接NetZero
    /// </summary>
    public void CancelConnectNetZero(string modifiedUser, DateTime modifiedDate)
    {
        ApiConnectionId = null;
        NetzeroReportId = null;
        NetzeroReportName = null;
        ModifiedUser = modifiedUser;
        ModifiedDate = modifiedDate;
        ShowHint = true;
    }

    /// <summary>
    /// 隱藏提示
    /// </summary>
    public void HideHint(string modifiedUser, DateTime modifiedDate)
    {
        ModifiedUser = modifiedUser;
        ModifiedDate = modifiedDate;
        ShowHint = false;
    }
}

