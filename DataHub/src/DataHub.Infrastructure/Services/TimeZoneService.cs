using Base.Domain.Exceptions;
using Base.Infrastructure.Interface.TimeZone;
using Base.Infrastructure.Toolkits.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace DataHub.Infrastructure.Services;

public class TimeZoneService(IHttpContextAccessor httpContextAccessor, IConfiguration configuration) : ITimeZoneService
{
    // 默認時區，從配置獲取
    public TimeZoneInfo UserTimeZone => TimeZoneInfo.FindSystemTimeZoneById(
            httpContextAccessor.HttpContext?.User.Claims.GetUserTimeZoneInfo()
            ?? GetConfigurationTimeZoneId()
            ?? "Asia/Taipei");

    public DateTime Now => TimeZoneInfo.ConvertTime(DateTime.UtcNow, UserTimeZone);

    public DateTime ConvertToUserTimeZone(DateTime utcDateTime) => TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, UserTimeZone);

    public DateTime ConvertToUtc(DateTime localDateTime) => TimeZoneInfo.ConvertTimeToUtc(localDateTime, UserTimeZone);

    public DateTimeOffset ConvertToUserTimeZone(DateTimeOffset utcDateTimeOffset)
    {
        // DateTimeOffset 已包含時區信息，所以需要先轉到UTC，再轉到目標時區
        if (utcDateTimeOffset.Offset != TimeSpan.Zero)
        {
            utcDateTimeOffset = utcDateTimeOffset.ToUniversalTime();
        }

        return TimeZoneInfo.ConvertTime(utcDateTimeOffset, UserTimeZone);
    }

    public DateTimeOffset ConvertToUtc(DateTimeOffset localDateTimeOffset)
    {
        // 如果已經是UTC，則直接返回
        if (localDateTimeOffset.Offset == TimeSpan.Zero)
        {
            return localDateTimeOffset;
        }

        // 將日期時間從使用者時區轉換到UTC
        return TimeZoneInfo.ConvertTime(localDateTimeOffset, TimeZoneInfo.Utc);
    }

    private string? GetConfigurationTimeZoneId() =>
        configuration.GetValue<string>("TimeZone:Default") ?? throw new ConfigNullException("認證模式未配置");
}
