using Base.Infrastructure.Interface.TimeZone;

namespace Main.Repository.Handlers;

public class DateTimeOffsetHandler(ITimeZoneService timeZoneService) : TypeHandler<DateTimeOffset>
{
    // 從資料庫讀取時轉換 (DB → 程式)
    public override DateTimeOffset Parse(object value)
    {
        if (value is DateTime dateTime)
        {
            // 假設資料庫存的是 UTC 時間
            DateTimeOffset utcOffset = new(dateTime, TimeSpan.Zero);
            return timeZoneService.ConvertToUserTimeZone(utcOffset);
        }

        if (value is DateTimeOffset dto)
        {
            return timeZoneService.ConvertToUserTimeZone(dto);
        }

        throw new InvalidCastException($"無法將 {value.GetType()} 轉換為 DateTimeOffset");
    }

    // 寫入資料庫時轉換 (程式 → DB)
    public override void SetValue(IDbDataParameter parameter, DateTimeOffset value) =>
        // 轉換為 UTC 時間存入資料庫
        parameter.Value = timeZoneService.ConvertToUtc(value);
}
