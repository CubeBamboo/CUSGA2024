using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelConfigSO", menuName = "Config/LevelConfigSO")]
public class LevelConfigSO : ScriptableObject
{
    [Tooltip("(in plus of minus and in ms) Input offset out of this value will be judged by miss.")]
    public float missTolerance = 150f;
}
