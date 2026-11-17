using UnityEngine;

public class LeaderboardUI : MonoBehaviour
{
    [SerializeField] private Transform content;
    [SerializeField] private LeaderboardRow rowPrefabs;
    [SerializeField] private int maxRows = 10;
    [SerializeField] Color meColor = new Color(1f, 0.9f, 0.4f, 0.5f);

    void OnEnable() => Refresh();

    private void Refresh()
    {
        foreach (Transform c in content) Destroy(c.gameObject);

        var rows = SaveSystem.GetBoard();
        for (int i = 0; i < rows.Count && i < maxRows; i++)
        {
            bool isMe = string.Equals(rows[i].name, SaveSystem.CurrentPlayer, System.StringComparison.OrdinalIgnoreCase);
            Instantiate(rowPrefabs, content).Set(i + 1, rows[i], isMe, meColor);
        }
    }
}
