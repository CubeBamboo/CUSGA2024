using Shuile.Rhythm;

using UnityEngine;

namespace Shuile
{
    [System.Serializable]
    public class LevelData
    {
        public string label;
        public string sceneName;
        public LevelEnemySO enemyData;
        public ChartSO chartFiles;
    }

    [CreateAssetMenu(fileName = "LevelDataMap", menuName = "Config/LevelDataMap")]
    public class LevelDataMapSO : ScriptableObject
    {
        public LevelData[] levelDataList;
    }

    public static class LevelDataMapSOExtensions
    {
        public static LevelData GetLevelData(this LevelDataMapSO levelDataMap, string label)
        {
            foreach (var levelData in levelDataMap.levelDataList)
            {
                if (levelData.label == label)
                {
                    return levelData;
                }
            }
            return null;
        }
    }
}
