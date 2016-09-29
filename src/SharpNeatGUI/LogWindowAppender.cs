/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2016 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */
using log4net.Appender;
using log4net.Core;

namespace SharpNeatGUI
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
