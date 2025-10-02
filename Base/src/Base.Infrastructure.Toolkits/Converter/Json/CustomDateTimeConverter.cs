using System.Text.Json;
using System.Text.Json.Serialization;

namespace Base.Infrastructure.Toolkits.Converter.Json;

public class CustomDateTimeConverter : JsonConverter<DateTime>
{
    private readonly string[] _formats = { "yyyy-MM-dd", "yyyy-MM-ddTHH:mm:ss", "yyyy-MM-ddTHH:mm:ss.fffZ", "MM/dd/yyyy", "yyyyMMdd", "yyyy/MM/dd", "yyyy/MM/dd HH:mm:ss" };

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? stringValue = reader.GetString();
        return DateTime.ParseExact(stringValue, _formats, null);
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options) => writer.WriteStringValue(value.ToString("yyyy/MM/dd HH:mm:ss"));
}