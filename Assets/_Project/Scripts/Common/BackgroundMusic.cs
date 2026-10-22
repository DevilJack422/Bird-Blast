using UnityEngine;
using UnityEngine.SceneManagement;

public class BackgroundMusic : MonoBehaviour
{
    void Start()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == "Level 1" || sceneName == "Level 2" || sceneName == "Level 3")
        {
            MusicManager.Instance.PlayMusic("Gameplay");
        }
        else if (sceneName == "MainMenu" || sceneName == "LevelSelect")
        {
            MusicManager.Instance.PlayMusic("MainMenu");
        }
    }
}
