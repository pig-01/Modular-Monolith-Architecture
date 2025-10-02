using Scheduler.Domain.AggregateModel.PlanAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

#nullable disable

namespace Scheduler.Infrastructure.Context.Configurations;

public partial class PlanDetailConfiguration : IEntityTypeConfiguration<PlanDetail>
{
    public void Configure(EntityTypeBuilder<PlanDetail> builder)
    {
        builder.HasKey(e => e.PlanDetailId).HasAnnotation("SqlServer:FillFactor", 80);

        builder.Property(e => e.AcceptDataSource).HasDefaultValue("Bizform");
        builder.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
        builder.Property(e => e.CreatedUser).HasDefaultValue("");
        builder.Property(e => e.ModifiedDate).HasDefaultValueSql("(getdate())");
        builder.Property(e => e.ModifiedUser).HasDefaultValue("''");
        builder.Property(e => e.Show).HasDefaultValue(true);
        builder.Property(e => e.ShowHint).HasDefaultValue(false);

        builder.HasOne(d => d.Plan).WithMany(p => p.PlanDetails)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_PlanDetail_Plan");

        OnConfigurePartial(builder);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<PlanDetail> entity);
}
