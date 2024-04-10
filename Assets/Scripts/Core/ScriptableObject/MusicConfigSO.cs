using UnityEngine;

/// <summary>
/// bind resource by scriptableobject
/// </summary>
[CreateAssetMenu(fileName = "New MusicConfig", menuName = "Config/Music Config")]
public class ChartSO : ScriptableObject
{
    public TextAsset chartFile;
    public AudioClip clip;
}
