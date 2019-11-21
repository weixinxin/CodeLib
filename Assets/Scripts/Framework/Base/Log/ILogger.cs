using System;
namespace Framework
{
    public interface ILogger
    {
        void Log(object message);

        void LogWarning(object message);

        void LogError(object message);

        void LogException(Exception exception);


        void LogFormat(string format, params object[] args);

        void LogWarningFormat(string format, params object[] args);

        void LogErrorFormat(string format, params object[] args);
    }
}
