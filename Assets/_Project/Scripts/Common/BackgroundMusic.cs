using UnityEngine;
using UnityEngine.SceneManagement;

public class BackgroundMusic : MonoBehaviour
{
    void Start()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        string trackName = sceneName.StartsWith("Level ") ? "Gameplay" : "MainMenu";
        if (MusicManager.Instance != null && trackName != null)
        {
            MusicManager.Instance.PlayMusic(trackName);
        }
    }
}
