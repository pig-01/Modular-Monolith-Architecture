using System.Text.Json;
using Base.Infrastructure.Toolkits.Converter.Json;

namespace Base.Infrastructure.Toolkits.Extensions;

public static class JsonExtension
{

    /// <summary>
    /// 設定序列化設定
    /// </summary>
    /// <returns></returns>
    public static readonly JsonSerializerOptions Settings = new(JsonSerializerDefaults.General)
    {
        Converters =
        {
            new DateOnlyConverter(),
            new TimeOnlyConverter(),
            IsoDateTimeOffsetConverter.Singleton
        },
    };

}
