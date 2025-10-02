using Main.Domain.SeedWork;

namespace Main.Domain.AggregatesModel.PlanAggregate;

public interface IPlanRepository : IRepository<Plan>
{
    Task SetPlanDetailApiConnectionFailedAsync(int planDetailId, bool failed, CancellationToken cancellationToken = default);
    Task<PlanDetail> AddPlanDetailAsync(PlanDetail planDetail, CancellationToken cancellationToken = default);
    Task UpdatePlanDetailAsync(PlanDetail planDetail, DateTime modifiedDate, CancellationToken cancellationToken = default);
    Task UpdatePlanDocumentAsync(PlanDocument planDocument, DateTime modifiedDate, CancellationToken cancellationToken = default);
    Task HidePlanAsync(int planId, DateTime hideDate, string hideUser, CancellationToken cancellationToken = default);
    Task HideMultiplePlansAsync(IEnumerable<int> planIds, DateTime hideDate, string hideUser, CancellationToken cancellationToken = default);

    /// <summary>
    /// 封存指標計畫
    /// </summary>
    /// <param name="planId"></param>
    /// <param name="archiveDate"></param>
    /// <param name="archiveUser"></param>
    /// <param name="cancellationToken">取消憑證</param>
    /// <returns></returns>
    Task ArchivePlanAsync(int planId, DateTime archiveDate, string archiveUser, CancellationToken cancellationToken = default);

    Task<PlanDocument> AddPlanDocumentAsync(PlanDocument model, CancellationToken cancellationToken = default);

    /// <summary>
    /// 封存指標計畫表單
    /// </summary>
    /// <param name="planDocumentId">表單ID</param>
    /// <param name="archiveDate">封存時間</param>
    /// <param name="archiveUser">封存人員</param>
    /// <param name="cancellationToken">取消憑證</param>
    /// <returns></returns>
    Task ArchivePlanDocumentAsync(int planDocumentId, DateTime archiveDate, string archiveUser, CancellationToken cancellationToken = default);

    Task AssignPlanDocumentAsync(int planId, string responsibleUser, string modifiedUser, IEnumerable<PlanDocument> planDocuments, CancellationToken cancellationToken = default);

    Task ConnectNetZeroToPlanDetailAsync(int apiConnectionId, int netZeroReportId, string netZeroReportName, string modifiedUser, DateTime modifiedDate, int planDetailId, CancellationToken cancellationToken = default);

    Task CancelConnectNetZeroToPlanDetailAsync(string modifiedUser, DateTime modifiedDate, int planDetailId, CancellationToken cancellationToken = default);

    Task HidePlanDetailHintAsync(string modifiedUser, DateTime modifiedDate, int planDetailId, CancellationToken cancellationToken = default);

    Task RemovePlanDocumentDataByPlanDocumentIdAsync(int planDocumentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 建立指標計畫表單資料
    /// </summary>
    /// <param name="documentId">指標計畫表單ID</param>
    /// <param name="planDocumentData">指標計畫表單資料</param>
    /// <param name="cancellationToken">取消憑證</param>
    /// <returns></returns>
    Task CreatePlanDocumentDataAsync(int documentId, List<PlanDocumentData> planDocumentDatas, string createUser, CancellationToken cancellationToken = default);
}