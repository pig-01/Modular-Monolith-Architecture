namespace Scheduler.Domain.AggregateModel.QuartzAggregate;

[PrimaryKey("SchedName", "InstanceName")]
[Table("QRTZ_SCHEDULER_STATE")]
public partial class QrtzSchedulerState
{
    [Key]
    [Column("SCHED_NAME")]
    [StringLength(120)]
    public string SchedName { get; set; } = null!;

    [Key]
    [Column("INSTANCE_NAME")]
    [StringLength(200)]
    public string InstanceName { get; set; } = null!;

    [Column("LAST_CHECKIN_TIME")]
    public long LastCheckinTime { get; set; }

    [Column("CHECKIN_INTERVAL")]
    public long CheckinInterval { get; set; }
}