namespace Scheduler.Domain.AggregateModel.QuartzAggregate;

[PrimaryKey("SchedName", "JobName", "JobGroup")]
[Table("QRTZ_JOB_DETAILS")]
public partial class QrtzJobDetail
{
    [Key]
    [Column("SCHED_NAME")]
    [StringLength(120)]
    public string SchedName { get; set; } = null!;

    [Key]
    [Column("JOB_NAME")]
    [StringLength(150)]
    public string JobName { get; set; } = null!;

    [Key]
    [Column("JOB_GROUP")]
    [StringLength(150)]
    public string JobGroup { get; set; } = null!;

    [Column("DESCRIPTION")]
    [StringLength(250)]
    public string? Description { get; set; }

    [Column("JOB_CLASS_NAME")]
    [StringLength(250)]
    public string JobClassName { get; set; } = null!;

    [Column("IS_DURABLE")]
    public bool IsDurable { get; set; }

    [Column("IS_NONCONCURRENT")]
    public bool IsNonconcurrent { get; set; }

    [Column("IS_UPDATE_DATA")]
    public bool IsUpdateData { get; set; }

    [Column("REQUESTS_RECOVERY")]
    public bool RequestsRecovery { get; set; }

    [Column("JOB_DATA")]
    public byte[]? JobData { get; set; }
}