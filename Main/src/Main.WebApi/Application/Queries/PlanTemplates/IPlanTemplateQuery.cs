using Main.Domain.AggregatesModel.PlanTemplateAggregate;
using Main.Domain.SeedWork;
using Main.Dto.ViewModel.PlanTemplate;

namespace Main.WebApi.Application.Queries.PlanTemplates;

public interface IPlanTemplateQuery : IQuery<PlanTemplate>
{
    Task<IEnumerable<PlanTemplate>> ListAsync(string tenantId, string[]? indicatorIds = null, string? version = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<GriRule>> GetGriRulesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> GetVersionListAsync(bool isAdmin = false, CancellationToken cancellationToken = default);
    Task<IEnumerable<ViewPlanTemplateExcelData>> GetPlanTemplateExcelDataAsync(string version, CancellationToken cancellationToken = default);
}
