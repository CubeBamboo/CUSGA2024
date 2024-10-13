using CbUtils.Extension;
using Shuile.Core.Global;
using UnityEngine;

namespace Shuile.Gameplay.Entity
{
    internal class LevelEntityFactory
    {
        public LevelEntityFactory(LevelEntityManager levelEntityManager, PrefabConfigSO prefabConfig)
        {
            LevelEntityManager = levelEntityManager;
            PrefabConfig = prefabConfig;
        }

        public LevelEntityManager LevelEntityManager { get; }
        public PrefabConfigSO PrefabConfig { get; }

        #region Mechanism

        public GameObject SpawnLaser()
        {
            var go = PrefabConfig.laser.Instantiate(); // spawn
            return go;
        }

        #endregion
    }
}
