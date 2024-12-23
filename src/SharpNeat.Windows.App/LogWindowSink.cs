using System.Globalization;
using Serilog.Core;
using Serilog.Events;

namespace SharpNeat.Windows.App;

internal sealed class LogWindowSink : ILogEventSink
{
    public void Emit(LogEvent logEvent)
    {
        string timestamp = logEvent.Timestamp.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture);
        UiLogger.Log($"{timestamp} {logEvent.RenderMessage(CultureInfo.InvariantCulture)}");
    }
}
