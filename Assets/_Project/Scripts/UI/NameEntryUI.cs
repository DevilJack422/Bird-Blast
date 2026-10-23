using TMPro;
using UnityEngine;

public class NameEntryUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_InputField input;
    [SerializeField] private TMP_Text errorText, playerLabel;

    void Start()
    {
        input.characterLimit = 12;
        Refresh();
        if (!SaveSystem.HasPlayer) Open();
        else panel.SetActive(false);
    }

    public void Open()
    {
        panel.SetActive(true);
        input.text = SaveSystem.CurrentPlayer;
        errorText.text = "";
        input.ActivateInputField();
    }

    public void OnConfirm()
    {
        if (SaveSystem.TrySetPlayer(input.text, out string err))
        {
            panel.SetActive(false);
            Refresh();
        }
        errorText.text = err;
    }

    public void Refresh()
    {
        playerLabel.text = SaveSystem.HasPlayer ? $"Player: {SaveSystem.CurrentPlayer}" : "";
    }
}
