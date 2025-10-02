using Base.Infrastructure.Toolkits.Resources;

namespace Base.Infrastructure.Toolkits.Extensions;

/// <summary>
/// DateTime 擴充方法
/// </summary>
public static class DateTimeExtension
{
    /// <summary>
    /// 轉換成使用者時區的時間
    /// </summary>
    /// <param name="dateTime">時間 Utc</param>
    /// <param name="timeZoneInfo">時區</param>
    /// <returns>轉換後時間</returns>
    public static DateTime ToSpecifiedDateTime(this DateTime dateTime, TimeZoneInfo timeZoneInfo) => TimeZoneInfo.ConvertTimeFromUtc(dateTime, timeZoneInfo);

    /// <summary>
    /// 轉換成 UTC 時間
    /// </summary>
    /// <param name="dateTime">時間 <paramref name="timeZoneInfo"/></param>
    /// <param name="timeZoneInfo">時區</param>
    /// <returns>轉換後時間</returns>
    public static DateTime ToUtcDateTime(this DateTime dateTime, TimeZoneInfo timeZoneInfo) => TimeZoneInfo.ConvertTimeToUtc(dateTime, timeZoneInfo);


    public static string ToSqlDateTime(this DateTime dateTime) => dateTime.ToString(SystemResource.DateTimeFormat);

    public static string ToSqlDate(this DateTime dateTime) => dateTime.ToString(SystemResource.DateFormat);

    public static string ToSqlTime(this DateTime dateTime) => dateTime.ToString(SystemResource.TimeFormat);

}