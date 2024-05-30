using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shuile
{
    [CreateAssetMenu(fileName = "MusicManagerConfig", menuName = "Config/MusicManagerConfig")]
    public class MusicManagerConfigSO : ScriptableObject
    {
        public bool playOnAwake = true;
        public float playTimeScale = 1f;
        public float volume = 0.4f;
    }
}
