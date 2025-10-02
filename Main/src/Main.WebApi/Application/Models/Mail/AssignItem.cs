namespace Main.WebApi.Application.Models.Mail;

public class AssignItem
{
    /// <summary>
    /// 指標明細 ID
    /// </summary>
    public int PlanDetailId { get; set; }

    /// <summary>
    /// 指標明細名稱
    /// </summary>
    public required string PlanDetailName { get; set; }

    /// <summary>
    /// 指標編號
    /// </summary>
    public string? RowNumber { get; set; }

    /// <summary>
    /// 結束日
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// 期別
    /// </summary>
    public required string CycleType { get; set; }

    /// <summary>
    /// 期別月份
    /// </summary>
    public int? CycleMonth { get; set; }

    /// <summary>
    /// 期別日期
    /// </summary>
    public int? CycleDay { get; set; }

    /// <summary>
    /// 期別是否為當月最後一天
    /// </summary>
    public bool CycleMonthLast { get; set; }

    /// <summary>
    /// 期別
    /// </summary>
    public int[] Cycles { get; set; } = [];

    /// <summary>
    /// 指標明細 URL
    /// </summary>
    public Uri? Url { get; set; }
}
