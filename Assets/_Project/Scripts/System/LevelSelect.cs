using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelect : MonoBehaviour
{
    public void Level1Click()
    {
        SceneManager.LoadScene("Level 1");
    }

    public void Level2Click()
    {
        SceneManager.LoadScene("Level 2");
    }

    public void Level3Click()
    {
        SceneManager.LoadScene("Level 3");
    }

    public void MenuClick()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
