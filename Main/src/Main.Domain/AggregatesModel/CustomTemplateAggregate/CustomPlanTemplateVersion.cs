using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Main.Domain.SeedWork;

namespace Main.Domain.AggregatesModel.CustomTemplateAggregate;

/// <summary>
/// 自訂指標計畫樣版版本
/// </summary>
[Table("CustomPlanTemplateVersion")]
public partial class CustomPlanTemplateVersion : Entity
{
    /// <summary>
    /// 指標計畫樣版版本識別碼
    /// </summary>
    [Key]
    [Column("VersionID")]
    public long VersionId { get; set; }

    /// <summary>
    /// 要求單位識別碼
    /// </summary>
    [Column("UnitID")]
    public long UnitId { get; set; }

    /// <summary>
    /// 版本名稱
    /// </summary>
    [StringLength(256)]
    public string Version { get; set; } = null!;

    /// <summary>
    /// 發布日期
    /// </summary>
    public DateTime? DeployAt { get; set; }

    [InverseProperty("Version")]
    public virtual ICollection<CustomPlanTemplate> CustomPlanTemplates { get; set; } = [];

    [ForeignKey("UnitId")]
    [InverseProperty("CustomPlanTemplateVersions")]
    public virtual CustomRequestUnit Unit { get; set; } = null!;

    /// <summary>
    /// 是否已被計畫使用
    /// </summary>
    /// <value></value>
    [NotMapped]
    public bool HasPlan { get; set; }

    /// <summary>
    /// 是否已發布
    /// </summary>
    [NotMapped]
    public bool IsDeployed => DeployAt.HasValue;

    /// <summary>
    /// 樣版數量
    /// </summary>
    [NotMapped]
    public int TemplateCount => CustomPlanTemplates.Count;

    public CustomPlanTemplateVersion() { }

    /// <summary>
    /// 建立一個新的自訂指標計畫樣版版本
    /// </summary>
    /// <param name="version">版本名稱</param>
    /// <param name="createdUser">建立人員</param>
    public CustomPlanTemplateVersion(string version, string createdUser)
    {
        Version = version;
        DeployAt = null;
        SetCreateMetadata(createdUser, createdUser);
    }

    /// <summary>
    /// 發布自訂指標計畫樣版版本
    /// </summary>
    /// <param name="deployAt">發布日期</param>
    /// <param name="modifiedUser">修改人員</param>
    public void Deploy(DateTime deployAt, string modifiedUser)
    {
        DeployAt = deployAt;
        SetModifiedMetadata(modifiedUser);
    }

    /// <summary>
    /// 檢查是否有被計畫使用
    /// </summary>
    /// <param name="versionIds">已使用的版本識別碼清單</param>
    public void CheckHasPlan(IEnumerable<long> versionIds)
    {
        HasPlan = versionIds.Contains(VersionId);
    }
}