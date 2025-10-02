namespace Scheduler.Domain.AggregateModel.QuartzAggregate;

[PrimaryKey("SchedName", "TriggerName", "TriggerGroup")]
[Table("QRTZ_CRON_TRIGGERS")]
public partial class QrtzCronTrigger
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

    [Column("CRON_EXPRESSION")]
    [StringLength(120)]
    public string CronExpression { get; set; } = null!;

    [Column("TIME_ZONE_ID")]
    [StringLength(80)]
    public string? TimeZoneId { get; set; }
}