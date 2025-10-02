using Main.Domain.SeedWork;

namespace Main.Domain.AggregatesModel.PlanAggregate;

public interface IPlanDocumentDataRepository : IRepository<PlanDocumentData>
{
    public Task AddSplitedDataRangeAsync(PlanDocumentDataSplited[] datas, CancellationToken cancellationToken = default);
}
