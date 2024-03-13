using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shuile
{
    /// <summary>
    /// use for player settings
    /// </summary>
    [CreateAssetMenu(fileName = "New Player Config", menuName = "Config/PlayerConfig")]
    public class PlayerConfigSO : ScriptableObject
    {
        /// <summary>
        /// (in ms) time need to delay when judge rhythm
        /// </summary>
        public float globalOffset = 0f;
    }
}
