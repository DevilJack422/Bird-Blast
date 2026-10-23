using System.Collections.Generic;

[System.Serializable]
public class LevelProgress
{
    public int levelIndex;
    public int bestScore;
    public int stars;
}

[System.Serializable]
public class PlayerProgress
{
    public string name;
    public List<LevelProgress> levels = new();
}

[System.Serializable]
public class ScoreEntry
{
    public string player;
    public int level;
    public int score;
    public int stars;
    public string date;
}

[System.Serializable]
public class SaveData
{
    public string currentPlayer = "";
    public List<PlayerProgress> players = new();
    public List<ScoreEntry> entries = new();
}
