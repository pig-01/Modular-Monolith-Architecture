using Scheduler.Domain.AggregateModel.MailAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Scheduler.Infrastructure.Context.Configurations;

public partial class MailQueueConfiguration : IEntityTypeConfiguration<MailQueue>
{
    public void Configure(EntityTypeBuilder<MailQueue> builder)
    {
        _ = builder.ToTable("MailQueue", tb => tb.HasComment("發信佇列"));

        _ = builder.Property(e => e.Id).HasComment("佇列識別碼");
        _ = builder.Property(e => e.Bcc).HasComment("密件副本");
        _ = builder.Property(e => e.Body).HasComment("本文");
        _ = builder.Property(e => e.Cc).HasComment("副本");
        _ = builder.Property(e => e.CreatedDate)
            .HasDefaultValueSql("(getdate())")
            .HasComment("建立日期");
        _ = builder.Property(e => e.CreatedUser).HasComment("建立人員");
        _ = builder.Property(e => e.Encoding).HasComment("本文編碼");
        _ = builder.Property(e => e.IsBodyHtml).HasComment("本文是否為HTML");
        _ = builder.Property(e => e.ModifiedDate)
            .HasDefaultValueSql("(getdate())")
            .HasComment("修改日期");
        _ = builder.Property(e => e.ModifiedUser).HasComment("修改人員");
        _ = builder.Property(e => e.Recipient).HasComment("收件者");
        _ = builder.Property(e => e.Status)
            .HasDefaultValue("0")
            .IsFixedLength()
            .HasComment("發信狀態");
        _ = builder.Property(e => e.Subject).HasComment("主旨");
        _ = builder.Property(e => e.TenantId).HasComment("站台識別碼");

        OnConfigurePartial(builder);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<MailQueue> entity);
}
