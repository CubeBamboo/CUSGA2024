using UnityEngine;

namespace Shuile.Core.Global.Config
{
    /// <summary>
    ///     level data config (fixed in game)
    /// </summary>
    [CreateAssetMenu(fileName = "New LevelConfig", menuName = "Config/Level Config")]
    public class LevelConfigSO : ScriptableObject
    {
        [Header("In Level")]
        [Tooltip("(in plus of minus and in ms) Input offset out of this value will be judged by miss.")]
        public float missTolerance = 150f;

        public float MissToleranceInSeconds => missTolerance * 0.001f;

        public int playerNotePreShowTime = 2;

        [Header("MusicManager")] public bool playOnAwake = true;

        public float playTimeScale = 1f;
        public float volume = 0.4f;

        [Header("PlayerSettingsConfig (Obsolete)")]
        public float globalOffset;

        [Header("LevelDebugSettings (for debug)")]
        public bool needHitWithRhythm = true;
    }
}
