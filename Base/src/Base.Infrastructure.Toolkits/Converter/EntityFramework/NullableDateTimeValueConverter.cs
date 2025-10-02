using Base.Infrastructure.Interface.TimeZone;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Base.Infrastructure.Toolkits.Converter.EntityFramework;

public class NullableDateTimeValueConverter(ITimeZoneService timeZoneService) : ValueConverter<DateTime?, DateTime?>(
    // 轉換到資料庫（將本地時間轉換為UTC）
    datetime => datetime.HasValue ? timeZoneService.ConvertToUtc(datetime.Value) : datetime,
    // 從資料庫讀取（將UTC轉換為本地時間）
    datetime => datetime.HasValue ? timeZoneService.ConvertToUserTimeZone(datetime.Value) : datetime)
{
}
