using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shuile
{
    [CreateAssetMenu(fileName = "LevelDebugSettings", menuName = "Config/LevelDebugSettings")]
    public class LevelDebugSettingsSO : ScriptableObject
    {
        public bool needHitWithRhythm = true;
    }
}
