using Main.Domain.AggregatesModel.PlanAggregate;

namespace Main.Repository.AggregatesModel.PlanAggregate;

public class PlanDocumentDataRepository(DemoContext context) : IPlanDocumentDataRepository
{

    public async Task<PlanDocumentData> AddAsync(PlanDocumentData model, CancellationToken cancellationToken = default)
    {
        // 新增
        PlanDocumentData planDocumentData = (await context.PlanDocumentData.AddAsync(model, cancellationToken)).Entity;
        _ = await context.SaveChangesAsync(cancellationToken);
        return planDocumentData;
    }

    public async Task AddSplitedDataRangeAsync(PlanDocumentDataSplited[] datas, CancellationToken cancellationToken = default)
    {
        // 新增
        await context.PlanDocumentDataSpliteds.AddRangeAsync(datas, cancellationToken);
        _ = await context.SaveChangesAsync(cancellationToken);

    }

    public Task DeleteAsync(PlanDocumentData entity, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task<PlanDocumentData> GetByIdAsync(string id, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task<List<PlanDocumentData>> ListAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task<List<PlanDocumentData>> ListAsync(Expression<Func<PlanDocumentData, bool>> predicate, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task UpdateAsync(PlanDocumentData entity, CancellationToken cancellationToken = default) => throw new NotImplementedException();
}
