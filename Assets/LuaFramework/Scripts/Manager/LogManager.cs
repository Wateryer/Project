using UnityEngine;
using System.Collections;
using System;

public class LogManager : MonoBehaviour {

    private static LogManager instance;
    public static LogManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new LogManager();
            }
            return instance;
        }
    }
    #region 开发调试使用前面的，发布时启用后面空函数，减少性能消耗

#if false
        public static Action<string> Debug = UnityEngine.Debug.Log;
        public static Action<string> Error = UnityEngine.Debug.LogError;
        public static Action<string> Warning = UnityEngine.Debug.LogWarning;
        //public static Action<string> Log = LuaInterface.Debugger.Log;
        //public static Action<string> LogWarning = LuaInterface.Debugger.LogWarning;
        //public static Action<string> LogError = LuaInterface.Debugger.LogError;
       // public static Action<string> LogException = LuaInterface.Debugger.LogException;
#else
    public static void Debug(string str) { }
    //public static void Log(object message){ }
    //public static void Log(string str) { }
    //public static void Log(string str, object arg0) { }
    //public static void Log(string str, params object[] param) { }
    //public static void Log(string str, object arg0, object arg1) { }
    //public static void Log(string str, object arg0, object arg1, object arg2) { }

    //public static void LogWarning(object message) { }
    //public static void LogWarning(string str) { }
    //public static void LogWarning(string str, object arg0) { }
    //public static void LogWarning(string str, params object[] param) { }
    //public static void LogWarning(string str, object arg0, object arg1) { }
    //public static void LogWarning(string str, object arg0, object arg1, object arg2) { }

    //public static void LogError(object message) { }
    //public static void LogError(string str) { }
    //public static void LogError(string str, object arg0) { }
    //public static void LogError(string str, params object[] param) { }
    //public static void LogError(string str, object arg0, object arg1) { }
    //public static void LogError(string str, object arg0, object arg1, object arg2) { }

    //public static void LogException(Exception e) { }
    //public static void LogException(string str, Exception e) { }

    public static void Error(string str) { }
    public static void Warning(string str) { }
#endif
    #endregion
}
