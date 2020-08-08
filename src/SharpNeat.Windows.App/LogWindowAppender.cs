using log4net.Appender;
using log4net.Core;

namespace SharpNeat.Windows.App
{
    /// <summary>
    /// Log4net appender that redirects log messages to a custom logging system.
    /// </summary>
    public class LogWindowAppender : AppenderSkeleton
    {
        /// <summary>
        /// Handle log event.
        /// </summary>
        protected override void Append(LoggingEvent loggingEvent)
        {
            Logger.Log(RenderLoggingEvent(loggingEvent));   
        }
    }
}
