namespace Scheduler.Domain.AggregateModel.QuartzAggregate;

[PrimaryKey("SchedName", "EntryId")]
[Table("QRTZ_FIRED_TRIGGERS")]
[Index("SchedName", "JobGroup", "JobName", Name = "IDX_QRTZ_FT_G_J")]
[Index("SchedName", "TriggerGroup", "TriggerName", Name = "IDX_QRTZ_FT_G_T")]
[Index("SchedName", "InstanceName", "RequestsRecovery", Name = "IDX_QRTZ_FT_INST_JOB_REQ_RCVRY")]
public partial class QrtzFiredTrigger
{
    [Key]
    [Column("SCHED_NAME")]
    [StringLength(120)]
    public string SchedName { get; set; } = null!;

    [Key]
    [Column("ENTRY_ID")]
    [StringLength(140)]
    public string EntryId { get; set; } = null!;

    [Column("TRIGGER_NAME")]
    [StringLength(150)]
    public string TriggerName { get; set; } = null!;

    [Column("TRIGGER_GROUP")]
    [StringLength(150)]
    public string TriggerGroup { get; set; } = null!;

    [Column("INSTANCE_NAME")]
    [StringLength(200)]
    public string InstanceName { get; set; } = null!;

    [Column("FIRED_TIME")]
    public long FiredTime { get; set; }

    [Column("SCHED_TIME")]
    public long SchedTime { get; set; }

    [Column("PRIORITY")]
    public int Priority { get; set; }

    [Column("STATE")]
    [StringLength(16)]
    public string State { get; set; } = null!;

    [Column("JOB_NAME")]
    [StringLength(150)]
    public string? JobName { get; set; }

    [Column("JOB_GROUP")]
    [StringLength(150)]
    public string? JobGroup { get; set; }

    [Column("IS_NONCONCURRENT")]
    public bool? IsNonconcurrent { get; set; }

    [Column("REQUESTS_RECOVERY")]
    public bool? RequestsRecovery { get; set; }
}