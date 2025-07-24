using UnityEngine;

[CreateAssetMenu(fileName = "SoundsSO", menuName = "Audio/SoundsSO")]
public class SoundsSO : ScriptableObject
{
    public SoundGroup[] soundLists;

    public SoundGroup GetSoundGroup(SoundType type)
    {
        foreach (var group in soundLists)
        {
            if (group.type == type)
                return group;
        }
        return null;
    }
}

[System.Serializable]
public class SoundGroup
{
    public SoundType type;
    public AudioClip[] sounds;
    [Range(0f, 1f)] public float volume = 1f;
}
