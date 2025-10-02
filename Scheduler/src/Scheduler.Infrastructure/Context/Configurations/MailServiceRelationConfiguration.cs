using Scheduler.Domain.AggregateModel.MailAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Scheduler.Infrastructure.Context.Configurations;

public partial class MailServiceRelationConfiguration : IEntityTypeConfiguration<MailServiceRelation>
{
    public void Configure(EntityTypeBuilder<MailServiceRelation> builder)
    {
        _ = builder.HasKey(e => e.MailServiceRelationId)
            .HasName("PK_MailServiceReleation")
            .HasAnnotation("SqlServer:FillFactor", 100);

        _ = builder.Property(e => e.MailServiceRelationId).HasComment("識別欄位");
        _ = builder.Property(e => e.CreatedDate)
            .HasDefaultValueSql("(getdate())")
            .HasComment("建檔日期");
        _ = builder.Property(e => e.CreatedUser)
            .HasDefaultValue("")
            .HasComment("建檔人員");
        _ = builder.Property(e => e.MailServiceParameterId).HasComment("Mail服務參數識別欄位");
        _ = builder.Property(e => e.ModifiedDate)
            .HasDefaultValueSql("(getdate())")
            .HasComment("最後異動日期");
        _ = builder.Property(e => e.ModifiedUser)
            .HasDefaultValue("")
            .HasComment("最後異動人員");
        _ = builder.Property(e => e.TenantId)
            .HasDefaultValue("")
            .HasComment("Tenant代號");

        _ = builder.HasOne(d => d.MailServiceParameter).WithMany(p => p.MailServiceRelations)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_MailServiceRelation_MailServiceParameter");

        OnConfigurePartial(builder);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<MailServiceRelation> entity);
}
