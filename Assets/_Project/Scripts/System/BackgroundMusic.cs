using UnityEngine;
using UnityEngine.SceneManagement;

public class BackgroundMusic : MonoBehaviour
{
    void Start()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == "Level 1")
        {
            MusicManager.Instance.PlayMusic("Gameplay");
        }
        else if (sceneName == "MainMenu")
        {
            MusicManager.Instance.PlayMusic("MainMenu");
        }
    }
}
