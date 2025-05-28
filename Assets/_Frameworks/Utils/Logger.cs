using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public static class Logger
{
    [Conditional("ver_DEV")]
    public static void Log(string message)
    {
        UnityEngine.Debug.LogFormat("[{0}] {1}", System.DateTime.Now.ToString("yyyy-mm-dd HH:mm:ss.fff"), message);
    }

    [Conditional("ver_DEV")]
    public static void WarningLog(string message)
    {
        UnityEngine.Debug.LogWarningFormat("[{0}] {1}", System.DateTime.Now.ToString("yyyy-mm-dd HH:mm:ss.fff"), message);
    }

    public static void ErrorLog(string message)
    {
        UnityEngine.Debug.LogErrorFormat("[{0}] {1}", System.DateTime.Now.ToString("yyyy-mm-dd HH:mm:ss.fff"), message);
    }
}
