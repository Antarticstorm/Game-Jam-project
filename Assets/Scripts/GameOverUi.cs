using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private Transform entryContainer;
    [SerializeField] private GameObject entryPrefab;
    [SerializeField] private TextMeshProUGUI lastRunText;

    private void Start()
    {
        Canvas.ForceUpdateCanvases();
        StartCoroutine(PopulateLeaderboardAsync());
    }

    private IEnumerator PopulateLeaderboardAsync()
    {
        foreach (Transform child in entryContainer)
            Destroy(child.gameObject);

        if (lastRunText != null)
        {
            int score = GameManager.Instance.Score;
            float time = GameManager.Instance.TimeAlive;
            lastRunText.text = $"Last Run: {score} coins | {FormatTime(time)}";
        }

        var entries = LeaderboardManager.Instance.Data.entries;
        for (int i = 0; i < entries.Count; i++)
        {
            var entry = entries[i];
            GameObject row = Instantiate(entryPrefab, entryContainer);
            TextMeshProUGUI[] texts = row.GetComponentsInChildren<TextMeshProUGUI>();
            texts[0].text = $"#{i + 1}";
            texts[1].text = $"{entry.score} coins";
            texts[2].text = FormatTime(entry.timeAlive);

            yield return null;        
        }
    }

    private string FormatTime(float seconds)
    {
        int m = Mathf.FloorToInt(seconds / 60);
        int s = Mathf.FloorToInt(seconds % 60);
        return $"{m:00}:{s:00}";
    }

    public void OnRetryPressed()
    {
        GameManager.Instance.ResetGame();
        SceneManager.LoadScene("MainGameplay");
    }

    public void OnMenuPressed()
    {
        GameManager.Instance.ResetGame();
        SceneManager.LoadScene("Main Menu");
    }
}