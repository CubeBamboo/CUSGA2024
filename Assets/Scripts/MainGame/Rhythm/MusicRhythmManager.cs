using CbUtils;
using UnityEngine;

//control the music time progress
public class MusicRhythmManager : MonoSingletons<MusicRhythmManager>
{
    [SerializeField] private LevelConfigSO levelConfig;
    [SerializeField] private MusicConfigSO currentMusic;
    private float currentTime; // timer for music playing
    private bool isPlaying = false;

    private AudioSource audioSource; // for music playing

    public bool playOnAwake = true;

    public bool IsPlaying => isPlaying;
    public float MissTolerance => levelConfig.missTolerance;
    public float CurrentTime => currentTime;
    public float BpmInterval => 60f / currentMusic.bpm;
    public float MusicBpm => currentMusic.bpm;
    public float MusicOffset => currentMusic.offset;

    protected override void Awake()
    {
        base.Awake();
        audioSource = gameObject.AddComponent<AudioSource>(); //not audiomanager...
    }

    private void Start()
    {
        //Time.timeScale = 0.5f; // TODO: delete. just for test
        InitMusic();
        currentTime = 0;
        StartPlay();
    }

    void FixedUpdate()
    {
        if (!isPlaying)
            return;

        currentTime += Time.fixedDeltaTime;
    }

    private void InitMusic()
    {
        audioSource.clip = currentMusic.clip;
        audioSource.playOnAwake = false;
        audioSource.loop = true;
        audioSource.volume = 0.4f;
    }

    public void StartPlay()
    {
        //audioSource.pitch = 0.5f; // TODO: delete.
        float offset = currentMusic.offset * 0.001f;

        // play clip
        if (currentMusic.offset > 0)
            audioSource.PlayScheduled(AudioSettings.dspTime + offset);
        else
            audioSource.PlayDelayed(offset);

        currentTime = -offset;
        isPlaying = true; // -> start timing

    }
}
