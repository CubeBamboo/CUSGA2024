using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New LevelConfig", menuName = "Config/Level Config")]
public class LevelConfigSO : ScriptableObject
{
    [Tooltip("(in plus of minus and in ms) Input offset out of this value will be judged by miss.")]
    public float missTolerance = 150f;

    [Tooltip("(in ms) global interval to prepare before music play.")]
    public float prePlayInterval = 2000f;
}
