using System.Linq.Expressions;
using Main.Domain.AggregatesModel.CustomTemplateAggregate;
using Main.Domain.AggregatesModel.SystemCodeAggregate;
using Main.Infrastructure.Demo.Context;

namespace Main.WebApi.Application.Queries.CustomTemplate.Impl;

public class CustomPlanTemplateVersionQuery(DemoContext context) : ICustomPlanTemplateVersionQuery
{
    public async Task<CustomPlanTemplateVersion?> GetByIdAsync(long id, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public async Task<CustomPlanTemplateVersion?> GetByIdAsync(long versionId, string tenantId, CancellationToken cancellationToken = default)
    {
        // 從 SystemData 獲取 I18nCode
        List<SystemData> systemDatas = await context.SystemDatas
            .AsNoTracking()
            .Where(sd => sd.CodeType == "DemoGroupId").ToListAsync(cancellationToken);

        // 查詢指定版本和租戶的 CustomPlanTemplateVersion
        CustomPlanTemplateVersion? result = await context.CustomPlanTemplateVersions
            .Include(x => x.CustomPlanTemplates)
            .ThenInclude(x => x.CustomPlanTemplateDetails)
            .Include(x => x.Unit)
            .AsSplitQuery()
            .AsNoTracking()
            .Where(x => x.VersionId == versionId && x.Unit.TenantId == tenantId)
            .FirstOrDefaultAsync(cancellationToken);

        if (result != null)
        {
            // 為每個 CustomPlanTemplate 和其 CustomPlanTemplateDetails 設定 I18nGroupName
            foreach (CustomPlanTemplate customPlanTemplate in result.CustomPlanTemplates)
            {
                SystemData? systemData = systemDatas.FirstOrDefault(sd => sd.SystemCode == customPlanTemplate.GroupId);
                string? i18nGroupName = systemData?.I18nCode;
                // 設定 CustomPlanTemplate 的 I18nGroupName
                customPlanTemplate.I18nGroupName = i18nGroupName;
            }
        }
        return result;
    }

    public async Task<IEnumerable<CustomPlanTemplateVersion>> ListAsync(CancellationToken cancellationToken = default) =>
        await context.CustomPlanTemplateVersions
            .Include(x => x.CustomPlanTemplates)
            .Include(x => x.Unit)
            .AsSplitQuery()
            .AsNoTracking()
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<CustomPlanTemplateVersion>> ListAsync(Expression<Func<CustomPlanTemplateVersion, bool>> predicate, CancellationToken cancellationToken = default) =>
        await context.CustomPlanTemplateVersions
            .Include(x => x.CustomPlanTemplates)
            .Include(x => x.Unit)
            .AsSplitQuery()
            .Where(predicate)
            .ToListAsync(cancellationToken);
}
