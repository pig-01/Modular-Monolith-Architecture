namespace Base.Infrastructure.Toolkits.ValueObjects;

public class DateTimeOffsetValue
{
    public DateTimeOffsetValue(DateTimeOffset dateTimeOffset) => LocalDateTime = dateTimeOffset;

    public DateTimeOffsetValue(DateTime dateTime, TimeZoneInfo timeZone)
    {
        // 將 DateTime 和 TimeZoneInfo 轉換為 DateTimeOffset
        TimeSpan offset = timeZone.GetUtcOffset(dateTime);
        LocalDateTime = new DateTimeOffset(dateTime, offset);
    }

    // 取得 UTC 時間
    public DateTimeOffset UtcDateTime => LocalDateTime.ToUniversalTime();

    // 取得本地時間 (保持原始的 Offset)
    public DateTimeOffset LocalDateTime { get; }

    // 取得特定時區的時間
    public DateTimeOffset InTimeZone(TimeZoneInfo timeZone) => TimeZoneInfo.ConvertTime(LocalDateTime, timeZone);

    // 取得 Offset
    public TimeSpan Offset => LocalDateTime.Offset;

    // 隱式轉換為 DateTimeOffset
    public static implicit operator DateTimeOffset(DateTimeOffsetValue value) => value.LocalDateTime;

    // 顯式轉換為 DateTime (UTC)
    public static explicit operator DateTime(DateTimeOffsetValue value) => value.LocalDateTime.UtcDateTime;
}
