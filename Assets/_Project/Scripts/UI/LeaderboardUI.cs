using UnityEngine;

public class LeaderboardUI : MonoBehaviour
{
    [SerializeField] private Transform content;
    [SerializeField] private LeaderboardRow rowPrefab;
    [SerializeField] private GameObject emptyText;
    [SerializeField] private Color meColor = new Color(1f, 0.9f, 0.4f, 0.5f);

    int currentTab;

    void OnEnable()
    {
        Show(currentTab);
    }

    public void Show(int level)
    {
        currentTab = level;
        foreach (Transform c in content) Destroy(c.gameObject);

        var list = SaveSystem.GetTop(level, 10);
        emptyText.SetActive(list.Count == 0);

        for (int i = 0; i < list.Count; i++)
        {
            bool isMe = string.Equals(list[i].player, SaveSystem.CurrentPlayer, System.StringComparison.OrdinalIgnoreCase);
            Instantiate(rowPrefab, content).Set(i + 1, list[i].player, list[i].score, isMe, meColor);
        }
    }
}
