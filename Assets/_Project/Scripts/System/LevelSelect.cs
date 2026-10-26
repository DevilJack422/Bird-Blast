using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
    [System.Serializable]
    public class LevelSlot
    {
        public string sceneName = "Level 1";
        public Button button;
        public GameObject lockIcon;
        public TMP_Text bestScoreText;
        public Image[] stars;
    }

    [SerializeField] private LevelSlot[] levels = new LevelSlot[3];
    [SerializeField] private Color starOn = Color.white;
    [SerializeField] private Color starOff = new Color(0.3f, 0.3f, 0.3f, 0.6f);

    void OnEnable()
    {
        Refresh();
    }

    public void Level1Click() => PlayLevel(1);
    public void Level2Click() => PlayLevel(2);
    public void Level3Click() => PlayLevel(3);
     public void MenuClick() => SceneLoader.Load("MainMenu");

    private void PlayLevel(int level)
    {
        if (!SaveSystem.IsUnlock(level)) return;
        if (level < 1 || level > levels.Length) return;
        SceneLoader.Load(levels[level - 1].sceneName);
    }

    private void Refresh()
    {
        for (int i = 0; i < levels.Length; i++)
        {
            int level = i + 1;
            var slot = levels[i];
            bool isUnlocked = SaveSystem.IsUnlock(level);
            int starCount = SaveSystem.GetStars(level);

            if (slot.button != null) slot.button.interactable = isUnlocked;
            if (slot.lockIcon != null) slot.lockIcon.SetActive(!isUnlocked);

            if (slot.bestScoreText != null)
            {
                int best = SaveSystem.GetBestScore(level);
                slot.bestScoreText.text = isUnlocked && best > 0 ? best.ToString("N0") : "";
            }

            if (slot.stars != null)
            {
                for (int s = 0; s < slot.stars.Length; s++)
                {
                    if (slot.stars[s] != null)
                    {
                        slot.stars[s].color = (isUnlocked && s < starCount) ? starOn : starOff;
                    }
                }
            }
        }
    }
}
