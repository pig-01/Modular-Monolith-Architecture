namespace Scheduler.Domain.AggregateModel.QuartzAggregate;

[PrimaryKey("SchedName", "TriggerGroup")]
[Table("QRTZ_PAUSED_TRIGGER_GRPS")]
public partial class QrtzPausedTriggerGrp
{
    [Key]
    [Column("SCHED_NAME")]
    [StringLength(120)]
    public string SchedName { get; set; } = null!;

    [Key]
    [Column("TRIGGER_GROUP")]
    [StringLength(150)]
    public string TriggerGroup { get; set; } = null!;
}