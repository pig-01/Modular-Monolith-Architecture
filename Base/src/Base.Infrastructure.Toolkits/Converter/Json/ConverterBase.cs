using System.Text.Json;

namespace Base.Infrastructure.Toolkits.Converter.Json;

public static class ConverterBase
{
    public static readonly JsonSerializerOptions Settings = new(JsonSerializerDefaults.General)
    {
        Converters =
        {
            new DateOnlyConverter(),
            new TimeOnlyConverter(),
            new CustomNullableDateTimeConverter(),
            new CustomDateTimeConverter(),
            IsoDateTimeOffsetConverter.Singleton
        },
    };
}
