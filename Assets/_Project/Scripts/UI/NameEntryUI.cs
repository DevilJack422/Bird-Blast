using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class NameEntryUI : MonoBehaviour
{
    [SerializeField] private GameObject playerEntryPanel;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private TMP_InputField input;
    [SerializeField] private TMP_Text errorText, playerLabel;

    private MainMenu mainMenu;

    void Start()
    {
        input.characterLimit = 12;
        Refresh();
        if (!SaveSystem.HasPlayer) Open();
        else playerEntryPanel.SetActive(false);
    }

    public void Open()
    {
        menuPanel.SetActive(false);
        playerEntryPanel.SetActive(true);
        input.text = SaveSystem.CurrentPlayer;
        errorText.text = "";
        input.ActivateInputField();
    }

    public void OnConfirm()
    {
        if (SaveSystem.TrySetPlayer(input.text, out string err))
        {
            playerEntryPanel.SetActive(false);
            menuPanel.SetActive(true);
            Refresh();
        }
        errorText.text = err;
    }

    public void Refresh()
    {
        playerLabel.text = SaveSystem.HasPlayer ? $"Player: {SaveSystem.CurrentPlayer}" : "";
    }
}
