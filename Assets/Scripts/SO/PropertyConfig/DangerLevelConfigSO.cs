using UnityEngine;

namespace Shuile
{
    // config data by static class...
    public static class DangerLevelConfigClass
    {
        public struct Range
        {
            public float min;
            public float max;
        }

        public static Range[] levelRanges = new Range[5]
        {
            new() {min = 0f, max = 50f},
            new() {min = 50f, max = 150f},
            new() {min = 150f, max = 500f},
            new() {min = 500f, max = 1000f},
            new() {min = 1000f, max = 3000f}
        };
        public const float PlayerAttackAddition = 10;
        public const float EnemyDieBaseAddition = 50;
        public const float NormalReductionPerSecond = 10;
        public const float PlayerHurtReduction = 50;
    }

    public static class DangerLevelUtils
    {
        /// <returns> return -1 if config.levelRanges is empty</returns>
        public static int GetDangerLevelUnClamped(float score)
        {
            DangerLevelConfigClass.Range[] levelRanges = DangerLevelConfigClass.levelRanges;
            if (levelRanges.Length <= 0) return -1;
            if (score < levelRanges[0].min) return 0;
            if (score > levelRanges[^1].max) return levelRanges.Length - 1;

            for (int i = 0; i < levelRanges.Length; i++)
            {
                if (score >= levelRanges[i].min && score <= levelRanges[i].max)
                {
                    return i;
                }
            }
            return -1;
        }

        public static float GetEnemyKillAddition()
        {
            return DangerLevelConfigClass.EnemyDieBaseAddition;
        }
        public static int GetEnemySpawnThresold(int dangerLevel)
            => (int)((dangerLevel + 1) * 1.5f);
    }
}
