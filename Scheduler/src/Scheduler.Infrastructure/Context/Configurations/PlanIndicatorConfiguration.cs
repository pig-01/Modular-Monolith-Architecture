using Scheduler.Domain.AggregateModel.PlanAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Scheduler.Infrastructure.Context.Configurations;

public partial class PlanIndicatorConfiguration : IEntityTypeConfiguration<PlanIndicator>
{
    public void Configure(EntityTypeBuilder<PlanIndicator> builder)
    {
        builder.HasKey(e => e.Id).HasAnnotation("SqlServer:FillFactor", 80);

        builder.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
        builder.Property(e => e.CreatedUser).HasDefaultValue("");
        builder.Property(e => e.ModifiedDate).HasDefaultValueSql("(getdate())");
        builder.Property(e => e.ModifiedUser).HasDefaultValue("");

        builder.HasOne(d => d.Plan).WithMany(p => p.PlanIndicators)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_PlanIndicator_Plan");

        OnConfigurePartial(builder);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<PlanIndicator> entity);
}
