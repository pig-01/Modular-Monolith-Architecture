using Scheduler.Domain.AggregateModel.MailAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Scheduler.Infrastructure.Context.Configurations;

public partial class MailServiceParameterConfiguration : IEntityTypeConfiguration<MailServiceParameter>
{
    public void Configure(EntityTypeBuilder<MailServiceParameter> builder)
    {
        _ = builder.HasKey(e => e.MailServiceParameterId).HasAnnotation("SqlServer:FillFactor", 100);

        _ = builder.Property(e => e.MailServiceParameterId).HasComment("識別欄位");
        _ = builder.Property(e => e.Account).HasComment("帳號");
        _ = builder.Property(e => e.CreatedDate)
            .HasDefaultValueSql("(getdate())")
            .HasComment("建檔日期");
        _ = builder.Property(e => e.CreatedUser)
            .HasDefaultValue("")
            .HasComment("建檔人員");
        _ = builder.Property(e => e.Domain).HasComment("網域");
        _ = builder.Property(e => e.EnableSsl).HasDefaultValue(false);
        _ = builder.Property(e => e.ModifiedDate)
            .HasDefaultValueSql("(getdate())")
            .HasComment("最後修改日期");
        _ = builder.Property(e => e.ModifiedUser)
            .HasDefaultValue("")
            .HasComment("最後修改人員");
        _ = builder.Property(e => e.Password).HasComment("密碼");
        _ = builder.Property(e => e.ServiceType).HasComment("服務類型(SystemCode CodeType='MailServiceType')");
        _ = builder.Property(e => e.TenantId)
            .HasDefaultValue("")
            .HasComment("Tenant代號");

        OnConfigurePartial(builder);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<MailServiceParameter> entity);
}
