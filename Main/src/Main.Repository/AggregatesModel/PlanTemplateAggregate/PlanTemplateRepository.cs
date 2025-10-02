using Main.Domain.AggregatesModel.PlanTemplateAggregate;

namespace Main.Repository.AggregatesModel.PlanTemplateAggregate;

public class PlanTemplateRepository(DemoContext context) : IPlanTemplateRepository
{
    private readonly DemoContext context = context;

    public async Task<PlanTemplate> AddAsync(PlanTemplate entity, CancellationToken cancellationToken = default)
    {
        PlanTemplate planTemplate = (await context.PlanTemplates.AddAsync(entity, cancellationToken)).Entity;
        await context.SaveChangesAsync(cancellationToken);
        return planTemplate;
    }

    public async Task<PlanTemplateDetail> AddPlanTemplateDetailAsync(PlanTemplateDetail data, CancellationToken cancellationToken = default)
    {

        PlanTemplateDetail planTemplateDetail = (await context.PlanTemplateDetails.AddAsync(data, cancellationToken)).Entity;
        await context.SaveChangesAsync(cancellationToken);
        return planTemplateDetail;
    }

    public async Task<PlanTemplateDetailExposeIndustry> AddDetailExposeIndustryAsync(PlanTemplateDetailExposeIndustry data, CancellationToken cancellationToken = default) => (await context.PlanTemplateDetailExposeIndustries.AddAsync(data, cancellationToken)).Entity;

    public async Task<PlanTemplateDetailGriRule> AddDetailGriRuleAsync(PlanTemplateDetailGriRule data, CancellationToken cancellationToken = default) => (await context.PlanTemplateDetailGriRules.AddAsync(data, cancellationToken)).Entity;

    public async Task UpdateRangeAsync(PlanTemplate[] datas, CancellationToken cancellationToken = default) => context.PlanTemplates.UpdateRange(datas);

    public async Task<int> DeployPlanTemplatesByVersionAsync(string version, DateTime modifiedDate, string modifiedUser, CancellationToken cancellationToken = default)
    {
        // 查詢指定版本的所有計畫樣板
        List<PlanTemplate> planTemplates = await context.PlanTemplates
            .Where(x => x.Version == version)
            .ToListAsync(cancellationToken);

        if (planTemplates.Count == 0)
        {
            return 0;
        }

        // 更新所有計畫樣板的 IsDeploy 欄位
        foreach (PlanTemplate? template in planTemplates)
        {
            template.IsDeploy = true;
            template.ModifiedDate = modifiedDate;
            template.ModifiedUser = modifiedUser;
        }

        // 批次更新資料庫
        context.PlanTemplates.UpdateRange(planTemplates);
        await context.SaveChangesAsync(cancellationToken);

        return planTemplates.Count;
    }

    public async Task DeleteRangeByVersionAsync(string version, CancellationToken cancellationToken = default)
    {
        // 載入完整的實體圖，包含所有關聯資料
        List<PlanTemplate> templates = await context.PlanTemplates
            .Include(x => x.PlanTemplateForms)
            .Include(x => x.PlanTemplateRequestUnits)
            .Include(pt => pt.PlanTemplateDetails)
                .ThenInclude(ptd => ptd.PlanTemplateDetailGriRules)
            .Include(pt => pt.PlanTemplateDetails)
                .ThenInclude(ptd => ptd.PlanTemplateDetailExposeIndustry)
            .Where(pt => pt.Version == version)
            .ToListAsync(cancellationToken);

        // 標記所有實體為刪除狀態，EF Core 會根據配置自動處理級聯刪除
        context.PlanTemplates.RemoveRange(templates);

        // 儲存變更，EF Core 會自動生成正確順序的 SQL 語句
        await context.SaveChangesAsync(cancellationToken);
    }

    public Task DeleteAsync(PlanTemplate entity, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task UpdateAsync(PlanTemplate entity, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public async Task<PlanTemplateRequestUnit> AddRequestUnitAsync(PlanTemplateRequestUnit data, CancellationToken cancellationToken = default) =>
        (await context.PlanTemplateRequestUnits.AddAsync(data, cancellationToken)).Entity;

    public async Task<PlanTemplateForm> AddFormAsync(PlanTemplateForm data, CancellationToken cancellationToken = default) =>
        (await context.PlanTemplateForms.AddAsync(data, cancellationToken)).Entity;

    public async Task<int> SyncPlanTemplateFormIdAsync(string version, string tenantId, Dictionary<string, long> formIdMap, CancellationToken cancellationToken = default)
    {
        // 查詢指定版本的所有計畫樣板
        List<PlanTemplate> planTemplates = await context.PlanTemplates
            .Include(pt => pt.PlanTemplateForms.Where(x => x.TenantId == tenantId))
            .Where(x => x.Version == version)
            .ToListAsync(cancellationToken);

        if (planTemplates.Count == 0)
        {
            return 0;
        }

        // 更新所有計畫樣板的 FormId
        foreach (PlanTemplate? template in planTemplates)
        {
            // TODO 預計PlanTemplate的FormName會被移除
            if (formIdMap.TryGetValue(template.FormName, out long formId))
            {
                template.FormId = (int)formId;
            }

            // 使用第一個關聯的 PlanTemplateForm 的名稱來查找對應的 FormId
            PlanTemplateForm? firstForm = template.PlanTemplateForms.FirstOrDefault();
            if (firstForm != null)
            {
                firstForm.FormId = (int)formId;
            }
        }

        // 批次更新資料庫
        context.PlanTemplates.UpdateRange(planTemplates);
        return await context.SaveChangesAsync(cancellationToken);
    }
}
