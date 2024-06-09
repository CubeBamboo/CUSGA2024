using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shuile
{
    [CreateAssetMenu(fileName = "New PlayerProperty", menuName = "Config/PlayerProperty")]
    public class PlayerPropertySO : ScriptableObject
    {
        public int maxHealthPoint;
        public int attackPoint;
    }
}
