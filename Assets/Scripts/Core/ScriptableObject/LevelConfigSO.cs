using UnityEngine;

/// <summary>
/// level data config (fixed in game)
/// </summary>
[CreateAssetMenu(fileName = "New LevelConfig", menuName = "Config/Level Config")]
public class LevelConfigSO : ScriptableObject
{
    [Tooltip("(in plus of minus and in ms) Input offset out of this value will be judged by miss.")]
    public float missTolerance = 150f;

    public int playerNotePreShowTime = 2;
}
