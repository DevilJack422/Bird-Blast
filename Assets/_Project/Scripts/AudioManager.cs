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

    const string MusicKey = "MusicVolume";
    const string SfxKey = "SfxVolume";

    public float MusicVolume => musicVolume;
    public float SFXVolume => sFXVolume;

    public static AudioManager Instance;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        sFXVolume = PlayerPrefs.GetFloat(SfxKey, sFXVolume);
        musicVolume = PlayerPrefs.GetFloat(MusicKey, musicVolume);
        musicSource.volume = musicVolume;
        musicSource.loop = true;
    }

    public void PlaySFX(SfxId id, float pitch = 1f)
    {
        foreach (var e in sFXList)
        {
            if (e.id != id) continue;
            float clipVol = e.volume > 0 ? e.volume : 1f;
            sFXSource.pitch = pitch;
            sFXSource.PlayOneShot(e.clip, clipVol * sFXVolume);
            return;
        }
        Debug.LogWarning($"{id} does not have clip");
    }

    public void PlayMusic(AudioClip clip)
    {
        if (musicSource.clip == clip && musicSource.isPlaying) return;
        musicSource.clip = clip;
        musicSource.volume = musicVolume;
        musicSource.Play();
    }

    public void SetMusicVolume(float value)
    {
        musicVolume = Mathf.Clamp01(value);
        musicSource.volume = musicVolume;
        PlayerPrefs.SetFloat(MusicKey, value);
    }

    public void SetSfxVolume(float value)
    {
        sFXVolume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat(SfxKey, value);
    }

    void OnApplicationQuit() => PlayerPrefs.Save();
}
