using UnityEngine;

public class LevelMusic : MonoBehaviour
{
    [SerializeField] private AudioClip levelMusic;

    void Start() 
    {
        AudioManager.Instance.PlayMusic(levelMusic);    
    }
}
