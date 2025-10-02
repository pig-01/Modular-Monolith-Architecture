using Base.Infrastructure.Interface.TimeZone;

namespace Main.Repository.Handlers;

public class NullableDateTimeHandler(ITimeZoneService timeZoneService) : TypeHandler<DateTime?>
{
    public override void SetValue(IDbDataParameter parameter, DateTime? value) =>
        // 設定參數值時，將 DateTime 轉換為資料庫可接受的格式
        // 轉換為 UTC 時間存入資料庫
        parameter.Value = value.HasValue ? timeZoneService.ConvertToUtc(value.Value) : value;

    public override DateTime? Parse(object value)
    {
        if (value is DateTime dateTime)
        {
            // 假設資料庫存的是 UTC 時間
            return dateTime.Kind switch
            {
                DateTimeKind.Utc => timeZoneService.ConvertToUserTimeZone(dateTime),
                DateTimeKind.Local => timeZoneService.ConvertToUserTimeZone(dateTime.ToUniversalTime()),
                DateTimeKind.Unspecified => timeZoneService.ConvertToUserTimeZone(DateTime.SpecifyKind(dateTime, DateTimeKind.Utc)),
                _ => throw new ArgumentOutOfRangeException(nameof(value), "不支援的 DateTime Kind")
            };
        }

        if (value is DateTimeOffset dto)
        {
            // 如果是 DateTimeOffset，直接轉換
            return timeZoneService.ConvertToUserTimeZone(dto).DateTime;
        }

        throw new InvalidCastException($"無法將 {value.GetType()} 轉換為 DateTimeOffset");
    }
}

