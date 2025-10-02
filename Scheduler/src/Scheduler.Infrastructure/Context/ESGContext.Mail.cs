using Scheduler.Domain.AggregateModel.MailAggregate;

namespace Scheduler.Infrastructure.Context;

public partial class DemoContext
{
    public virtual DbSet<MailQueue> MailQueues { get; set; }

    public virtual DbSet<MailSender> MailSenders { get; set; }

    public virtual DbSet<MailServiceParameter> MailServiceParameters { get; set; }

    public virtual DbSet<MailServiceRelation> MailServiceRelations { get; set; }

    public virtual DbSet<MailTemplate> MailTemplates { get; set; }

    protected static void OnMailModelCreatingPartial(ModelBuilder modelBuilder)
    {
        _ = modelBuilder.ApplyConfiguration(new Configurations.MailQueueConfiguration());
        _ = modelBuilder.ApplyConfiguration(new Configurations.MailServiceParameterConfiguration());
        _ = modelBuilder.ApplyConfiguration(new Configurations.MailServiceRelationConfiguration());
        _ = modelBuilder.ApplyConfiguration(new Configurations.MailTemplateConfiguration());
    }
}
