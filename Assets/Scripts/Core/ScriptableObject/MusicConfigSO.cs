using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Obsolete("get data from chart file")]
[CreateAssetMenu(fileName = "New MusicConfig", menuName = "Config/Music Config")]
public class MusicConfigSO : ScriptableObject
{
    public AudioClip clip;
    public float bpm;
    /// <summary>
    /// (in ms) define where is the first beat in audio clip.
    /// </summary>
    public float offset;
}
