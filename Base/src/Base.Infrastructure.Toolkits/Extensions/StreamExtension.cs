using System.Text.Json;

namespace Base.Infrastructure.Toolkits.Extensions;

public static class StreamExtension
{
    public static async ValueTask<TValue?> FromJsonAsync<TValue>(this Stream utf8Json, CancellationToken cancellationToken = default)
        => await JsonSerializer.DeserializeAsync<TValue>(utf8Json, JsonExtension.Settings, cancellationToken);
}
