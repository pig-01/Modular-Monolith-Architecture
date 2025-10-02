using Scheduler.Domain.AggregateModel.PlanAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Scheduler.Infrastructure.Context.Configurations;

public partial class PlanIndustryConfiguration : IEntityTypeConfiguration<PlanIndustry>
{
    public void Configure(EntityTypeBuilder<PlanIndustry> builder)
    {
        builder.HasKey(e => e.Id).HasAnnotation("SqlServer:FillFactor", 80);

        builder.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
        builder.Property(e => e.CreatedUser).HasDefaultValue("");
        builder.Property(e => e.ModifiedDate).HasDefaultValueSql("(getdate())");
        builder.Property(e => e.ModifiedUser).HasDefaultValue("");

        builder.HasOne(d => d.Plan).WithMany(p => p.PlanIndustries)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_PlanIndustry_Plan");

        OnConfigurePartial(builder);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<PlanIndustry> entity);
}
