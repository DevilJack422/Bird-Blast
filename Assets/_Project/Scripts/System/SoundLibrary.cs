using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SoundGroup
{
    public string groupId;
    public AudioClip[] clips;
}

public class SoundLibrary : MonoBehaviour
{
    [SerializeField] private SoundGroup[] soundEffects;

    // groupId -> (clipName -> clip)
    private Dictionary<string, Dictionary<string, AudioClip>> lookup;   

    public AudioClip GetClip(string groupId, string clipName)
    {
        foreach (SoundGroup group in soundEffects)
        {
            if (group.groupId != groupId) continue;

            foreach (AudioClip clip in group.clips)
            {
                if (clip != null && clip.name == clipName) return clip;
            }
        }

        Debug.LogWarning($"SoundLibrary: Clip '{clipName}' none found in group '{groupId}'.", this);
        return null;
    } 
}
