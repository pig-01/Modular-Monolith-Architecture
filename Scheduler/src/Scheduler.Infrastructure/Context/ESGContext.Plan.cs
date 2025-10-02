using Scheduler.Domain.AggregateModel.PlanAggregate;

namespace Scheduler.Infrastructure.Context;

public partial class DemoContext
{
    public virtual DbSet<Plan> Plans { get; set; }

    public virtual DbSet<PlanDetail> PlanDetails { get; set; }

    public virtual DbSet<PlanDocument> PlanDocuments { get; set; }

    public virtual DbSet<PlanFactory> PlanFactories { get; set; }

    public virtual DbSet<PlanIndicator> PlanIndicators { get; set; }

    public virtual DbSet<PlanIndustry> PlanIndustries { get; set; }

    protected static void OnPlanModelCreatingPartial(ModelBuilder modelBuilder)
    {
        _ = modelBuilder.ApplyConfiguration(new Configurations.PlanConfiguration());
        _ = modelBuilder.ApplyConfiguration(new Configurations.PlanDetailConfiguration());
        _ = modelBuilder.ApplyConfiguration(new Configurations.PlanDocumentConfiguration());
        _ = modelBuilder.ApplyConfiguration(new Configurations.PlanFactoryConfiguration());
        _ = modelBuilder.ApplyConfiguration(new Configurations.PlanIndicatorConfiguration());
        _ = modelBuilder.ApplyConfiguration(new Configurations.PlanIndustryConfiguration());
    }
}
