using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardRow : MonoBehaviour
{
    [SerializeField] private TMP_Text rankText, nameText, levelsText, starsText, scoreText;
    [SerializeField] private Image background;

    public void Set(int rank, SaveSystem.BoardRow r, bool isMe, Color meColor)
    {
        rankText.text = rank.ToString();
        nameText.text = r.name;
        levelsText.text = $"{r.unlockedLevel}/{SaveSystem.TotalLevel}";
        starsText.text = $"{r.stars}/{SaveSystem.TotalLevel * 3}";
        scoreText.text = r.score.ToString("N0");
        if (background != null && isMe) background.color = meColor;
    }
}
