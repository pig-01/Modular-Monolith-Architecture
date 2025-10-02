using System.Text.Json.Serialization;

namespace Main.Dto.ViewModel.CustomTemplate;

public class ViewCustomPlanTemplateVersion
{
    /// <summary>
    /// 指標計畫樣版版本識別碼
    /// </summary>
    [JsonPropertyName("versionId")]
    public long VersionId { get; set; }

    /// <summary>
    /// 版本名稱
    /// </summary>
    [JsonPropertyName("version")]
    public string Version { get; set; } = null!;

    /// <summary>
    /// 是否已部署
    /// </summary>
    /// <value></value>
    [JsonPropertyName("isDeployed")]
    public bool IsDeployed { get; set; }

    /// <summary>
    /// 部署時間
    /// </summary>
    /// <value></value>
    [JsonPropertyName("deployAt")]
    public DateTime? DeployAt { get; set; }

    /// <summary>
    /// 建立日期
    /// </summary>
    /// <value></value>
    [JsonPropertyName("createdDate")]
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// 建立使用者
    /// </summary>
    /// <value></value>
    [JsonPropertyName("createdUser")]
    public string? CreatedUser { get; set; }

    /// <summary>
    /// 修改日期
    /// </summary>
    /// <value></value>
    [JsonPropertyName("modifiedDate")]
    public DateTime? ModifiedDate { get; set; }

    /// <summary>
    /// 修改使用者
    /// </summary>
    /// <value></value>
    [JsonPropertyName("modifiedUser")]
    public string? ModifiedUser { get; set; }

    /// <summary>
    /// 是否有計畫使用此要求單位
    /// </summary>
    [JsonPropertyName("hasPlan")]
    public bool HasPlan { get; set; } = false;

    /// <summary>
    /// 自訂指標計畫樣版集合
    /// </summary>
    /// <value></value>
    [JsonPropertyName("customPlanTemplates")]
    public ICollection<ViewCustomPlanTemplate> CustomPlanTemplates { get; set; } = [];

    /// <summary>
    /// 樣版數量
    /// </summary>
    /// <value></value>
    public int TemplateCount => CustomPlanTemplates.Count;
}
