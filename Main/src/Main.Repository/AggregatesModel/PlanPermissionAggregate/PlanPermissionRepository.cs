using Main.Domain.AggregatesModel.PlanPermissionAggregate;
using Microsoft.Extensions.Logging;

namespace Main.Repository.AggregatesModel.PlanPermissionAggregate;

public class PlanPermissionRepository(DemoContext context, ILogger<PlanPermissionRepository> logger) : IPlanPermissionRepository
{
    public async Task<PlanPermission> AddAsync(PlanPermission entity, CancellationToken cancellationToken = default)
    {
        await context.PlanPermissions.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return entity;
    }
    public Task DeleteAsync(PlanPermission entity, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public async Task<List<PlanPermission>> GetAllByPlanIdAsync(int planId, CancellationToken cancellationToken = default) => await context.PlanPermissions
            .Where(pp => pp.PlanId == planId)
            .Include(pp => pp.PlanPermissionRelatedItems)
            .ToListAsync(cancellationToken);

    public async Task<List<PlanPermissionRelatedItem>> GetPlanPermissionUsersAsync(int id, CancellationToken cancellationToken = default)
    {
        // 不使用 Include，直接查詢
        var items = await context.PlanPermissionRelatedItems
            .Where(ppu => ppu.PlanPermissionId == id)
            .ToListAsync(cancellationToken);

        logger.LogInformation($"GetPlanPermissionUsersAsync found {items.Count} items without includes");

        return items;
    }

    public async Task<List<PlanPermissionRelatedItem>> GetPlanPermissionUsersWithUserInfoAsync(int id, CancellationToken cancellationToken = default)
    {
        // 先取得所有關聯項目（不使用 Include）
        List<PlanPermissionRelatedItem> result = await context.PlanPermissionRelatedItems
            .Where(ppu => ppu.PlanPermissionId == id)
            .ToListAsync(cancellationToken);

        logger.LogInformation($"Query found {result.Count} items");

        // 根據 RelatedType 手動載入相關資料
        foreach (PlanPermissionRelatedItem item in result)
        {
            switch (item.RelatedType.ToLower())
            {
                case "member":
                    item.UserTenant = await context.ScuserTenants
                        .Include(ut => ut.User)
                        .FirstOrDefaultAsync(ut => ut.UserTenantId == item.RelatedId, cancellationToken);
                    break;

                case "company":
                    item.CompanyEvent = await context.CompanyEvents
                        .FirstOrDefaultAsync(c => c.CompanyId == item.RelatedId, cancellationToken);
                    break;

                case "organization":
                    item.Organization = await context.Organizations
                        .FirstOrDefaultAsync(o => o.OrganizationId == item.RelatedId, cancellationToken);
                    break;
            }
        }

        return result;
    }

    public async Task AddPlanPermissionRelatedIdAsync(int planPermissionId, string relatedType, List<long> relatedIds, string executor, CancellationToken cancellationToken = default)
    {
        if (relatedIds == null || relatedIds.Count == 0)
            return;

        IEnumerable<PlanPermissionRelatedItem> planPermissionUsers = relatedIds.Select(relatedIds => new PlanPermissionRelatedItem
        {
            PlanPermissionId = planPermissionId,
            RelatedId = relatedIds,
            RelatedType = relatedType,
            CreatedUser = executor,
            ModifiedUser = executor
        });

        await context.PlanPermissionRelatedItems.AddRangeAsync(planPermissionUsers, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(PlanPermission entity, CancellationToken cancellationToken = default)
    {
        context.PlanPermissions.Update(entity);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<PlanPermission>> GetByPlanIdWithUsersAsync(int planId, CancellationToken cancellationToken = default) =>
        await context.PlanPermissions
        .Include(p => p.PlanPermissionRelatedItems)
        .Where(p => p.PlanId == planId)
        .ToListAsync(cancellationToken);

    public async Task RemovePlanPermissionUsersAsync(int planPermissionId, CancellationToken cancellationToken = default)
    {
        List<PlanPermissionRelatedItem> users = await context.PlanPermissionRelatedItems
            .Where(u => u.PlanPermissionId == planPermissionId)
            .ToListAsync(cancellationToken);

        if (users.Count != 0)
        {
            context.PlanPermissionRelatedItems.RemoveRange(users);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
    public async Task RemoveByPlanIdAsync(int planId, CancellationToken cancellationToken = default)
    {
        List<PlanPermission> entity = await context.PlanPermissions.Where(p => p.PlanId == planId).ToListAsync(cancellationToken);
        context.PlanPermissions.RemoveRange(entity);
        await context.SaveChangesAsync(cancellationToken);
    }
}
