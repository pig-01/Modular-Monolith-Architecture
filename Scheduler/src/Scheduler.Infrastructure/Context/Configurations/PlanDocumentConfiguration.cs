using Scheduler.Domain.AggregateModel.PlanAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Scheduler.Infrastructure.Context.Configurations;

public partial class PlanDocumentConfiguration : IEntityTypeConfiguration<PlanDocument>
{
    public void Configure(EntityTypeBuilder<PlanDocument> builder)
    {
        builder.HasKey(e => e.PlanDocumentId).HasAnnotation("SqlServer:FillFactor", 80);

        builder.Property(e => e.PlanDocumentId).HasComment("計畫明細表單識別碼");
        builder.Property(e => e.Approve).HasComment("審核人");
        builder.Property(e => e.CreatedDate)
            .HasDefaultValueSql("(getdate())")
            .HasComment("建檔時間");
        builder.Property(e => e.CreatedUser)
            .HasDefaultValue("")
            .HasComment("建檔人員");
        builder.Property(e => e.DocumentId).HasComment("表單識別碼");
        builder.Property(e => e.EndDate).HasComment("結束日期");
        builder.Property(e => e.FormStatus).HasComment("進度代碼");
        builder.Property(e => e.ModifiedDate)
            .HasDefaultValueSql("(getdate())")
            .HasComment("最後修改時間");
        builder.Property(e => e.ModifiedUser)
            .HasDefaultValue("''")
            .HasComment("最後修改人員");
        builder.Property(e => e.Month).HasComment("月份");
        builder.Property(e => e.PlanDetailId).HasComment("計畫明細識別碼");
        builder.Property(e => e.PlanFactoryId).HasComment("區域對應欄位");
        builder.Property(e => e.Quarter).HasComment("季度");
        builder.Property(e => e.Responsible).HasComment("負責人");
        builder.Property(e => e.StartDate).HasComment("開始日期");
        builder.Property(e => e.Year).HasComment("年度");

        builder.HasOne(d => d.PlanDetail).WithMany(p => p.PlanDocuments)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_PlanDocument_PlanDetail");

        OnConfigurePartial(builder);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<PlanDocument> entity);
}
