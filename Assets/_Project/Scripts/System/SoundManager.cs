using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] private SoundLibrary soundLibrary;
    [SerializeField] private AudioSource soundSource;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlaySound(string groupId, string soundName)
    {
        AudioClip clip = soundLibrary.GetClip(groupId, soundName);
        if (clip != null)
        {
            soundSource.PlayOneShot(clip);
        }
    }
}
