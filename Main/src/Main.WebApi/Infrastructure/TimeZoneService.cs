using Base.Domain.Exceptions;
using Base.Infrastructure.Interface.TimeZone;
using Base.Infrastructure.Toolkits.Extensions;

namespace Main.WebApi.Infrastructure;

public class TimeZoneService(IHttpContextAccessor httpContextAccessor, IConfiguration configuration) : ITimeZoneService
{
    // 默認時區，從配置獲取
    public TimeZoneInfo UserTimeZone
    {
        get
        {
            string? timeZoneId = httpContextAccessor.HttpContext?.User.Claims.GetUserTimeZoneInfo()
                ?? GetConfigurationTimeZoneId(); // 預設時區
            return string.IsNullOrEmpty(timeZoneId)
                ? TimeZoneInfo.Local
                : TimeZoneInfo.FindSystemTimeZoneById(timeZoneId)
                   ?? throw new ConfigNullException($"未找到時區 ID: {timeZoneId}");
        }
    }

    public DateTime Now => ConvertToUserTimeZone(DateTime.UtcNow);

    public DateTime ConvertToUserTimeZone(DateTime utcDateTime) => TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, UserTimeZone);

    public DateTime ConvertToUtc(DateTime localDateTime) => TimeZoneInfo.ConvertTimeToUtc(localDateTime, UserTimeZone);

    public DateTimeOffset ConvertToUserTimeZone(DateTimeOffset utcDateTimeOffset)
    {
        // DateTimeOffset 已包含時區信息，所以需要先轉到UTC，再轉到目標時區
        if (utcDateTimeOffset.Offset != TimeSpan.Zero)
            utcDateTimeOffset = utcDateTimeOffset.ToUniversalTime();

        return TimeZoneInfo.ConvertTime(utcDateTimeOffset, UserTimeZone);
    }

    public DateTimeOffset ConvertToUtc(DateTimeOffset localDateTimeOffset)
    {
        // 如果已經是UTC，則直接返回
        if (localDateTimeOffset.Offset == TimeSpan.Zero)
            return localDateTimeOffset;

        // 將日期時間從使用者時區轉換到UTC
        return TimeZoneInfo.ConvertTime(localDateTimeOffset, TimeZoneInfo.Utc);
    }

    private string? GetConfigurationTimeZoneId()
    {
        // 獲取認證模式
        string authMode = configuration.GetValue<string>("Authentication:Mode") ?? throw new ConfigNullException("認證模式未配置");

        // 根據認證模式選擇正確的配置節點
        string? timeZoneConfigPath = authMode switch
        {
            "CAS" => "Authentication:CASSetting:DefaultTimeZoneId",
            "JWT" => "Authentication:JWTSetting:DefaultTimeZoneId",
            _ => "Authentication:CASSetting:DefaultTimeZoneId" // 預設使用CAS配置
        };

        return configuration[timeZoneConfigPath];
    }
}
