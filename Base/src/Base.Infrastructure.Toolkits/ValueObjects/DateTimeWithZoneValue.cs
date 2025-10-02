namespace Base.Infrastructure.Toolkits.ValueObjects;

public class DateTimeWithZoneValue(DateTime dateTime, TimeZoneInfo timeZone)
{
    public DateTime UtcDateTime { get; } = TimeZoneInfo.ConvertTimeToUtc(dateTime, timeZone);

    public DateTime LocalDateTime => TimeZoneInfo.ConvertTimeFromUtc(UtcDateTime, TimeZone);

    public TimeZoneInfo TimeZone { get; } = timeZone;
}
