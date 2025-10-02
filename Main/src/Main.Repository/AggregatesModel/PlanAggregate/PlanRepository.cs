using Base.Domain.Exceptions;
using Main.Domain.AggregatesModel.PlanAggregate;

namespace Main.Repository.AggregatesModel.PlanAggregate;

public class PlanRepository(DemoContext context) : IPlanRepository
{
    public async Task<Plan> AddAsync(Plan entity, CancellationToken cancellationToken = default)
    {
        // 如果 PlanTemplateVersion 為 null，則使用目前最大的   版本號
        entity.PlanTemplateVersion = string.IsNullOrEmpty(entity.PlanTemplateVersion?.Trim()) ?
            await context.PlanTemplates.MaxAsync(pt => pt.Version, cancellationToken) :
            entity.PlanTemplateVersion;

        // 新增計畫
        Plan plan = (await context.Plans.AddAsync(entity, cancellationToken)).Entity;
        _ = await context.SaveChangesAsync(cancellationToken);
        return plan;
    }

    public Task DeleteAsync(Plan entity, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public async Task UpdateAsync(Plan entity, CancellationToken cancellationToken = default)
    {
        context.Entry(entity).CurrentValues.SetValues(entity);
        await Task.CompletedTask;
    }

    public async Task<PlanDetail> AddPlanDetailAsync(PlanDetail planDetail, CancellationToken cancellationToken = default) =>
        (await context.PlanDetails.AddAsync(planDetail, cancellationToken)).Entity;

    public async Task UpdatePlanDocumentAsync(PlanDocument planDocument, DateTime modifiedDate, CancellationToken cancellationToken = default)
    {
        var test = context.PlanDocuments.Update(planDocument);

        PlanDetail planDetail = await context.PlanDetails
            .Include(pd => pd.Plan)
            .Where(pd => pd.PlanDetailId == planDocument.PlanDetailId)
            .FirstOrDefaultAsync(cancellationToken) ??
            throw new NotFoundException($"PlanDetail not found for PlanDocument's PlanDetail ID {planDocument.PlanDetailId}.");

        Plan plan = planDetail.Plan ?? throw new NotFoundException($"Plan not found for PlanDetail ID {planDetail.PlanDetailId}.");
        plan.ModifiedDate = modifiedDate;
        _ = await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdatePlanDetailAsync(PlanDetail planDetail, DateTime modifiedDate, CancellationToken cancellationToken = default)
    {
        context.Entry(planDetail).CurrentValues.SetValues(planDetail);
        Plan plan = await context.Plans
            .FirstOrDefaultAsync(x => x.PlanId == planDetail.PlanId, cancellationToken) ??
            throw new NotFoundException($"Plan with ID {planDetail.PlanId} not found.");
        plan.ModifiedDate = modifiedDate;
        context.Entry(plan).State = EntityState.Modified;
        await Task.CompletedTask;
    }

    public async Task ArchivePlanAsync(int planId, DateTime archiveDate, string archiveUser, CancellationToken cancellationToken = default)
    {
        Plan plan = await context.Plans.FindAsync([planId], cancellationToken) ??
            throw new NotFoundException($"Plan with ID {planId} not found.");
        plan.Archive(archiveDate, archiveUser);
        context.Entry(plan).CurrentValues.SetValues(plan);

        // 封存計畫表單資料
        await context.PlanDocumentData
            .Include(x => x.PlanDocument)
            .ThenInclude(x => x.PlanDetail)
            .Where(x => x.PlanDocument.PlanDetail!.PlanId == planId)
            .ForEachAsync(x =>
            {
                x.Archive(archiveDate, archiveUser);
                context.Entry(x).CurrentValues.SetValues(x);
            }, cancellationToken);

        // 封存計畫切分資料，沒資料不會處理
        await context.PlanDocumentDataSpliteds
            .Include(x => x.PlanDocument)
            .ThenInclude(x => x.PlanDetail)
            .Where(x => x.PlanDocument.PlanDetail!.PlanId == planId)
            .ForEachAsync(x =>
            {
                x.Archive(archiveDate, archiveUser);
                context.Entry(x).CurrentValues.SetValues(x);
            }, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<PlanDocument> AddPlanDocumentAsync(PlanDocument model, CancellationToken cancellationToken = default)
    {
        // 新增
        PlanDocument planDocument = (await context.PlanDocuments.AddAsync(model, cancellationToken)).Entity;
        _ = await context.SaveChangesAsync(cancellationToken);
        return planDocument;
    }

    public async Task ArchivePlanDocumentAsync(int planDocumentId, DateTime archiveDate, string archiveUser, CancellationToken cancellationToken = default)
    {
        PlanDocument planDocument = await context.PlanDocuments
            .Include(x => x.PlanDocumentDatas)
            .Include(x => x.PlanDocumentDataSpliteds)
            .FirstOrDefaultAsync(x => x.PlanDocumentId == planDocumentId, cancellationToken) ??
            throw new NotFoundException($"PlanDocument with ID {planDocumentId} not found.");
        await context.PlanDocumentLegacies.AddAsync(planDocument.Archive(archiveDate, archiveUser), cancellationToken);
        context.PlanDocumentData.RemoveRange(planDocument.PlanDocumentDatas);
        context.PlanDocumentDataSpliteds.RemoveRange(planDocument.PlanDocumentDataSpliteds);
        context.PlanDocuments.Remove(planDocument);
        _ = await context.SaveChangesAsync(cancellationToken);
    }


    public async Task AssignPlanDocumentAsync(int planId, string responsibleUser, string modifiedUser, IEnumerable<PlanDocument> planDocuments, CancellationToken cancellationToken = default)
    {
        // 查出計畫和計畫明細和計畫表單
        Plan plan = await context.Plans
            .Include(x => x.PlanDetails)
            .ThenInclude(x => x.PlanDocuments)
            .FirstOrDefaultAsync(x => x.PlanId == planId, cancellationToken) ??
            throw new NotFoundException($"Plan with ID {planId} not found.");

        // 透過 Detail ID 和 週期 查出是否有對應的表單建立
        // 如果有則更新表單的負責人和修改人
        // 如果沒有則新增表單
        foreach (PlanDocument planDocument in planDocuments)
        {
            PlanDocument? existingDocument = planDocument switch
            {
                // 執行 month 的查詢
                { IsSingleMonth: true } => plan.PlanDetails.First(x => x.PlanDetailId == planDocument.PlanDetailId).PlanDocuments
                                                           .FirstOrDefault(x => x.PlanDetailId == planDocument.PlanDetailId && x.Month == planDocument.Month),

                // 執行 quarter 的查詢
                { IsSingleQuarter: true } => plan.PlanDetails.First(x => x.PlanDetailId == planDocument.PlanDetailId).PlanDocuments
                                                           .FirstOrDefault(x => x.PlanDetailId == planDocument.PlanDetailId && x.Quarter == planDocument.Quarter),

                // 執行 year 的查詢
                { IsSingleYear: true } => plan.PlanDetails.First(x => x.PlanDetailId == planDocument.PlanDetailId).PlanDocuments
                                                           .FirstOrDefault(x => x.PlanDetailId == planDocument.PlanDetailId && x.Year == planDocument.Year),

                _ => throw new ParameterException("Invalid AssignPlanDetail")
            };

            if (existingDocument is null)
            {
                planDocument.Assign(responsibleUser, modifiedUser);
                // 新增新的表單
                plan.PlanDetails.FirstOrDefault(x => x.PlanDetailId == planDocument.PlanDetailId)?.PlanDocuments.Add(planDocument);
            }
            else
            {
                existingDocument.Assign(responsibleUser, modifiedUser);
                // 更新已存在的表單
                plan.ModifiedDate = DateTime.UtcNow;
                context.Entry(existingDocument).CurrentValues.SetValues(existingDocument);
            }
        }
    }

    public async Task HidePlanAsync(int planId, DateTime hideDate, string hideUser, CancellationToken cancellationToken = default)
    {
        Plan plan = await context.Plans.FirstOrDefaultAsync(x => x.PlanId == planId, cancellationToken) ??
            throw new NotFoundException($"Plan with ID {planId} not found.");
        plan.HideToggle(plan.Show, hideDate, hideUser);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task HideMultiplePlansAsync(IEnumerable<int> planIds, DateTime hideDate, string hideUser, CancellationToken cancellationToken = default)
    {
        List<Plan> plansToHide = await context.Plans
            .Where(p => planIds.Contains(p.PlanId) && p.Show)
            .ToListAsync(cancellationToken);

        foreach (Plan plan in plansToHide)
        {
            plan.HideToggle(plan.Show, hideDate, hideUser);
            context.Entry(plan).CurrentValues.SetValues(plan);
        }
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task ConnectNetZeroToPlanDetailAsync(int apiConnectionId, int netZeroReportId, string netzeroReportName, string modifiedUser, DateTime modifiedDate, int planDetailId, CancellationToken cancellationToken = default)
    {
        // 查出計畫明細
        PlanDetail planDetail = await context.PlanDetails.FirstOrDefaultAsync(x => x.PlanDetailId == planDetailId, cancellationToken) ??
            throw new NotFoundException($"PlanDetail with ID {planDetailId} not found.");

        planDetail.ConnectNetZero(apiConnectionId, netZeroReportId, netzeroReportName, modifiedUser, modifiedDate);
        context.Entry(planDetail).CurrentValues.SetValues(planDetail);
    }

    public async Task CancelConnectNetZeroToPlanDetailAsync(string modifiedUser, DateTime modifiedDate, int planDetailId, CancellationToken cancellationToken = default)
    {
        // 查出計畫明細
        PlanDetail planDetail = await context.PlanDetails.FirstOrDefaultAsync(x => x.PlanDetailId == planDetailId, cancellationToken) ??
            throw new NotFoundException($"PlanDetail with ID {planDetailId} not found.");

        planDetail.CancelConnectNetZero(modifiedUser, modifiedDate);
        context.Entry(planDetail).CurrentValues.SetValues(planDetail);
    }

    public async Task HidePlanDetailHintAsync(string modifiedUser, DateTime modifiedDate, int planDetailId, CancellationToken cancellationToken = default)
    {
        // 查出計畫明細
        PlanDetail planDetail = await context.PlanDetails.FirstOrDefaultAsync(x => x.PlanDetailId == planDetailId, cancellationToken) ??
            throw new NotFoundException($"PlanDetail with ID {planDetailId} not found.");

        planDetail.HideHint(modifiedUser, modifiedDate);
        context.Entry(planDetail).CurrentValues.SetValues(planDetail);
    }


    public async Task RemovePlanDocumentDataByPlanDocumentIdAsync(int planDocumentId, CancellationToken cancellationToken = default)
    {
        List<PlanDocumentData> dataRes = await context.PlanDocumentData.Where(x => x.PlanDocumentId == planDocumentId).ToListAsync(cancellationToken);
        List<PlanDocumentDataSplited> splitedDataRef = await context.PlanDocumentDataSpliteds.Where(x => x.PlanDocumentId == planDocumentId).ToListAsync(cancellationToken);

        context.PlanDocumentData.RemoveRange(dataRes);
        context.PlanDocumentDataSpliteds.RemoveRange(splitedDataRef);
    }

    public async Task CreatePlanDocumentDataAsync(int documentId, List<PlanDocumentData> planDocumentDatas, string createUser, CancellationToken cancellationToken = default)
    {
        PlanDocument planDocument = await context.PlanDocuments
            .Include(x => x.PlanDocumentDatas)
            .Include(x => x.PlanDocumentDataSpliteds)
            .Include(x => x.PlanDetail)
            .ThenInclude(x => x!.Plan)
            .FirstOrDefaultAsync(x => x.DocumentId == documentId, cancellationToken) ??
            throw new NotFoundException($"PlanDocument with ID {documentId} not found.");

        planDocument.ResetData(createUser);
        planDocument.CreateData(planDocumentDatas, createUser, createUser);

        _ = await context.SaveChangesAsync(cancellationToken);
    }


    public async Task SetPlanDetailApiConnectionFailedAsync(
    int planDetailId,
    bool v,
    CancellationToken cancellationToken = default)
    {
        await context.PlanDetails
            .Where(p => p.PlanDetailId == planDetailId)
            .ExecuteUpdateAsync(s => s
                .SetProperty(p => p.ApiConnectionFailed, _ => v),
                cancellationToken);
    }


}


