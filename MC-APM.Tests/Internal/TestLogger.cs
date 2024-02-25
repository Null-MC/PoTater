using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace NullMC.APM.Tests.Internal;

internal class TestLogger(ITestOutputHelper output) : TestLogger<object>(output);

internal class TestLogger<T>(ITestOutputHelper output) : ILogger<T>
{
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string?> formatter)
    {
        var message = formatter(state, exception);
        output.WriteLine($"LOG: {message}");
    }

    public bool IsEnabled(LogLevel logLevel) => true;

    public IDisposable BeginScope<TState>(TState state) where TState : notnull
    {
        throw new NotImplementedException();
    }
}
