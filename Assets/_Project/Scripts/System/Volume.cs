using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Volume : MonoBehaviour
{
    [Header("-----Volume Setting-----")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider soundSlider;

    void Awake()
    {
        LoadVolume();
    }
    
    public void UpdateMusicVolume(float value)
    {
        audioMixer.SetFloat("MusicVolume", value);
    }

    public void UpdateSoundVolume(float value)
    {
        audioMixer.SetFloat("SoundVolume", value);
    }

    public void SaveVolume()
    {
        audioMixer.GetFloat("MusicVolume", out float musicVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);

        audioMixer.GetFloat("SoundVolume", out float soundVolume);
        PlayerPrefs.SetFloat("SoundVolume", soundVolume);
    }

    private void LoadVolume()
    {
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        soundSlider.value = PlayerPrefs.GetFloat("SoundVolume");
    }
}
