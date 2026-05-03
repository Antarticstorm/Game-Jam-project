using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public int Score { get; private set; }
    public float TimeAlive { get; private set; }
    public bool IsGameOver { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (!IsGameOver)
            TimeAlive += Time.deltaTime;
    }

    public void AddScore(int amount)
    {
        if (!IsGameOver)
            Score += amount;
    }

    public void GameOver()
    {
        if (IsGameOver) return;
        IsGameOver = true;
        LeaderboardManager.Instance.SubmitEntry(Score, TimeAlive);
    }

    public void ResetGame()
    {
        Score = 0;
        TimeAlive = 0f;
        IsGameOver = false;
    }

    // Any system can ask if death is already handled
    public bool TryTriggerDeath()
    {
        if (IsGameOver) return false; // already dead
        GameOver();
        return true; // this caller gets to run the death sequence
    }
}