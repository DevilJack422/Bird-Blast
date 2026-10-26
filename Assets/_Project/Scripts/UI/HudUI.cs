using TMPro;
using UnityEngine;

public class HudUI : MonoBehaviour
{
    [SerializeField] private LevelManager levelManager;
    [SerializeField] TMP_Text scoreText, birdsText;

    void Start()
    {
        ScoreManager.Instance.OnScoreChanged += SetScore;
        levelManager.OnBirdLeftChanged += SetBirds;
        SetScore(0);
        SetBirds(levelManager.BirdsLeft);
    }

    private void OnDestroy()
    {
        if (ScoreManager.Instance != null) ScoreManager.Instance.OnScoreChanged -= SetScore;
        if (levelManager != null) levelManager.OnBirdLeftChanged -= SetBirds;
    }

    private void SetScore(int point) => scoreText.text = $"Score: {point:N0}";
    private void SetBirds(int bird) => birdsText.text = $"Bird: {bird}";
}
