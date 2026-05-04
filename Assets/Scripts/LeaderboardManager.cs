using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class LeaderboardEntry
{
    public int score;
    public float timeAlive;

    public LeaderboardEntry(int score, float timeAlive)
    {
        this.score = score;
        this.timeAlive = timeAlive;
    }
}

[System.Serializable]
public class LeaderboardData
{
    public List<LeaderboardEntry> entries = new List<LeaderboardEntry>();
}

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager Instance { get; private set; }

    private const string SaveKey = "Leaderboard";
    private const int MaxEntries = 10;

    public LeaderboardData Data { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Load();
    }

    public void SubmitEntry(int score, float timeAlive)
    {
        Debug.Log($"[Leaderboard] Submitting score:{score} time:{timeAlive}");
        Data.entries.Add(new LeaderboardEntry(score, timeAlive));
        Data.entries.Sort((a, b) => b.score.CompareTo(a.score));

        if (Data.entries.Count > MaxEntries)
            Data.entries.RemoveRange(MaxEntries, Data.entries.Count - MaxEntries);

        Save();
        Debug.Log($"[Leaderboard] Saved {Data.entries.Count} entries");
    }

    private void Save()
    {
        string json = JsonUtility.ToJson(Data);
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();
    }

    private void Load()
    {
        string json = PlayerPrefs.GetString(SaveKey, "");
        Debug.Log($"[Leaderboard] Load JSON: {json}");

        if (string.IsNullOrEmpty(json))
            Data = new LeaderboardData();
        else
        {
            try
            {
                Data = JsonUtility.FromJson<LeaderboardData>(json);
                if (Data == null || Data.entries == null)
                    Data = new LeaderboardData();
            }
            catch
            {
                Debug.Log("[Leaderboard] Failed to parse JSON, resetting data");
                Data = new LeaderboardData();
            }
        }
    }

    public void ClearLeaderboard()
    {
        Data = new LeaderboardData();
        Save();
    }
}