namespace Scheduler.Domain.AggregateModel.QuartzAggregate;

[PrimaryKey("SchedName", "CalendarName")]
[Table("QRTZ_CALENDARS")]
public partial class QrtzCalendar
{
    [Key]
    [Column("SCHED_NAME")]
    [StringLength(120)]
    public string SchedName { get; set; } = null!;

    [Key]
    [Column("CALENDAR_NAME")]
    [StringLength(200)]
    public string CalendarName { get; set; } = null!;

    [Column("CALENDAR")]
    public byte[] Calendar { get; set; } = null!;
}