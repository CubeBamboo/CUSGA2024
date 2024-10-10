using Shuile.Core.Gameplay.Data;
using Shuile.Core.Global.Config;
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
    }
}
