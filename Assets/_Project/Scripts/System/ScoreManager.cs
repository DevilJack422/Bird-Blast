using System;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }
    public int Point { get; private set; }
    public event Action<int> OnScoreChanged;

    void Awake() => Instance = this;
    void OnEnable() => Destructible.Destroyed += HandleDestroy;
    void OnDisable() => Destructible.Destroyed -= HandleDestroy;

    private void HandleDestroy(Destructible d) => AddCombo(d.PointValue);

    public void AddCombo(int baseValue)
    {
        float mult = ComboSystem.Instance != null ? ComboSystem.Instance.Register() : 1f;
        Add(Mathf.RoundToInt(baseValue * mult));
    }

    public void Add(int amount)
    {
        Point += amount;
        OnScoreChanged?.Invoke(Point);
    }
}
