using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("-----Panel Control-----")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject optionPanel;
    [SerializeField] private GameObject leaderBoardPanel;
    [SerializeField] private GameObject quitButton;

    private NameEntryUI nameEntry;


    void Start()
    {
        if (nameEntry == null) nameEntry = FindAnyObjectByType<NameEntryUI>();

#if UNITY_WEBGL && !UNITY_EDITOR
            if (quitButton == null) quitButton.SetActive(false);
#endif

        menuPanel.SetActive(true);
        optionPanel.SetActive(false);
        leaderBoardPanel.SetActive(false);
    }

    public void PlayClick()
    {
        if (!SaveSystem.HasPlayer)
        {
            nameEntry.Open();
            return;
        }
        SceneManager.LoadScene("LevelSelect");
    }

    public void OptionClick()
    {
        menuPanel.SetActive(false);
        optionPanel.SetActive(true);
    }

    public void BackClick()
    {
        optionPanel.SetActive(true == false);
        leaderBoardPanel.SetActive(true == false);
        menuPanel.SetActive(true);
    }

    public void QuitClick()
    {
        Application.Quit();
    }

    public void ResetClick()
    {
        SaveSystem.ResetAll();
        SceneLoader.Reload();
    }

    public void LeaderClick()
    {
        menuPanel.SetActive(false);
        leaderBoardPanel.SetActive(true);
    }
}
