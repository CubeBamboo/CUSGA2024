using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Music Config", menuName = "Config/Music Config")]
public class MusicConfigSO : ScriptableObject
{
    public AudioClip clip;
    public float bpm;
    /// <summary>
    /// (in ms) define where is the first beat in audio clip.
    /// </summary>
    public float offset;
}
