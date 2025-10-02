namespace Main.Infrastructure.Options.System;

public class ActivationSettings
{
    /// <summary>
    /// 預設連結有效天數
    /// </summary>
    public int DefaultExpireDays { get; set; } = 7;

    /// <summary>
    /// 預設連結有效時間
    /// </summary>
    public TimeSpan DefaultExpireTime { get; set; } = TimeSpan.FromHours(24);

    /// <summary>
    /// 預設時區ID
    /// </summary>
    public string DefaultTimeZoneId { get; set; } = "Asia/Taipei";
}