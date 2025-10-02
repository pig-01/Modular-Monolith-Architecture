using Base.Infrastructure.Interface.TimeZone;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Base.Infrastructure.Toolkits.Converter.EntityFramework;

public class DateTimeOffsetValueConverter(ITimeZoneService timeZoneService) : ValueConverter<DateTimeOffset, DateTimeOffset>(
    // 轉換到資料庫（將本地時間轉換為UTC）
    datetime => timeZoneService.ConvertToUtc(datetime),
    // 從資料庫讀取（將UTC轉換為本地時間）
    datetime => timeZoneService.ConvertToUserTimeZone(datetime))
{
}
