namespace Scheduler.Application.Models.Mails;

// 新增輔助類別
public class ResponsibleNotificationModel
{
    public string Responsible { get; set; } = string.Empty;
    public int TotalDocuments { get; set; }
    public List<PlanDocumentGroup> PlanGroups { get; set; } = [];
    public string NotificationType { get; set; } = string.Empty;
    public int DaysUntilExpiration { get; set; }
}

public class PlanDocumentGroup
{
    public int PlanId { get; set; }
    public string PlanName { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public List<DocumentSummary> Documents { get; set; } = [];
}

public class DocumentSummary
{
    public int PlanDocumentId { get; set; }
    public string PlanDetailName { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public DateTime EndDate { get; set; }
    public string? FormStatus { get; set; }
    public int DaysOverdue { get; set; }
    public int? Year { get; set; }
    public int? Quarter { get; set; }
    public int? Month { get; set; }

    /// <summary>
    /// 指標明細 URL
    /// </summary>
    public Uri? Url { get; set; }

    public string CycleDescription => (Year, Quarter, Month) switch
    {
        (not null, null, null) => $"{Year}年",
        (not null, not null, null) => $"{Year}年第{Quarter}季",
        (not null, null, not null) => $"{Year}年{Month}月",
        _ => "未定義週期"
    };
}
