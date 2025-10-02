using System.Text.Json.Serialization;
using Main.Dto.Model.Pagination;

namespace Main.Dto.ViewModel.Plan;

public class QueryPlanDocumentDataSplitedRequest : SortedPaginationModel<QueryPlanDocumentDataSplitedRequest>
{
    /// <summary>
    /// TenantID
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("tenantId")]
    public string? TenantID { get; set; }

    /// <summary>
    /// 是否為前三名公司
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("isTopThreeCompany")]
    public bool IsTopThreeCompany { get; set; }

    /// <summary>
    /// 是否為前三名區域
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("isTopThreeArea")]
    public bool IsTopThreeArea { get; set; }

    /// <summary>
    /// 計算排名前三的欄位清單
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("topThreeCustomNameFields")]
    public string? TopThreeCustomNameFields { get; set; }

    /// <summary>
    /// 要取得的欄位清單
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("customNameFields")]
    public string? CustomNameFields { get; set; }


    /// <summary>
    /// 公司ID清單
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("companyNameList")]
    public string? CompanyNameList { get; set; }

    /// <summary>
    /// 區域代碼清單
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("areaNameList")]
    public string? AreaNameList { get; set; }

    /// <summary>
    /// 週期類型
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("cycleType")]
    public string? CycleType { get; set; }

    /// <summary>
    /// 開始年份
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("startYear")]
    public DateTimeOffset StartYear { get; set; }

    /// <summary>
    /// 結束年份
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("endYear")]
    public DateTimeOffset EndYear { get; set; }
}
