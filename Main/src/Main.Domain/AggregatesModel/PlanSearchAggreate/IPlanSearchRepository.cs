using Main.Domain.SeedWork;
namespace Main.Domain.AggregatesModel.PlanSearchAggreate;

public interface IPlanSearchRepository
{
    Task CreatePlanSearchHistoryAsync(string keyWord, string userId, DateTime createdDate, string createdUser, DateTime modifiedDate, string modifiedUser, string tenantId);
    Task RemoveOlderPlanSearchHistory(string userId, string tenantId);
}
