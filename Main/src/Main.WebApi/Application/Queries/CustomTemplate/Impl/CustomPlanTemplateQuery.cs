using System.Linq.Expressions;
using Main.Domain.AggregatesModel.CustomTemplateAggregate;
using Main.Domain.AggregatesModel.SystemCodeAggregate;
using Main.Infrastructure.Demo.Context;

namespace Main.WebApi.Application.Queries.CustomTemplate.Impl;

public class CustomPlanTemplateQuery(DemoContext context) : ICustomPlanTemplateQuery
{
    public async Task<CustomPlanTemplate?> GetByIdAsync(long id, CancellationToken cancellationToken = default) => await context.CustomPlanTemplates
        .Include(x => x.CustomPlanTemplateDetails)
        .Include(x => x.Version)
        .ThenInclude(x => x.Unit)
        .AsNoTracking()
        .AsSplitQuery()
        .FirstOrDefaultAsync(x => x.PlanTemplateId == id, cancellationToken);

    /// <summary>
    /// 取得指定要求單位的最新版本的自訂指標計畫套版
    /// </summary>
    /// <param name="requestUnitId">自訂要求單位識別碼</param>
    /// <param name="tenantId">站台識別碼</param>
    /// <param name="cancellationToken">取消憑證</param>
    /// <returns></returns>
    public async Task<IEnumerable<CustomPlanTemplate?>> GetLastVersionAsync(long requestUnitId, string tenantId, CancellationToken cancellationToken = default)
    {
        CustomRequestUnit customRequestUnit = await context.CustomRequestUnits
            .Include(x => x.CustomPlanTemplateVersions)
            .ThenInclude(x => x.CustomPlanTemplates)
            .ThenInclude(x => x.CustomPlanTemplateDetails)
            .AsNoTracking()
            .AsSplitQuery()
            .FirstOrDefaultAsync(x => x.UnitId == requestUnitId && x.TenantId == tenantId, cancellationToken) ??
        throw new ArgumentException("CustomRequestUnit not found");

        CustomPlanTemplateVersion customPlanTemplateVersion =
            customRequestUnit.LastVersion ?? throw new ArgumentException("CustomRequestUnit has no version");

        // 從 SystemData 獲取 I18nCode
        List<SystemData> systemDatas = await context.SystemDatas
            .AsNoTracking()
            .Where(sd => sd.CodeType == "DemoGroupId").ToListAsync(cancellationToken);

        // 為每個 CustomPlanTemplate 和其 CustomPlanTemplateDetails 設定 I18nGroupName
        foreach (CustomPlanTemplate customPlanTemplate in customPlanTemplateVersion.CustomPlanTemplates)
        {
            SystemData? systemData = systemDatas.FirstOrDefault(sd => sd.SystemCode == customPlanTemplate.GroupId);

            string? i18nGroupName = systemData?.I18nCode;

            // 設定 CustomPlanTemplate 的 I18nGroupName
            customPlanTemplate.I18nGroupName = i18nGroupName;
        }

        return customPlanTemplateVersion.CustomPlanTemplates;
    }

    public async Task<IEnumerable<CustomPlanTemplate>> ListAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public async Task<IEnumerable<CustomPlanTemplate>> ListAsync(Expression<Func<CustomPlanTemplate, bool>> predicate, CancellationToken cancellationToken = default)
    {
        List<CustomPlanTemplate> customPlanTemplates = await context.CustomPlanTemplates
            .Include(x => x.CustomPlanTemplateDetails)
            .Include(x => x.Version)
            .ThenInclude(x => x.Unit)
            .AsNoTracking()
            .AsSplitQuery()
            .Where(predicate)
            .ToListAsync(cancellationToken);

        // 從 SystemData 獲取 I18nCode
        List<SystemData> systemDatas = await context.SystemDatas
            .AsNoTracking()
            .Where(sd => sd.CodeType == "DemoGroupId").ToListAsync(cancellationToken);

        // 為每個 CustomPlanTemplate 和其 CustomPlanTemplateDetails 設定 I18nGroupName
        foreach (CustomPlanTemplate customPlanTemplate in customPlanTemplates)
        {
            SystemData? systemData = systemDatas.FirstOrDefault(sd => sd.SystemCode == customPlanTemplate.GroupId);

            string? i18nGroupName = systemData?.I18nCode;

            // 設定 CustomPlanTemplate 的 I18nGroupName
            customPlanTemplate.I18nGroupName = i18nGroupName;
            customPlanTemplate.GroupName = systemData?.Name;
        }

        return customPlanTemplates;
    }
}
