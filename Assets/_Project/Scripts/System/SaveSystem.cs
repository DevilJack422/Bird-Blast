using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

public static class SaveSystem
{
    private static string FilePath => Path.Combine(Application.persistentDataPath, "save.json");
    private static SaveData data;
    private static SaveData Data { get { if (data == null) Load(); return data; } }

    public static string CurrentPlayer => Data.currentPlayer;
    public static bool HasPlayer => !string.IsNullOrEmpty(Data.currentPlayer);


    //------------------Load & Write File-----------------------------
    private static void Load()
    {
        try
        {
            if (File.Exists(FilePath))
            {
                data = JsonUtility.FromJson<SaveData>(File.ReadAllText(FilePath));
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"SaveSystem: file save error, create new. {e.Message}");
        }
        data ??= new SaveData();
        MergeDuplicates();
    }

    private static void Save()
    {
        try
        {
            File.WriteAllText(FilePath, JsonUtility.ToJson(Data, true));
        }
        catch (Exception e)
        {
            Debug.LogWarning($"SaveSystem: cannot write save file. {e.Message}");
        }
    }

    //------------------------Player's name--------------------------
    public static bool TrySetPlayer(string raw, out string error)
    {
        string name = Sanitize(raw);
        if (name.Length < 3)
        {
            error = "Name must be 3-12 characters";
            return false;
        }
        Data.currentPlayer = name;
        Save();
        error = null;
        return true;
    }

    private static string Sanitize(string s)
    {
        if (string.IsNullOrWhiteSpace(s)) return "";
        var sb = new StringBuilder();
        foreach (char c in s.Trim())
        {
            if (char.IsLetterOrDigit(c) || c == ' ' || c == '_') sb.Append(c);
        }
        string r = sb.ToString().Trim();
        return r.Length > 12 ? r.Substring(0, 12).Trim() : r;
    }

    //-----------------Progress--------------------------
    private static PlayerProgress GetProgress(bool create)
    {
        if (!HasPlayer)  return null;

        var p = Data.players.Find(x => string.Equals(x.name, CurrentPlayer, StringComparison.OrdinalIgnoreCase));
        if (p == null && create)
        {
            p = new PlayerProgress { name = CurrentPlayer };
            Data.players.Add(p);
        }
        return p;
    }

    private static LevelProgress GetLevel(int level) =>
        GetProgress(false)?.levels.Find(l => l.levelIndex == level);

    public static int GetStars(int level) => GetLevel(level)?.stars ?? 0;
    public static int GetBestScore(int level) => GetLevel(level)?.bestScore ?? 0;
    public static bool IsUnlock(int level) => level <= 1 || GetStars(level - 1) >= 1;
    public static int GetTotalStars() => GetProgress(false)?.levels.Sum(l => l.stars) ?? 0; 

    //---------------Write result after win-----------------
    /// Return true if break record of this level
    public static bool SaveResult(int level, int score, int stars)
    {
        if (!HasPlayer)
        {
            Debug.LogWarning("[Save] Player does not have name, result NOT save!");
            return false;
        }
        Debug.Log($"[Save] Player={CurrentPlayer} level={level}, score={score}, stars={stars}");

        var p = GetProgress(true);
        var lp = p.levels.Find(l => l.levelIndex == level);
        if (lp == null)
        {
            lp = new LevelProgress
            {
                levelIndex = level
            };
            p.levels.Add(lp);
        }

        bool newBest = score > lp.bestScore;
        lp.bestScore = Math.Max(lp.bestScore, score);
        lp.stars = Math.Max(lp.stars, stars);

        var e = Data.entries.Find(x => x.level == level && string.Equals(x.player, CurrentPlayer, StringComparison.OrdinalIgnoreCase));
        string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        if (e == null)
        {
            Data.entries.Add(new ScoreEntry { player = CurrentPlayer, level = level, score = score, stars = stars, date = now });
        }
        else if (score > e.score)
        {
            e.score = score;
            e.stars = Math.Max(e.stars, stars);
            e.date = now;
        }

        Save();
        return newBest;
    }

    //----------------Leader Board-----------------
    ///Level = 0: General board(sum all best score of every level)
    public static List<ScoreEntry> GetTop(int level, int count = 100)
    {
        IEnumerable<ScoreEntry> q;
        if (level > 0)
        {
            q = Data.entries.Where(e => e.level == level);
        }
        else
        {
            q = Data.entries.GroupBy(e => e.player.ToLowerInvariant())
                            .Select(g => new ScoreEntry
                            {
                                player = g.First().player,
                                score = g.Sum(x => x.score),
                                stars = g.Sum(x => x.stars),
                                date = g.Max(x => x.date)
                            });
        }
        return q.OrderByDescending(e => e.score).ThenBy(e => e.date).Take(count).ToList();
    }

    private static void MergeDuplicates()
    {
        var merged = new List<PlayerProgress>();
        foreach (var p in data.players)
        {
            var m = merged.Find(x => string.Equals(x.name, p.name, StringComparison.OrdinalIgnoreCase));
            if (m == null)
            {
                merged.Add(p);
                continue;
            }

            foreach (var lp in p.levels)
            {
                var ml = m.levels.Find(l => l.levelIndex == lp.levelIndex);
                if (ml == null) m.levels.Add(lp);
                else
                {
                    ml.bestScore = Mathf.Max(ml.bestScore, lp.bestScore);
                    ml.stars = Mathf.Max(ml.stars, lp.stars);
                }
            }
        }
        data.players = merged;
    }

    public static void ResetAll()
    {
        data = new SaveData();
        try
        {
            if (File.Exists(FilePath)) File.Delete(FilePath);
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.Message);
        }
    }
}
