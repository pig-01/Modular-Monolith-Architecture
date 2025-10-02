using Scheduler.Domain.AggregateModel.PlanAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Scheduler.Infrastructure.Context.Configurations;

public partial class PlanConfiguration : IEntityTypeConfiguration<Plan>
{
    public void Configure(EntityTypeBuilder<Plan> builder)
    {
        builder.HasKey(e => e.PlanId).HasAnnotation("SqlServer:FillFactor", 80);

        builder.Property(e => e.Archived).HasDefaultValue(false);
        builder.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
        builder.Property(e => e.CreatedUser).HasDefaultValue("''");
        builder.Property(e => e.ModifiedDate).HasDefaultValueSql("(getdate())");
        builder.Property(e => e.ModifiedUser).HasDefaultValue("''");
        builder.Property(e => e.Show).HasDefaultValue(true);

        OnConfigurePartial(builder);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<Plan> entity);
}
