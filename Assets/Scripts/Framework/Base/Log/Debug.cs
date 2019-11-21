using System;
namespace Framework
{
    internal static class Debug
    {
        private static ILogger s_Logger = null;
        internal static void SetLogger(ILogger logger)
        {
            s_Logger = logger;
        }

        internal static void Log(object message)
        {
            if (s_Logger != null)
                s_Logger.Log(message);
        }

        internal static void LogWarning(object message)
        {
            if (s_Logger != null)
                s_Logger.LogWarning(message);
        }
        internal static void LogError(object message)
        {
            if (s_Logger != null)
                s_Logger.LogError(message);
        }

        internal static void LogException(Exception exception)
        {
            if (s_Logger != null)
                s_Logger.LogException(exception);
        }

        internal static void LogFormat(string format, params object[] args)
        {
            if (s_Logger != null)
                s_Logger.LogFormat(format, args);
        }

        internal static void LogWarningFormat(string format, params object[] args)
        {
            if (s_Logger != null)
                s_Logger.LogWarningFormat(format, args);
        }

        internal static void LogErrorFormat(string format, params object[] args)
        {
            if (s_Logger != null)
                s_Logger.LogErrorFormat(format, args);
        }
    }
}
