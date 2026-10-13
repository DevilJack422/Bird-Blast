using System;
using UnityEngine;

public enum SfxId
{
    SlingStretch, 
    SlingRelease,
    BirdHit,
    BlockBreak,
    PigDie,
    ButtonClick
}

public class AudioManager : MonoBehaviour
{
    [System.Serializable]
    public struct SfxEntry
    {
        public SfxId id;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume;
    }

    [Header("-----Source-----")]
    [SerializeField] private AudioSource sFXSource;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private SfxEntry[] sFXList;

    [Header("-----Volume-----")]
    [Range(0f, 1f)] [SerializeField] private float sFXVolume = 1f;
    [Range(0f, 1f)] [SerializeField] private float musicVolume = 1f;

    public static AudioManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    public void PlaySFX(SfxId id, float pitch = 1f)
    {
        foreach (var e in sFXList)
        {
            if (e.id != id) continue;
            sFXSource.pitch = pitch;
            sFXSource.PlayOneShot(e.clip, e.volume > 0 ? e.volume : 1f);
            return;
        }
        Debug.LogWarning($"{id} does not have clip");
    }

    public void PlayMusic(AudioClip clip)
    {
        if (musicSource.clip == clip && musicSource.isPlaying) return;
        musicSource.clip = clip;
        musicSource.Play();
    }
}
