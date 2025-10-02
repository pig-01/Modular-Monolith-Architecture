using Base.Domain.Exceptions;
using Base.Infrastructure.Interface.TimeZone;
using Base.Infrastructure.Toolkits.Extensions;

namespace Scheduler.Infrastructure;

// TODO: 將來如果需要從設定檔讀取預設時區，請在建構函式中加入 IConfiguration configuration 參數
public class TimeZoneService(IHttpContextAccessor httpContextAccessor) : ITimeZoneService
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

    public DateTime ConvertToUserTimeZone(DateTime utcDateTime)
    {
        // 確保傳入的時間是 UTC 時間
        if (utcDateTime.Kind != DateTimeKind.Utc)
        {
            // 如果是 Local 時間，先轉換為 UTC
            if (utcDateTime.Kind == DateTimeKind.Local)
                utcDateTime = utcDateTime.ToUniversalTime();
            // 如果是 Unspecified，假設它是 UTC 時間
            else if (utcDateTime.Kind == DateTimeKind.Unspecified)
                utcDateTime = DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);
        }

        return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, UserTimeZone);
    }

    public DateTime ConvertToUtc(DateTime localDateTime)
    {
        // 根據 DateTime.Kind 進行安全轉換
        return localDateTime.Kind switch
        {
            DateTimeKind.Utc => localDateTime, // 已經是 UTC，直接返回
            DateTimeKind.Local => localDateTime.ToUniversalTime(), // 本地時間轉 UTC
            DateTimeKind.Unspecified => ConvertUnspecifiedToUtc(localDateTime), // 未指定時間，使用用戶時區轉換
            _ => throw new ArgumentException($"Unsupported DateTimeKind: {localDateTime.Kind}", nameof(localDateTime))
        };
    }

    /// <summary>
    /// 將未指定種類的 DateTime 轉換為 UTC 時間
    /// </summary>
    private DateTime ConvertUnspecifiedToUtc(DateTime unspecifiedDateTime)
    {
        // 將未指定的時間視為用戶時區的時間
        DateTime specifiedDateTime = DateTime.SpecifyKind(unspecifiedDateTime, DateTimeKind.Unspecified);
        return TimeZoneInfo.ConvertTimeToUtc(specifiedDateTime, UserTimeZone);
    }

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

    private static string? GetConfigurationTimeZoneId()
    {
        // TODO: 將來需要時，請將此方法改為非靜態並使用 configuration 參數
        // 獲取認證模式
        // string authMode = configuration.GetValue<string>("Authentication:Mode") ?? throw new ConfigNullException("認證模式未配置");

        // 根據認證模式選擇正確的配置節點
        // string? timeZoneConfigPath = authMode switch
        // {
        //     "CAS" => "Authentication:CASSetting:DefaultTimeZoneId",
        //     "JWT" => "Authentication:JWTSetting:DefaultTimeZoneId",
        //     _ => "Authentication:CASSetting:DefaultTimeZoneId" // 預設使用CAS配置
        // };

        //return configuration[timeZoneConfigPath];

        return "";
    }
}
