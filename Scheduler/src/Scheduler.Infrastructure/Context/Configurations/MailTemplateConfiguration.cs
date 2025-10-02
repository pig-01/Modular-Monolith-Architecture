using Scheduler.Domain.AggregateModel.MailAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Scheduler.Infrastructure.Context.Configurations;

public partial class MailTemplateConfiguration : IEntityTypeConfiguration<MailTemplate>
{
    public void Configure(EntityTypeBuilder<MailTemplate> builder)
    {
        _ = builder.HasKey(e => e.MailTemplateId).HasAnnotation("SqlServer:FillFactor", 80);

        _ = builder.Property(e => e.MailTemplateId).HasComment("識別欄位");
        _ = builder.Property(e => e.CreatedDate)
            .HasDefaultValueSql("(getdate())")
            .HasComment("建檔日期");
        _ = builder.Property(e => e.CreatedUser)
            .HasDefaultValue("")
            .HasComment("建檔人員");
        _ = builder.Property(e => e.EnUsbody).HasComment("英文內文");
        _ = builder.Property(e => e.EnUssubject).HasComment("英文主旨");
        _ = builder.Property(e => e.FunctionCode).HasComment("功能代號");
        _ = builder.Property(e => e.JaJpbody).HasComment("日文內文");
        _ = builder.Property(e => e.JaJpsubject).HasComment("日文主旨");
        _ = builder.Property(e => e.MailType)
            .HasDefaultValue("")
            .HasComment("mail類別");
        _ = builder.Property(e => e.ModifiedDate)
            .HasDefaultValueSql("(getdate())")
            .HasComment("最後修改日期");
        _ = builder.Property(e => e.ModifiedUser)
            .HasDefaultValue("")
            .HasComment("最後修改人員");
        _ = builder.Property(e => e.TenantId)
            .HasDefaultValue("")
            .HasComment("Tenant代號");
        _ = builder.Property(e => e.ZhChsbody).HasComment("簡體內文");
        _ = builder.Property(e => e.ZhChssubject).HasComment("簡體主旨");
        _ = builder.Property(e => e.ZhChtbody).HasComment("中文內文");
        _ = builder.Property(e => e.ZhChtsubject).HasComment("中文主旨");

        OnConfigurePartial(builder);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<MailTemplate> entity);
}
