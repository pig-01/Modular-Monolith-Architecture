namespace Scheduler.Domain.AggregateModel.QuartzAggregate;

[PrimaryKey("SchedName", "TriggerName", "TriggerGroup")]
[Table("QRTZ_SIMPLE_TRIGGERS")]
public partial class QrtzSimpleTrigger
{
    [Key]
    [Column("SCHED_NAME")]
    [StringLength(120)]
    public string SchedName { get; set; } = null!;

    [Key]
    [Column("TRIGGER_NAME")]
    [StringLength(150)]
    public string TriggerName { get; set; } = null!;

    [Key]
    [Column("TRIGGER_GROUP")]
    [StringLength(150)]
    public string TriggerGroup { get; set; } = null!;

    [Column("REPEAT_COUNT")]
    public int RepeatCount { get; set; }

    [Column("REPEAT_INTERVAL")]
    public long RepeatInterval { get; set; }

    [Column("TIMES_TRIGGERED")]
    public int TimesTriggered { get; set; }
}