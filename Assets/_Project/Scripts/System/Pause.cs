using UnityEngine;

public class Pause : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;

    void Start()
    {
        pausePanel.SetActive(false);
    }

    public void PauseClick()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ResumeClick()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void RestartClick()
    {
        SceneLoader.Reload();
    }

    public void MenuClick()
    {
        SceneLoader.Load("MainMenu");
    }
}
