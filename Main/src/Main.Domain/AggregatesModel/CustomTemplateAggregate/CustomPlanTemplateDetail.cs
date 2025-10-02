using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Main.Domain.SeedWork;

namespace Main.Domain.AggregatesModel.CustomTemplateAggregate;

/// <summary>
/// 自訂指標計畫樣版明細
/// </summary>
[Table("CustomPlanTemplateDetail")]
public partial class CustomPlanTemplateDetail : Entity
{
    /// <summary>
    /// 指標計畫樣版明細識別碼
    /// </summary>
    [Key]
    [Column("PlanTemplateDetailID")]
    public long PlanTemplateDetailId { get; set; }

    /// <summary>
    /// 指標計畫樣板識別碼
    /// </summary>
    [Column("PlanTemplateID")]
    public long PlanTemplateId { get; set; }

    /// <summary>
    /// 樣版明細名稱
    /// </summary>
    [StringLength(200)]
    public string Title { get; set; } = null!;

    /// <summary>
    /// 樣版明細名稱簡體中文
    /// </summary>
    [StringLength(200)]
    public string? TitleCh { get; set; }

    /// <summary>
    /// 樣版明細名稱英文
    /// </summary>
    [StringLength(200)]
    public string? TitleEn { get; set; }

    /// <summary>
    /// 樣版明細名稱日文
    /// </summary>
    [StringLength(200)]
    public string? TitleJp { get; set; }

    /// <summary>
    /// 排序編號
    /// </summary>
    public int? SortSequence { get; set; }

    [InverseProperty("PlanTemplateDetail")]
    public virtual ICollection<CustomExposeIndustry> CustomExposeIndustries { get; set; } = [];

    [ForeignKey("PlanTemplateId")]
    [InverseProperty("CustomPlanTemplateDetails")]
    public virtual CustomPlanTemplate PlanTemplate { get; set; } = null!;

    public CustomPlanTemplateDetail() { }

    /// <summary>
    /// 建立自訂指標計畫樣版明細
    /// </summary>
    /// <param name="title">樣版明細名稱</param>
    /// <param name="titleCh">樣版明細名稱簡體中文</param>
    /// <param name="titleEn">樣版明細名稱英文</param>
    /// <param name="titleJp">樣版明細名稱日文</param>
    /// <param name="sortSequence">排序編號</param>
    /// <param name="createdUser">建立人員</param>
    public CustomPlanTemplateDetail(string title, string? titleCh, string? titleEn, string? titleJp, int sortSequence, string[] customExposeIndustry, string createdUser)
    {
        Title = title;
        TitleCh = titleCh;
        TitleEn = titleEn;
        TitleJp = titleJp;
        SortSequence = sortSequence;
        CustomExposeIndustries = [.. customExposeIndustry.Select(industry => new CustomExposeIndustry(industry, createdUser))];
        SetCreateMetadata(createdUser, createdUser);
    }
}