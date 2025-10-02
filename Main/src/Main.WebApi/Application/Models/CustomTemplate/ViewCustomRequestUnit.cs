using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Main.Dto.ViewModel.CustomTemplate;

public class ViewCustomRequestUnit
{
    /// <summary>
    /// 要求單位識別碼
    /// </summary>
    [JsonPropertyName("requestUnitId")]
    public long UnitId { get; set; }

    /// <summary>
    /// 要求單位名稱
    /// </summary>
    [JsonPropertyName("requestUnitName")]
    public string? UnitName { get; set; }

    /// <summary>
    /// 站台識別碼
    /// </summary>
    [JsonPropertyName("tenantId")]
    public string TenantId { get; set; } = null!;

    /// <summary>
    /// 是否有計畫使用此要求單位
    /// </summary>
    [JsonPropertyName("hasPlan")]
    public bool HasPlan => CustomPlanTemplateVersions.Any(v => v.HasPlan);

    /// <summary>
    /// 版本列表
    /// </summary>
    /// <value></value>
    [JsonPropertyName("versions")]
    public ICollection<ViewCustomPlanTemplateVersion> CustomPlanTemplateVersions { get; set; } = [];

    /// <summary>
    /// 最新已部署版本
    /// </summary>
    /// <value></value>
    [JsonPropertyName("lastVersion")]
    public ViewCustomPlanTemplateVersion? LastVersion =>
        CustomPlanTemplateVersions.Where(v => v.IsDeployed).OrderByDescending(v => v.VersionId).FirstOrDefault();
}
