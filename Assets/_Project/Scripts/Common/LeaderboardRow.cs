using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardRow : MonoBehaviour
{
    [SerializeField] private TMP_Text rankText, nameText, scoreText;
    [SerializeField] private Image background;

    public void Set(int rank, string player, int score, bool isMe, Color meColor)
    {
        rankText.text = rank.ToString();
        nameText.text = player;
        scoreText.text = score.ToString("N0");
        if (isMe) background.color = meColor;
    }
}
