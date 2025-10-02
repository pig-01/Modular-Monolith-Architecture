namespace Base.Infrastructure.Interface.TimeZone;

public interface ITimeZoneService
{
    // DateTime 方法
    DateTime Now { get; }
    DateTime ConvertToUserTimeZone(DateTime utcDateTime);
    DateTime ConvertToUtc(DateTime localDateTime);

    // DateTimeOffset 方法
    DateTimeOffset ConvertToUserTimeZone(DateTimeOffset utcDateTimeOffset);
    DateTimeOffset ConvertToUtc(DateTimeOffset localDateTimeOffset);

    // 取得使用者時區
    TimeZoneInfo UserTimeZone { get; }
}
