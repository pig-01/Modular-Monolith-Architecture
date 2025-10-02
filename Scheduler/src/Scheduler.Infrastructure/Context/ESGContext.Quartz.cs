using Scheduler.Domain.AggregateModel.QuartzAggregate;

namespace Scheduler.Infrastructure.Context;

public partial class DemoContext
{
    public virtual DbSet<QrtzBlobTrigger> QrtzBlobTriggers { get; set; }

    public virtual DbSet<QrtzCalendar> QrtzCalendars { get; set; }

    public virtual DbSet<QrtzCronTrigger> QrtzCronTriggers { get; set; }

    public virtual DbSet<QrtzFiredTrigger> QrtzFiredTriggers { get; set; }

    public virtual DbSet<QrtzJobDetail> QrtzJobDetails { get; set; }

    public virtual DbSet<QrtzLock> QrtzLocks { get; set; }

    public virtual DbSet<QrtzPausedTriggerGrp> QrtzPausedTriggerGrps { get; set; }

    public virtual DbSet<QrtzSchedulerState> QrtzSchedulerStates { get; set; }

    public virtual DbSet<QrtzSimpleTrigger> QrtzSimpleTriggers { get; set; }

    public virtual DbSet<QrtzSimpropTrigger> QrtzSimpropTriggers { get; set; }

    protected static void OnQuartzModelCreatingPartial(ModelBuilder modelBuilder) { }
}
