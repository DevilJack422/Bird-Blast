using System.Collections;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultUI : MonoBehaviour
{
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private GameObject winPanel, losePanel;
    [SerializeField] private TMP_Text titleText, scoreText;
    [SerializeField] private Image[] starts;
    [SerializeField] private Button nextButton;
    [SerializeField] private Color startOn = new Color(1f, 0.85f, 0.2f);
    [SerializeField] private Color startOff = new Color(0.3f, 0.3f, 0.3f, 0.6f);

    void Start()
    {
        winPanel.SetActive(false);
        losePanel.SetActive(false);
        levelManager.OnLevelEnded += Show;
    }

    void OnDestroy()
    {
        if (levelManager != null) levelManager.OnLevelEnded -= Show;
    }

    private void Show(bool won, int point, int startCount)
    {
        if (won) StartCoroutine(WinRoutine(point, startCount));
        else losePanel.SetActive(true);
    }

    private IEnumerator WinRoutine(int point, int startCount)
    {
        winPanel.SetActive(true);
        titleText.text = levelManager.Data.levelName + " Clear!";
        nextButton.gameObject.SetActive(!string.IsNullOrEmpty(levelManager.Data.nextSceneName));
        foreach (var s in starts)
        {
            s.color = startOff;
            s.transform.localScale = Vector3.one;
        }

        // Point slowly increase from 0
        for (float t = 0; t < 1f; t += Time.unscaledDeltaTime)
        {
            scoreText.text = Mathf.RoundToInt(Mathf.Lerp(0, point, t)).ToString("N0");
            yield return null;
        }
        scoreText.text = point.ToString("N0");

        // Start turnly appear, effect from small to big
        for (int i = 0; i < startCount; i++)
        {
            yield return new WaitForSeconds(0.35f);
            starts[i].color = startOn;
            for (float t = 0; t < 0.2f; t += Time.unscaledDeltaTime)
            {
                float k = Mathf.Sin(t / 0.2f * Mathf.PI) * 0.4f; // Swelled up and then went back
                starts[i].transform.localScale = Vector3.one * (1f + k);
                yield return null;
            }
            starts[i].transform.localScale = Vector3.one;
        }
    }

    public void ReplayClick() => SceneLoader.Reload();
    public void NextLevelClick() => SceneLoader.Load(levelManager.Data.nextSceneName);
    public void MenuClick() => SceneLoader.Load("MainMenu");
}
