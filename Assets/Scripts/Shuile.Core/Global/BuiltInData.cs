using Shuile.Core.Gameplay.Data;
using Shuile.Core.Global.Config;
using System.Collections.Generic;
using UnityEngine;

namespace Shuile.Core.Global
{
    /// <summary>
    ///     facade for built-in data (mostly are configuration like scriptable object). <br/>
    ///     config it in unity editor. and load this scriptable object in runtime with resource loader.
    /// </summary>
    [CreateAssetMenu(fileName = "BuiltInData", menuName = "Shuile/BuiltInData")]
    public class BuiltInData : ScriptableObject
    {
        public LevelConfigSO levelConfig;
        public PrefabConfigSO globalPrefabs;
        public LevelDataMapSO levelDataMap;

        [SerializeField] private GameObject[] prefabArray;

        public GameObject GetFromPrefabArray(string prefabName)
        {
            foreach (var prefabItem in prefabArray)
            {
                if (prefabItem.name == prefabName)
                {
                    return prefabItem;
                }
            }

            throw new KeyNotFoundException("Prefab not found in prefab array: " + prefabName);
        }

    }
}
