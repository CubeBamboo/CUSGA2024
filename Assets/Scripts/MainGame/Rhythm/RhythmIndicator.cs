using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//now just for test
public class RhythmIndicator : MonoBehaviour
{
    //[SerializeField] private float xOffset = 0.5f;

    //TODO: delete. just for ongui debug
    private float targetTime1;
    private float targetTime2;

    private void Start()
    {
        //MusicRhythmManager.Instance.StartPlay();
    }

#if UNITY_EDITOR
    private void OnGUI()
    {
        GUI.skin.label.fontSize = 40;
        GUILayout.Label($"CurrentTime:{MusicRhythmManager.Instance.CurrentTime}");
        GUILayout.Label($"targetTime1:{targetTime1}");
        GUILayout.Label($"targetTime2:{targetTime2}");
    }
#endif

    private void Update()
    {
        //TODO: pre-process
        float singleDuration = 60 / MusicRhythmManager.Instance.MusicBpm;
        float offset = MusicRhythmManager.Instance.MusicOffset;

        float currentTime = MusicRhythmManager.Instance.CurrentTime;
        float tolerance = 0.1f;

        int k = (int)((currentTime-offset) / singleDuration);
        targetTime1 = singleDuration * k + offset;
        targetTime2 = singleDuration * (k + 1) + offset;

        if (Mathf.Abs(currentTime - targetTime1) < tolerance || Mathf.Abs(currentTime - targetTime2) < tolerance)
        {
            transform.DOScale(1.2f, 0.1f).OnComplete(() =>
                transform.DOScale(1f, 0.1f));
        }
    }
}
