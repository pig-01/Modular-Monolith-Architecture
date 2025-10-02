namespace Scheduler.Domain.AggregateModel.QuartzAggregate;

[PrimaryKey("SchedName", "LockName")]
[Table("QRTZ_LOCKS")]
public partial class QrtzLock
{
    [Key]
    [Column("SCHED_NAME")]
    [StringLength(120)]
    public string SchedName { get; set; } = null!;

    [Key]
    [Column("LOCK_NAME")]
    [StringLength(40)]
    public string LockName { get; set; } = null!;
}