using Base.Domain.Models.Authentication;
using Main.Domain.AggregatesModel.PlanAggregate;
using Main.Domain.AggregatesModel.PlanSearchAggreate;
using Microsoft.Extensions.Logging;

namespace Main.Repository.AggregatesModel.PlanSearchAggregate;

public class PlanSearchRepository(DemoContext context, ILogger<PlanSearchRepository> logger) : IPlanSearchRepository
{
    public async Task CreatePlanSearchHistoryAsync(string keyWord, string userId, DateTime createdDate, string createdUser, DateTime modifiedDate, string modifiedUser, string tenantId)
    {
        //DONE 新增搜尋紀錄
        await context.PlanSearchHistories.AddAsync(PlanSearchHistory.Create(keyWord, userId, createdDate, createdUser, modifiedDate, modifiedUser, tenantId));
    }

    public async Task RemoveOlderPlanSearchHistory(string userId, string tenantId)
    {
        PlanSearchHistory? entity = await context.PlanSearchHistories.Where(x => x.UserId == userId && x.TenantId == tenantId).OrderByDescending(x => x.CreatedDate).FirstOrDefaultAsync();
        if (entity is not null)
        {
            context.PlanSearchHistories.Remove(entity);
        }
        await context.SaveChangesAsync();
    }
}
