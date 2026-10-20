using UnityEngine;

[CreateAssetMenu(menuName = "Game/Level Data")]
public class LevelData : ScriptableObject
{
    public int levelIndex = 1;
    public string levelName = "Level 1";
    public string nextSceneName; // null if final level
    public int oneStart = 10000;
    public int twoStart = 30000;
    public int threeStart = 50000;
    public int birdBonus = 10000;
}
