using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance { get; private set; }

    [Header("Sound Data")]
    [SerializeField] public SoundsSO soundsSO;

    private Dictionary<SoundType, SoundGroup> soundLookup;
    private AudioSource sfxAudioSource;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        sfxAudioSource = gameObject.AddComponent<AudioSource>();
        InitializeSoundLookup();
    }

    private void InitializeSoundLookup()
    {
        soundLookup = new Dictionary<SoundType, SoundGroup>();
        foreach (var group in soundsSO.soundLists)
        {
            if (!soundLookup.ContainsKey(group.type))
            {
                soundLookup.Add(group.type, group);
            }
        }
    }

    public void PlaySFX(SoundType type)
    {
        if (soundLookup.TryGetValue(type, out var group))
        {
            if (group.sounds.Length > 0)
            {
                var clip = group.sounds[Random.Range(0, group.sounds.Length)];
                sfxAudioSource.PlayOneShot(clip, group.volume);
            }
        }
        else
        {
            Debug.LogWarning($"SFXManager: SoundType '{type}' not found.");
        }
    }
}