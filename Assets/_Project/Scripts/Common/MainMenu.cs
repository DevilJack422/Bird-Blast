using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("-----Panel Control-----")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject optionPanel;

    void Start()
    {
        menuPanel.SetActive(true);
        optionPanel.SetActive(false);
    }

    public void PlayClick()
    {
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
        menuPanel.SetActive(true);
    }

    public void QuitClick()
    {
        Application.Quit();
    }
}
