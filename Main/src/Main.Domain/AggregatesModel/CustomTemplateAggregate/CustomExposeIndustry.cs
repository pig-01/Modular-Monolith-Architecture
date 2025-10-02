using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Main.Domain.SeedWork;

namespace Main.Domain.AggregatesModel.CustomTemplateAggregate;

/// <summary>
/// 自訂揭露產業
/// </summary>
[Table("CustomExposeIndustry")]
public partial class CustomExposeIndustry : Entity
{
    /// <summary>
    /// 揭露產業識別碼
    /// </summary>
    [Key]
    [Column("ID")]
    public long Id { get; set; }

    /// <summary>
    /// 指標計畫樣板明細識別碼
    /// </summary>
    [Column("PlanTemplateDetailID")]
    public long PlanTemplateDetailId { get; set; }

    /// <summary>
    /// 產業識別碼
    /// </summary>
    [Column("IndustryID")]
    [StringLength(30)]
    public string IndustryId { get; set; } = null!;

    [ForeignKey("PlanTemplateDetailId")]
    [InverseProperty("CustomExposeIndustries")]
    public virtual CustomPlanTemplateDetail PlanTemplateDetail { get; set; } = null!;

    public CustomExposeIndustry() { }

    /// <summary>
    /// 建立自訂揭露產業
    /// </summary>
    /// <param name="industryId"></param>
    /// <param name="createdUser"></param>
    public CustomExposeIndustry(string industryId, string createdUser)
    {
        IndustryId = industryId;
        SetCreateMetadata(createdUser, createdUser);
    }
}