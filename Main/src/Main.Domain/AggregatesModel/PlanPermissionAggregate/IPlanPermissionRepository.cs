using Main.Domain.SeedWork;

namespace Main.Domain.AggregatesModel.PlanPermissionAggregate;

public interface IPlanPermissionRepository : IRepository<PlanPermission>
{
    Task<List<PlanPermission>> GetAllByPlanIdAsync(int planId, CancellationToken cancellationToken = default);
    Task<List<PlanPermissionRelatedItem>> GetPlanPermissionUsersAsync(int id, CancellationToken cancellationToken = default);
    Task<List<PlanPermissionRelatedItem>> GetPlanPermissionUsersWithUserInfoAsync(int id, CancellationToken cancellationToken = default);
    Task AddPlanPermissionRelatedIdAsync(int planPermissionId, string relatedType, List<long> relatedIds, string executor, CancellationToken cancellationToken = default);
    Task<List<PlanPermission>> GetByPlanIdWithUsersAsync(int planId, CancellationToken cancellationToken = default);
    Task RemovePlanPermissionUsersAsync(int planPermissionId, CancellationToken cancellationToken = default);
    Task UpdateAsync(PlanPermission entity, CancellationToken cancellationToken = default);
    Task RemoveByPlanIdAsync(int planId, CancellationToken cancellationToken = default);
}
