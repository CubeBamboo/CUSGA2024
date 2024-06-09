using CbUtils.Extension;
using Shuile.Gameplay.Entity;
using Shuile.Gameplay.Manager;
using Shuile.Rhythm;
using Shuile.Rhythm.Runtime;
using UnityEngine;

namespace Shuile.Gameplay
{
    public class LevelSingletonsSpawner : MonoBehaviour
    {
        private Transform _dependencyParent; // not singleton
        private Transform _singletonParent; // singleton

        public void Awake()
        {
            _dependencyParent = new GameObject($"{nameof(_dependencyParent)}").SetParent(transform).transform;
            _singletonParent = new GameObject($"{nameof(_singletonParent)}").SetParent(transform).transform;
            
            // new GameObject($"{nameof(LevelEntityManager)}", typeof(LevelEntityManager))
            //     .SetParent(_singletonParent);
            new GameObject($"{nameof(LevelGlobalManager)}", typeof(LevelGlobalManager))
                .SetParent(_singletonParent);
        }

        public void OnDestroy()
        {
            Destroy(_dependencyParent.gameObject);
            Destroy(_singletonParent.gameObject);
        }
    }
}
