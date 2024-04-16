using CbUtils.Config;
using UDebug = UnityEngine.Debug;

namespace Shuile.Framework
{
    public interface ICbLogger
    {
        void Log(object message, string label = "");
        void LogWarning(object message, string label = "");
        void LogError(object message, string label = "");
        public static bool CheckLabel(string label)
            => !CbGlobalConfig.Logger.labelEnable.ContainsKey(label) || CbGlobalConfig.Logger.labelEnable[label];
    }

    public static class CbLogger
    {
        public static void Log(object message, string label = "")
        {
            if (ICbLogger.CheckLabel(label))
                UDebug.Log(message);
        }
        public static void LogWarning(object message, string label = "")
        {
            if (ICbLogger.CheckLabel(label))
                UDebug.LogWarning(message);
        }
        public static void LogError(object message, string label = "")
        {
            if (ICbLogger.CheckLabel(label))
                UDebug.LogError(message);
        }
    }
}
