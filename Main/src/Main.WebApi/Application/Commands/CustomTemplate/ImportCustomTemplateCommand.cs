using System.Text.Json.Serialization;
using Main.Dto.ViewModel.CustomTemplate;

namespace Main.WebApi.Application.Commands.CustomTemplate;

/// <summary>
/// 匯入自訂計畫範本命令
/// </summary>
public class ImportCustomTemplateCommand : IRequest<Unit?>
{
    /// <summary>
    /// 任務ID (用於進度追蹤)
    /// </summary>
    [JsonPropertyName("taskId")]
    public Guid TaskId { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 版本ID
    /// </summary>
    /// <value></value>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("versionId")]
    public required long VersionId { get; set; }

    /// <summary>
    /// Excel 匯入資料
    /// </summary>
    /// <value></value>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("dataList")]
    public IEnumerable<ViewCustomTemplateExcelData>? ExcelData { get; set; }
}
