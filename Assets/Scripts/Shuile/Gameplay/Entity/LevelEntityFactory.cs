using CbUtils.Extension;
using Shuile.Core.Global;

namespace Shuile.Gameplay.Entity
{
    public class LevelEntityFactory
    {
        public PrefabConfigSO PrefabConfig { get; } = GameApplication.BuiltInData.globalPrefabs;

        private ClassedObjectPool<Laser> _laserPool;

        public LevelEntityFactory()
        {
            _laserPool = new ClassedObjectPool<Laser>(() =>
            {
                var laser = PrefabConfig.laser.Instantiate().GetComponent<Laser>();
                laser.FxEnd = () => _laserPool.Release(laser);
                return laser;
            });
        }

        public Laser SpawnLaser()
        {
            return _laserPool.Get();
        }
    }
}
