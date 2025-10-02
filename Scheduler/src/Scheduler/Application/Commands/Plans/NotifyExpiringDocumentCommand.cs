using MediatR;

namespace Scheduler.Application.Commands.Plans;

/// <summary>
/// 過期表單通知命令
/// </summary>
public class NotifyExpiringDocumentCommand : IRequest<NotifyExpiringDocumentResult>
{
    /// <summary>
    /// 距離過期的天數 (0 = 今日過期, 負數 = 已過期, 正數 = 即將過期)
    /// </summary>
    public int DaysUntilExpiration { get; set; }

    /// <summary>
    /// 是否為預警通知 (true = 預警, false = 過期通知)
    /// </summary>
    public bool IsWarningNotification { get; set; }

    /// <summary>
    /// 執行者識別碼
    /// </summary>
    public string ExecutedBy { get; set; } = "SYSTEM";

    public NotifyExpiringDocumentCommand() { }

    public NotifyExpiringDocumentCommand(int daysUntilExpiration, bool isWarningNotification = false, string executedBy = "SYSTEM")
    {
        DaysUntilExpiration = daysUntilExpiration;
        IsWarningNotification = isWarningNotification;
        ExecutedBy = executedBy;
    }
}
/// <summary>
/// 過期表單通知結果
/// </summary>
public class NotifyExpiringDocumentResult
{
    /// <summary>
    /// 處理成功的表單數量
    /// </summary>
    public int ProcessedCount { get; set; }

    /// <summary>
    /// 發送的通知數量
    /// </summary>
    public int NotificationsSent { get; set; }

    /// <summary>
    /// 執行是否成功
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// 錯誤訊息 (如果有)
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 處理的表單摘要資訊
    /// </summary>
    public List<ExpiredDocumentSummary> DocumentSummaries { get; set; } = [];
}

/// <summary>
/// 過期表單摘要
/// </summary>
public class ExpiredDocumentSummary
{
    /// <summary>
    /// 指標計畫表單識別碼
    /// </summary>
    public int PlanDocumentId { get; set; }

    /// <summary>
    /// 負責人
    /// </summary>
    public string? Responsible { get; set; }

    /// <summary>
    /// 結束日期
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// 表單狀態
    /// </summary>
    public string? FormStatus { get; set; }

    /// <summary>
    /// 已過期天數
    /// </summary>
    public int DaysOverdue { get; set; }
}