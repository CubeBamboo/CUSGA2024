namespace Shuile.Framework
{
    public static class GlobalEventUtils
    {
        public static void SafeTrigger(System.Action triggerDo)
        {
            if (triggerDo == null) return;
            try
            {
                triggerDo();
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogWarning($"GlobalEventUtils captures some exceptions: {e}");
            }
        }
    }
}
