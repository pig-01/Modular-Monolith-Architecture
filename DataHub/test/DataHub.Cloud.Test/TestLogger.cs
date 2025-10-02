using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace DataHub.Cloud.Test;

/// <summary>
/// a fake logger for testing
/// </summary>
/// <typeparam name="T"></typeparam>
public class TestLogger<T>(ITestOutputHelper output) : ILogger<T>, IDisposable
{
    // 這是關鍵，我們實作了Log<TState>，讓呼叫端可以
    // 自由的使用Log<FormattedLogValues>
    // 同時，我們把該呼叫轉呼叫給另一個abstract method。
    public void Log<TState>(LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception exception,
        Func<TState, Exception, string> formatter) => output.WriteLine($"{logLevel}: {formatter(state, exception)}");
    public IDisposable BeginScope<TState>(TState state) => this;
    public void Dispose() { }
    public bool IsEnabled(LogLevel logLevel) => true;
}
