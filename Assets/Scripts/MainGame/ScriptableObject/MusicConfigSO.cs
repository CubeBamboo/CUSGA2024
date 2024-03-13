using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Music Config", menuName = "Config/Music Config")]
public class MusicConfigSO : ScriptableObject
{
    public AudioClip clip;
    public float bpm; //TODO: value > 0 check
    public float offset; //in ms
}
