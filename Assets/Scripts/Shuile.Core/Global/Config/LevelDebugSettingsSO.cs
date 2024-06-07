using UnityEngine;

namespace Shuile.Core.Global.Config
{
    [CreateAssetMenu(fileName = "LevelDebugSettings", menuName = "Config/LevelDebugSettings")]
    public class LevelDebugSettingsSO : ScriptableObject
    {
        public bool needHitWithRhythm = true;
    }
}
