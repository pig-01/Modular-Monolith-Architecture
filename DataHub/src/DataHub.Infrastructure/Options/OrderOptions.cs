namespace DataHub.Infrastructure.Options;

public class OrderOptions
{
    public const string Position = "Order";

    /// <summary>
    /// 流程取消撤回時，要TIGGER的ACT ID
    /// </summary>
    /// <value></value>
    public string? ABORTTIGGERACTDEFID { get; set; }

    /// <summary>
    /// 簽報單取消功能是否可查詢到代填單的流程
    /// </summary>
    public string? SEARCHINCLUDEAGT { get; set; }
    public string? WF21CONNECTIONID { get; set; }
    public string? RADARCONNECTIONID { get; set; }
    public string? OperationServicesUrl { get; set; }

    /// <summary>
    /// 表單是否用非同步作業
    /// </summary> 
    public string? ISFLOWASYNC { get; set; }

    /// <summary>
    /// 個人簽報單取消可查詢的表單狀態，預設""則不限制
    /// </summary>
    /// <value></value>
    public string? SelfCancelVisableFormState { get; set; }

}
