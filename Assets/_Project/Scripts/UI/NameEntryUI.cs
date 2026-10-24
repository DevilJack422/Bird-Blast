using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class NameEntryUI : MonoBehaviour
{
    [Header("-----Entry Name-----")]
    [SerializeField] private GameObject playerEntryPanel;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private TMP_InputField input;
    [SerializeField] private TMP_Text errorText, playerLabel;

    [Header("-----Confirm If Name Existed-----")]
    [SerializeField] private GameObject confirmPanel;
    [SerializeField] private TMP_Text confirmText;

    private string pendingName;

    void Start()
    {
        input.characterLimit = 12;
        confirmPanel.SetActive(false);
        Refresh();
        if (!SaveSystem.HasPlayer) Open();
        else playerEntryPanel.SetActive(false);
    }

    public void Open()
    {
        menuPanel.SetActive(false);
        playerEntryPanel.SetActive(true);
        confirmPanel.SetActive(false);
        input.text = SaveSystem.CurrentPlayer;
        errorText.text = "";
        input.ActivateInputField();
    }

    public void OnConfirm()
    {
        if (!SaveSystem.ValidateName(input.text, out string clean, out string err))
        {
            errorText.text = err;
            return;
        }

        bool isCurrent = string.Equals(clean, SaveSystem.CurrentPlayer, System.StringComparison.CurrentCultureIgnoreCase);

        // Name none taken or is using by current player: Accept to the game
        if (isCurrent || !SaveSystem.PlayerExists(clean))
        {
            Accept(clean);
            return;
        }

        // Name data taken: ask again
        SaveSystem.GetPlayerSummary(clean, out string shown, out int stars, out int cleared);
        pendingName = clean;
        confirmText.text = $"The name \"{shown}\" already exists\n" +
                            $"({cleared} level(s) cleared, {stars} stars(s)).\n\n" +
                            "Is this you?";
        confirmPanel.SetActive(true);
    }

    // "Yes, continue" button
    public void ConfirmExistingClick()
    {
        confirmPanel.SetActive(false);
        Accept(pendingName);
    }

    // "Choose another name" button
    public void ChooseAnotherClick()
    {
        confirmPanel.SetActive(false);
        errorText.text = "Please enter a different name";
        input.text = "";
        input.ActivateInputField();
    }

    private void Accept(string clean)
    {
        SaveSystem.SetPlayer(clean);
        playerEntryPanel.SetActive(false);
        menuPanel.SetActive(true);
        Refresh();    
    }

    public void Refresh()
    {
        playerLabel.text = SaveSystem.HasPlayer ? $"Player: {SaveSystem.CurrentPlayer}" : "";
    }
}
