using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Text.Json.Serialization;
using Base.Domain.Exceptions;
using Base.Infrastructure.Toolkits.Extensions;
using Main.Domain.Enums;
using Main.Domain.Events.PlanAggregate;
using Main.Domain.SeedWork;

namespace Main.Domain.AggregatesModel.PlanAggregate;

[Table("PlanDocument")]
public partial class PlanDocument : Entity
{
    [Key]
    [Column("PlanDocumentID")]
    public int PlanDocumentId { get; set; }

    [Column("PlanDetailID")]
    public int PlanDetailId { get; set; }

    [Column("DocumentID")]
    public int? DocumentId { get; set; }

    [StringLength(50)]
    public string? Responsible { get; set; }

    [StringLength(50)]
    public string? Approve { get; set; }

    [StringLength(3)]
    public string? FormStatus { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime StartDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime EndDate { get; set; }

    public int? Year { get; set; }

    public int? Quarter { get; set; }

    public int? Month { get; set; }

    [ForeignKey("PlanDetailId")]
    [InverseProperty("PlanDocuments")]
    [JsonIgnore]
    public virtual PlanDetail? PlanDetail { get; set; }

    [InverseProperty("PlanDocument")]
    public virtual ICollection<PlanDocumentData> PlanDocumentDatas { get; set; } = [];

    [InverseProperty("PlanDocument")]
    public virtual ICollection<PlanDocumentDataSplited> PlanDocumentDataSpliteds { get; set; } = [];

    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    public bool IsSingleMonth => Month != null && Quarter == null && Year == null;

    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    public bool IsSingleQuarter => Month == null && Quarter != null && Year == null;

    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    public bool IsSingleYear => Month == null && Quarter == null && Year != null;

    [Column("PlanFactoryID")]
    public int PlanFactoryId { get; set; }

    /// <summary>
    /// 建立指標計畫表單
    /// </summary>
    /// <param name="planDetailId">指標計畫明細識別碼</param>
    /// <param name="startDate">開始日期</param>
    /// <param name="endDate">結束日期</param>
    /// <param name="year">年度</param>
    /// <param name="quarter">季度</param>
    /// <param name="month">月份</param>
    /// <param name="createdUser">建立人員</param>
    /// <param name="modifiedUser">修改人員</param>
    public PlanDocument(int planDetailId, DateTime startDate, DateTime endDate, int planFactoryId, int? year, int? quarter, int? month, string createdUser, string modifiedUser)
    {
        PlanDetailId = planDetailId;
        StartDate = startDate;
        EndDate = endDate;
        PlanFactoryId = planFactoryId;
        SetCycleData(year, quarter, month);
        SetCreateMetadata(createdUser, modifiedUser);
    }

    /// <summary>
    /// 指派表單負責人
    /// </summary>
    /// <param name="responsible">指派人員</param>
    /// <param name="modifiedUser">修改人員</param>
    public void Assign(string responsible, string modifiedUser)
    {
        Responsible = responsible;
        FormStatus = ((int)DocumentStatus.UnWritten).ToString(CultureInfo.InvariantCulture);
        SetModifiedMetadata(modifiedUser);
    }

    /// <summary>
    /// 設定周期資料
    /// </summary>
    /// <param name="year">年度</param>
    /// <param name="quarter">季度</param>
    /// <param name="month">月份</param>
    public void SetCycleData(int? year, int? quarter, int? month)
    {
        Year = year;
        Quarter = quarter;
        Month = month;
    }

    /// <summary>
    /// 附加Bizform表單進入指標表單
    /// </summary>
    /// <param name="documentId">Bizform表單ID</param>
    public void AttachDocument(int documentId)
    {
        DocumentId = documentId;
        FormStatus = "2";   //更新狀態為 "待覆核"
    }

    /// <summary>
    /// 變更周期類型
    /// </summary>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    public void ChangeCycleType(DateTime startDate, DateTime endDate)
    {
        StartDate = startDate;
        EndDate = endDate;
    }

    /// <summary>
    /// 封存表單
    /// </summary>
    /// <param name="archiveDate"></param>
    /// <param name="archiveUser"></param>
    /// <returns></returns>
    public PlanDocumentLegacy Archive(DateTime archiveDate, string archiveUser)
    {
        AddDomainEvent(new PlanDocumentArchivedDomainEvent(this, archiveDate, archiveUser));

        return PlanDocumentLegacy.FromPlanDocument(this);
    }

    /// <summary>
    /// 重設表單資料
    /// </summary>
    /// <param name="modifiedUser"></param>
    public void ResetData(string modifiedUser)
    {
        PlanDocumentDatas.Clear();
        PlanDocumentDataSpliteds.Clear();
        SetModifiedMetadata(modifiedUser);
    }

    /// <summary>
    /// 建立表單資料
    /// </summary>
    /// <param name="datas">欄位清單</param>
    /// <param name="createdUser">建立人員</param>
    /// <param name="modifiedUser">修改人員</param>
    public void CreateData(List<PlanDocumentData> planDocumentDatas, string createdUser, string modifiedUser)
    {
        int documentId = DocumentId ?? throw new InvalidOperationException("DocumentId cannot be null when creating PlanDocumentData.");
        PlanDetail planDetail = PlanDetail ?? throw new InvalidOperationException("PlanDetail cannot be null when creating PlanDocumentData.");
        Plan plan = planDetail.Plan ?? throw new InvalidOperationException("Plan cannot be null when creating PlanDocumentData.");
        SetModifiedMetadata(modifiedUser);

        if (!int.TryParse(plan.Year, out int year))
        {
            throw new InvalidOperationException("Plan Year is not a valid integer.");
        }

        int cycleNumber = true switch
        {
            var _ when IsSingleQuarter => Quarter ?? throw new InvalidOperationException("Quarter cannot be null for single quarter data."),
            var _ when IsSingleYear => Year ?? throw new InvalidOperationException("Year cannot be null for single year data."),
            var _ when IsSingleMonth => Month ?? throw new InvalidOperationException("Month cannot be null for single month data."),
            _ => throw new ParameterException("Invalid AssignPlanDetail"),
        };

        foreach (PlanDocumentData planDocumentData in planDocumentDatas)
        {
            planDocumentData.BindData(this, createdUser, modifiedUser);
            PlanDocumentDatas.Add(planDocumentData);

            if (planDocumentData.IsSplited && (IsSingleYear || IsSingleQuarter))
            {
                List<PlanDocumentDataSplited> planDocumentDataSpliteds = planDocumentData.Split(this, year, createdUser);
                PlanDocumentDataSpliteds.AddRange(planDocumentDataSpliteds);
            }
            else
            {
                PlanDocumentDataSplited dataSplited = new(PlanDetail.CycleType, cycleNumber, planDocumentData, createdUser);
                dataSplited.BindData(planDocumentData, createdUser);
                PlanDocumentDataSpliteds.Add(dataSplited);
            }
        }
    }
}