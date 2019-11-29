using System;

public class Logger : Framework.ILogger
{
    public void Log(object message)
    {
        UnityEngine.Debug.Log(message);
    }

    public void LogError(object message)
    {
        UnityEngine.Debug.LogError(message);
    }

    public void LogErrorFormat(string format, params object[] args)
    {
        UnityEngine.Debug.LogErrorFormat(format, args);
    }

    public void LogException(Exception exception)
    {
        UnityEngine.Debug.LogException(exception);
    }

    public void LogFormat(string format, params object[] args)
    {
        UnityEngine.Debug.LogFormat(format, args);
    }

    public void LogWarning(object message)
    {
        UnityEngine.Debug.LogWarning(message);
    }

    public void LogWarningFormat(string format, params object[] args)
    {
        UnityEngine.Debug.LogWarningFormat(format, args);
    }
}