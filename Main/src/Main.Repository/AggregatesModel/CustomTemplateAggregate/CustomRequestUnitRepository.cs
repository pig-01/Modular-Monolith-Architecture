using Base.Infrastructure.Toolkits.Extensions;
using Main.Domain.AggregatesModel.CustomTemplateAggregate;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Main.Repository.AggregatesModel.CustomTemplateAggregate;

public class CustomRequestUnitRepository(DemoContext context) : ICustomRequestUnitRepository
{
    public async Task<CustomRequestUnit> AddAsync(CustomRequestUnit entity, CancellationToken cancellationToken = default)
    {
        EntityEntry<CustomRequestUnit> result = await context.CustomRequestUnits.AddAsync(entity, cancellationToken);
        _ = await context.SaveChangesAsync(cancellationToken);
        return result.Entity;
    }

    /// <summary>
    /// 新增自訂要求單位
    /// </summary>
    /// <param name="unitId">要求單位識別碼</param>
    /// <param name="unitName">要求單位名稱</param>
    /// <param name="version">版本名稱</param>
    /// <param name="tenantId">站台識別碼</param>
    /// <param name="createdUser">建立人員</param>
    /// <param name="cancellationToken">取消憑證</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">有傳入要求單位識別碼但沒有自訂要求單位</exception>
    /// <exception cref="ArgumentException">要求單位名稱為空</exception>
    public async Task<CustomRequestUnit> AddAsync(long? unitId, string? unitName, string version, string tenantId, string createdUser, CancellationToken cancellationToken = default)
    {
        if (unitId.HasValue)
        {
            CustomRequestUnit? existingUnit = await context.CustomRequestUnits
                .FirstOrDefaultAsync(u => u.UnitId == unitId.Value, cancellationToken)
                ?? throw new InvalidOperationException($"CustomRequestUnit with UnitId {unitId.Value} does not exist.");

            existingUnit.AddVersion(version, createdUser);
            _ = await context.SaveChangesAsync(cancellationToken);
            return existingUnit;
        }
        else
        {
            if (string.IsNullOrWhiteSpace(unitName))
            {
                throw new ArgumentException("Unit name must be provided when UnitId is not specified.", nameof(unitName));
            }

            // Create new unit with version
            CustomRequestUnit newUnit = new(unitName, tenantId, createdUser);
            newUnit.AddVersion(version, createdUser);

            EntityEntry<CustomRequestUnit> result = await context.CustomRequestUnits.AddAsync(newUnit, cancellationToken);
            _ = await context.SaveChangesAsync(cancellationToken);
            return result.Entity;
        }
    }

    /// <summary>
    /// 新增自訂範本
    /// </summary>
    /// <param name="versionId">版本識別碼</param>
    /// <param name="customTemplates">自訂範本集合</param>
    /// <param name="createdUser">建立人員</param>
    /// <param name="cancellationToken">取消憑證</param>
    /// <returns></returns>
    public async Task<int> AddCustomTemplateAsync(long versionId, IEnumerable<CustomPlanTemplate> customTemplates, string createdUser, CancellationToken cancellationToken = default)
    {
        CustomPlanTemplateVersion version = await context.CustomPlanTemplateVersions.FirstOrDefaultAsync(v => v.VersionId == versionId, cancellationToken)
            ?? throw new InvalidOperationException($"CustomPlanTemplateVersion with Id {versionId} does not exist.");

        version.CustomPlanTemplates.AddRange(customTemplates);
        return await context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// 新增單一自訂範本
    /// </summary>
    /// <param name="customTemplate">自訂範本</param>
    /// <param name="cancellationToken">取消憑證</param>
    /// <returns></returns>
    public async Task<int> AddCustomTemplateAsync(CustomPlanTemplate customTemplate, CancellationToken cancellationToken = default)
    {
        EntityEntry<CustomPlanTemplate> result = await context.CustomPlanTemplates.AddAsync(customTemplate, cancellationToken);
        return await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(CustomRequestUnit entity, CancellationToken cancellationToken = default)
    {
        context.CustomRequestUnits.Remove(entity);
        _ = await context.SaveChangesAsync(cancellationToken);
    }

    public Task<int> DeleteAsync(long unitId, CancellationToken cancellationToken = default)
    {
        CustomRequestUnit customRequestUnit = context.CustomRequestUnits
            .Include(u => u.CustomPlanTemplateVersions)
            .ThenInclude(v => v.CustomPlanTemplates)
            .ThenInclude(t => t.CustomPlanTemplateDetails)
            .ThenInclude(d => d.CustomExposeIndustries)
            .FirstOrDefault(u => u.UnitId == unitId) ?? throw new InvalidOperationException($"CustomRequestUnit with UnitId {unitId} does not exist.");

        context.CustomRequestUnits.Remove(customRequestUnit);
        return context.SaveChangesAsync(cancellationToken);
    }
    public Task<int> DeleteVersionAsync(long unitId, long versionId, CancellationToken cancellationToken = default)
    {
        CustomPlanTemplateVersion version = context.CustomPlanTemplateVersions
            .Include(v => v.CustomPlanTemplates)
            .ThenInclude(t => t.CustomPlanTemplateDetails)
            .ThenInclude(d => d.CustomExposeIndustries)
            .FirstOrDefault(v => v.VersionId == versionId && v.UnitId == unitId)
            ?? throw new InvalidOperationException($"CustomPlanTemplateVersion with VersionId {versionId} and UnitId {unitId} does not exist.");

        context.CustomPlanTemplateVersions.Remove(version);
        return context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// 發布自訂指標計畫樣版版本
    /// </summary>
    /// <param name="requestUnitId">要求單位識別碼</param>
    /// <param name="versionId">版本識別碼</param>
    /// <param name="deployAt">發布日期</param>
    /// <param name="modifiedUser">修改人員</param>
    /// <param name="cancellationToken">取消憑證</param>
    /// <returns></returns>
    public Task<int> DeployVersionAsync(long requestUnitId, long versionId, DateTime deployAt, string modifiedUser, CancellationToken cancellationToken)
    {
        CustomPlanTemplateVersion customPlanTemplateVersion = context.CustomPlanTemplateVersions
            .FirstOrDefault(v => v.VersionId == versionId && v.UnitId == requestUnitId)
            ?? throw new InvalidOperationException($"CustomPlanTemplateVersion with VersionId {versionId} and UnitId {requestUnitId} does not exist.");

        customPlanTemplateVersion.Deploy(deployAt, modifiedUser);
        return context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// 重新命名要求單位
    /// </summary>
    /// <param name="unitId">要求單位識別碼</param>
    /// <param name="newName">要求單位新名稱</param>
    /// <param name="modifiedUser">修改人員</param>
    /// <param name="cancellationToken">取消憑證</param>
    /// <returns></returns>
    public async Task<CustomRequestUnit> RenameAsync(long unitId, string newName, string modifiedUser, CancellationToken cancellationToken = default)
    {
        CustomRequestUnit? customRequestUnit = await context.CustomRequestUnits.FirstOrDefaultAsync(u => u.UnitId == unitId, cancellationToken)
            ?? throw new InvalidOperationException($"CustomRequestUnit with UnitId {unitId} does not exist.");

        customRequestUnit.UnitName = newName;

        await context.SaveChangesAsync(cancellationToken);
        return customRequestUnit;
    }

    /// </summary>
    /// <param name="unitId">要求單位識別碼</param>
    /// <param name="versionId">版本識別碼</param>
    /// <param name="newName">要求單位版本新名稱</param>
    /// <param name="cancellationToken">取消憑證</param>
    /// <returns></returns>
    public async Task<CustomPlanTemplateVersion> RenameVersionAsync(long unitId, long versionId, string newName, string modifiedUser, CancellationToken cancellationToken = default)
    {
        CustomPlanTemplateVersion? version = await context.CustomPlanTemplateVersions
            .Include(v => v.Unit)
            .FirstOrDefaultAsync(v => v.VersionId == versionId && v.UnitId == unitId, cancellationToken)
            ?? throw new InvalidOperationException($"CustomPlanTemplateVersion with VersionId {versionId} and UnitId {unitId} does not exist.");

        version.Version = newName;

        await context.SaveChangesAsync(cancellationToken);
        return version;
    }

    public async Task UpdateAsync(CustomRequestUnit entity, CancellationToken cancellationToken = default)
    {
        context.CustomRequestUnits.Update(entity);
        _ = await context.SaveChangesAsync(cancellationToken);
    }
}
