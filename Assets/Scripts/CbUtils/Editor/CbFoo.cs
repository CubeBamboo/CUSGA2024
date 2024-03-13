using UnityEngine;
using UnityEngine.InputSystem;


public class CbFoo : MonoBehaviour
{
    private AudioSource audioSource;
    private float timer;
    private bool isPlaying;

    public float offset; //in ms
    public float offsetInSeconds => offset*0.001f;
    private Keyboard keyboard;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        timer = 0f;
        isPlaying = false;

        keyboard = Keyboard.current;
    }

#if UNITY_EDITOR
    private void OnGUI()
    {
        GUI.skin.label.fontSize = 40;
        GUILayout.Label($"Timer:{timer}");
        GUILayout.Label($"AudioSettings.dspTime:{AudioSettings.dspTime}");
    }
#endif

    private void Update()
    {
        if (keyboard.aKey.wasPressedThisFrame)
        {
            Debug.Log("was pressed");
            isPlaying = true;
            timer = 0;

            PlayAudio(offsetInSeconds);
        }

        if (isPlaying)
        {
            timer += Time.deltaTime;
        }
    }

    private void PlayAudio()
    {
        audioSource.Play();
    }

    public void PlayAudio(float offset)
    {
        float preTime = 2f; // 2000ms for prepare the AudioPlay (in case offset < 0)
        audioSource.PlayScheduled(AudioSettings.dspTime + preTime + offset);
    }

    public void Play()
    {
        isPlaying = true;
        timer = 0;
        audioSource.Play();
    }

    public void PlayDelayed()
    {
        isPlaying = true;
        timer = 0;
        audioSource.PlayDelayed(offsetInSeconds);
    }

    public void PlayScheduled()
    {
        isPlaying = true;
        float preTime = 2f; // 2000ms for prepare the AudioPlay (in case offset < 0)
        timer = -preTime;

        Time.timeScale = 0.5f;
        audioSource.pitch = 0.5f;

        audioSource.PlayScheduled(AudioSettings.dspTime - preTime + offsetInSeconds);
    }
}
