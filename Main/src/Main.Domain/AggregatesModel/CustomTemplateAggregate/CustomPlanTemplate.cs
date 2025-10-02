using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Main.Domain.SeedWork;

namespace Main.Domain.AggregatesModel.CustomTemplateAggregate;

/// <summary>
/// 自訂指標計畫樣版
/// </summary>
[Table("CustomPlanTemplate")]
public partial class CustomPlanTemplate : Entity
{
    /// <summary>
    /// 指標計畫樣版識別碼
    /// </summary>
    [Key]
    [Column("PlanTemplateID")]
    public long PlanTemplateId { get; set; }

    /// <summary>
    /// 指標計畫樣版版本識別碼
    /// </summary>
    [Column("VersionID")]
    public long VersionId { get; set; }

    /// <summary>
    /// 指標編號
    /// </summary>
    [StringLength(50)]
    public string? Code { get; set; }

    /// <summary>
    /// 指標計畫樣版名稱
    /// </summary>
    [StringLength(200)]
    public string PlanTemplateName { get; set; } = null!;

    /// <summary>
    /// 指標計畫樣版名稱簡體中文
    /// </summary>
    [StringLength(200)]
    public string? PlanTemplateNameCh { get; set; }

    /// <summary>
    /// 指標計畫樣版名稱英文
    /// </summary>
    [StringLength(200)]
    public string? PlanTemplateNameEn { get; set; }

    /// <summary>
    /// 指標計畫樣版名稱日文
    /// </summary>
    [StringLength(200)]
    public string? PlanTemplateNameJp { get; set; }

    /// <summary>
    /// 表單識別碼
    /// </summary>
    [Column("FormID")]
    public long? FormId { get; set; }

    /// <summary>
    /// 表單名稱
    /// </summary>
    [StringLength(200)]
    public string FormName { get; set; } = null!;

    /// <summary>
    /// 分群識別碼
    /// </summary>
    [Column("GroupID")]
    [StringLength(50)]
    public string GroupId { get; set; } = null!;

    /// <summary>
    /// 週期類型
    /// </summary>
    [StringLength(10)]
    public string? CycleType { get; set; }

    /// <summary>
    /// 排序編號
    /// </summary>
    public int? SortSequence { get; set; }

    [InverseProperty("PlanTemplate")]
    public virtual ICollection<CustomPlanTemplateDetail> CustomPlanTemplateDetails { get; set; } = [];

    [ForeignKey("VersionId")]
    [InverseProperty("CustomPlanTemplates")]
    public virtual CustomPlanTemplateVersion Version { get; set; } = null!;

    /// <summary>
    /// 分群名稱
    /// </summary>
    /// <value></value>
    [NotMapped]
    public string? GroupName { get; set; }

    /// <summary>
    /// 分群名稱
    /// </summary>
    /// <value></value>
    [NotMapped]
    public string? I18nGroupName { get; set; }

    /// <summary>
    /// 週期類型顯示名稱
    /// </summary>
    /// <value></value>
    [NotMapped]
    public string CycleTypeDisplay
    {
        get
        {
            return CycleType?.Trim() switch
            {
                "year" => "每年",
                "quarter" => "每季",
                "month" => "每月",
                _ => CycleType ?? string.Empty,
            } ?? string.Empty;
        }
    }

    public CustomPlanTemplate() { }

    /// <summary>
    /// 建立一個新的自訂指標計畫樣版實體
    /// </summary>
    /// <param name="planTemplateName">指標計畫樣版名稱</param>
    /// <param name="planTemplateNameCh">指標計畫樣版名稱簡體中文</param>
    /// <param name="planTemplateNameEn">指標計畫樣版名稱英文</param>
    /// <param name="planTemplateNameJp">指標計畫樣版名稱日文</param>
    /// <param name="formId">表單識別碼</param>
    /// <param name="formName">表單名稱</param>
    /// <param name="groupId">分群識別碼</param>
    /// <param name="cycleType">週期類型</param>
    /// <param name="code">指標編號</param>
    /// <param name="createdUser">建立人員</param>
    public CustomPlanTemplate(string planTemplateName, string? planTemplateNameCh, string? planTemplateNameEn, string? planTemplateNameJp, long formId, string formName, string groupId, string cycleType, string code, int sortSequence, string createdUser)
    {
        PlanTemplateName = planTemplateName;
        PlanTemplateNameCh = planTemplateNameCh;
        PlanTemplateNameEn = planTemplateNameEn;
        PlanTemplateNameJp = planTemplateNameJp;
        FormId = formId;
        FormName = formName;
        GroupId = groupId;
        CycleType = CycleTypeToDbValue(cycleType);
        Code = code;
        SortSequence = sortSequence;
        SetCreateMetadata(createdUser, createdUser);
    }

    /// <summary>
    /// 新增自訂指標計畫樣版明細 (支援多語言版本)
    /// </summary>
    /// <param name="title">樣版明細名稱</param>
    /// <param name="titleCh">樣版明細名稱簡體中文</param>
    /// <param name="titleEn">樣版明細名稱英文</param>
    /// <param name="titleJp">樣版明細名稱日文</param>
    /// <param name="sortSequence">排序編號</param>
    /// <param name="createdUser">建立人員</param>
    public void AddDetail(string? title, string? titleCh, string? titleEn, string? titleJp, int sortSequence, string[] customExposeIndustry, string createdUser)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title cannot be null or empty.", nameof(title));
        }

        CustomPlanTemplateDetail detail = new(title, titleCh, titleEn, titleJp, sortSequence, customExposeIndustry, createdUser);

        CustomPlanTemplateDetails.Add(detail);
    }

    /// <summary>
    /// 將週期類型轉換為資料庫儲存格式
    /// </summary>
    /// <param name="cycleType">週期類型</param>
    /// <returns></returns>
    private static string CycleTypeToDbValue(string cycleType)
    {
        return cycleType.Trim() switch
        {
            "每年" => "year",
            "每季" => "quarter",
            "每月" => "month",
            _ => throw new ArgumentException($"Invalid cycle type: {cycleType}", nameof(cycleType)),
        };
    }
}