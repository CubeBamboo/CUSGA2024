using Shuile.Core.Gameplay;
using Shuile.Core.Gameplay.Data;
using Shuile.Core.Global;
using Shuile.ResourcesManagement.Loader;

namespace Shuile
{
    public static class LevelEntityUtils
    {
        public static UnityEngine.GameObject EnemyType2Prefab(EnemyType enemyType)
        {
            PrefabConfigSO prefabConfig = LevelResourcesLoader.Instance.SyncContext.globalPrefabs;
            var res = enemyType switch
            {
                EnemyType.ZakoRobot => prefabConfig.zakoRobot,
                EnemyType.Creeper => prefabConfig.creeper,
                EnemyType.MahouDefenseTower => prefabConfig.mahouDefenseTower,
                _ => throw new System.Exception("Invalid EnemyType."),
            };
            return res;
        }
    }
}
