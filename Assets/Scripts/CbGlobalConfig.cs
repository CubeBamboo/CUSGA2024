using System.Collections.Generic;

namespace CbUtils.Config
{
    public static class CbGlobalConfig
    {
        public static class Logger
        {
            public static Dictionary<string, bool> labelEnable = new()
            {
                { "GamePlay", true },
                { "EnemyRoundManager.cs", false },
                { "AudioOffsetDebugger", true }
            };
        }

        public static class IamXXX
        {

        }
    }
}
