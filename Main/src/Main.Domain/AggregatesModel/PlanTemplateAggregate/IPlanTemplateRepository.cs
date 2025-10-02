using Main.Domain.SeedWork;

namespace Main.Domain.AggregatesModel.PlanTemplateAggregate;

public interface IPlanTemplateRepository : IRepository<PlanTemplate>
{

    Task UpdateRangeAsync(PlanTemplate[] datas, CancellationToken cancellationToken = default);
    Task DeleteRangeByVersionAsync(string version, CancellationToken cancellationToken = default);
    Task<PlanTemplateDetail> AddPlanTemplateDetailAsync(PlanTemplateDetail data, CancellationToken cancellationToken = default);
    Task<PlanTemplateDetailExposeIndustry> AddDetailExposeIndustryAsync(PlanTemplateDetailExposeIndustry data, CancellationToken cancellationToken = default);
    Task<PlanTemplateDetailGriRule> AddDetailGriRuleAsync(PlanTemplateDetailGriRule data, CancellationToken cancellationToken = default);
    Task<PlanTemplateForm> AddFormAsync(PlanTemplateForm data, CancellationToken cancellationToken = default);
    Task<PlanTemplateRequestUnit> AddRequestUnitAsync(PlanTemplateRequestUnit data, CancellationToken cancellationToken = default);
    Task<int> DeployPlanTemplatesByVersionAsync(string version, DateTime modifiedDate, string modifiedUser, CancellationToken cancellationToken = default);
    Task<int> SyncPlanTemplateFormIdAsync(string version, string tenantId, Dictionary<string, long> formIdMap, CancellationToken cancellationToken = default);
}
