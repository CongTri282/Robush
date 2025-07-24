using UnityEngine;

public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance { get; private set; }

    [Header("Music Clips")]
    [SerializeField] private AudioClip[] bgmTracks;

    [Range(0f, 1f)]
    [SerializeField] private float volume = 0.1f;

    private AudioSource bgmSource;

    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // AudioSource setup
        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = true;
        bgmSource.playOnAwake = false;
        bgmSource.volume = volume;
    }

    /// <summary>
    /// Play background music from index in the array
    /// </summary>
    public void PlayBGM(int trackIndex)
    {
        if (trackIndex < 0 || trackIndex >= bgmTracks.Length)
        {
            Debug.LogWarning("BGMManager: Invalid track index.");
            return;
        }

        AudioClip selectedTrack = bgmTracks[trackIndex];
        // Always assign and play, so it resumes after StopBGM()
        bgmSource.clip = selectedTrack;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    public void PauseBGM()
    {
        bgmSource.Pause();
    }

    public void ResumeBGM()
    {
        bgmSource.UnPause();
    }

    public void SetVolume(float newVolume)
    {
        volume = Mathf.Clamp01(newVolume);
        bgmSource.volume = volume;
    }

    public AudioSource AudioSource => bgmSource;
}
