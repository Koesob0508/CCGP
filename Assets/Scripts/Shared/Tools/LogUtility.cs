using System;

namespace CCGP.Shared
{
    /// <summary>
    /// Static logger class providing logging functionality with customizable colors and context.
    /// </summary>
    public static class LogUtility
    {
        // The current logger instance, defaults to DefaultLogger.
        private static ILogger _logger = new DefaultLogger();

        /// <summary>
        /// Allows setting a custom logger implementation.
        /// </summary>
        /// <param name="customLogger">The custom ILogger instance to use.</param>
        public static void SetLogger(ILogger customLogger)
        {
            _logger = customLogger;
        }

        public static void Log(string log, string colorName = null)
        {
            LogInternal(log, colorName, _logger.Log);
        }
        public static void LogWarning(string log, string colorName = null)
        {
            LogInternal(log, colorName, _logger.LogWarning);
        }
        public static void LogError(string log, string colorName = null)
        {
            LogInternal(log, colorName, _logger.LogError);
        }

        private static void LogInternal(string log, string colorName, Action<string> logAction)
        {
            if(colorName == null)
            {
                logAction(log);
            }
            else
            {
                string formattedLog = $"<color={colorName}><b>{log}</b></color>";
                logAction(log);
            }
        }

        /// <summary>
        /// Internal logging method to format and log messages.
        /// </summary>
        /// <typeparam name="T">The type providing context for the log.</typeparam>
        /// <param name="log">The log message.</param>
        /// <param name="colorName">The color for the log message.</param>
        /// <param name="param">Optional type parameter to include in the log.</param>
        /// <param name="logAction">The logging action to perform (Log, LogWarning, or LogError).</param>
        private static void LogInternal<T>(string log, string colorName, Type param, Action<string> logAction)
        {
            var context = GetTypeName(typeof(T));
            string formattedLog = FormatLogMessage(context, log, colorName, param);
            logAction(formattedLog);
        }

        /// <summary>
        /// Formats a log message with context, color, and optional type information.
        /// </summary>
        /// <param name="context">The context type name.</param>
        /// <param name="log">The log message.</param>
        /// <param name="colorName">The color for the log message.</param>
        /// <param name="param">Optional type parameter to include in the log.</param>
        /// <returns>The formatted log message.</returns>
        private static string FormatLogMessage(string context, string log, string colorName, Type param)
        {
            string formattedLog = $"[{context}] <color={colorName}><b>{log}</b></color>";

            if (param != null)
            {
                formattedLog += $" (Type : {GetTypeName(param)})";
            }

            return formattedLog;
        }

        /// <summary>
        /// Extracts the type name from a Type object.
        /// </summary>
        /// <param name="type">The type to extract the name from.</param>
        /// <returns>The name of the type.</returns>
        private static string GetTypeName(Type type)
        {
            if (type == null) return string.Empty;

            var split = type.ToString().Split('.');
            return split.Length > 0 ? split[^1] : string.Empty;
        }

        /// <summary>
        /// Logs a message with optional color and context.
        /// </summary>
        /// <typeparam name="T">The context type for the log.</typeparam>
        /// <param name="log">The log message.</param>
        /// <param name="colorName">The color for the log message (default is black).</param>
        /// <param name="param">Optional type parameter to include in the log.</param>
        public static void Log<T>(string log, string colorName = "black", Type param = null)
        {
            LogInternal<T>(log, colorName, param, _logger.Log);
        }

        /// <summary>
        /// Logs a warning with optional color and context.
        /// </summary>
        /// <typeparam name="T">The context type for the warning log.</typeparam>
        /// <param name="log">The warning message.</param>
        /// <param name="colorName">The color for the warning message (default is black).</param>
        /// <param name="param">Optional type parameter to include in the warning log.</param>
        public static void LogWarning<T>(string log, string colorName = "black", Type param = null)
        {
            LogInternal<T>(log, colorName, param, _logger.LogWarning);
        }

        /// <summary>
        /// Logs an error with optional color and context.
        /// </summary>
        /// <typeparam name="T">The context type for the error log.</typeparam>
        /// <param name="log">The error message.</param>
        /// <param name="colorName">The color for the error message (default is black).</param>
        /// <param name="param">Optional type parameter to include in the error log.</param>
        public static void LogError<T>(string log, string colorName = "black", Type param = null)
        {
            LogInternal<T>(log, colorName, param, _logger.LogError);
        }
    }
}
