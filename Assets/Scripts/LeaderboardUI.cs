using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LeaderboardUI : MonoBehaviour
{
    [SerializeField] private Transform entryContainer;
    [SerializeField] private GameObject entryPrefab; // A UI prefab with 3 Text fields
    [SerializeField] private TextMeshProUGUI lastRunText;

    private void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        // Clear old entries
        foreach (Transform child in entryContainer)
            Destroy(child.gameObject);

        var entries = LeaderboardManager.Instance.Data.entries;

        for (int i = 0; i < entries.Count; i++)
        {
            var entry = entries[i];
            GameObject row = Instantiate(entryPrefab, entryContainer);
            TextMeshProUGUI[] texts = row.GetComponentsInChildren<TextMeshProUGUI>();

            texts[0].text = $"#{i + 1}";
            texts[1].text = $"{entry.score} coins";
            texts[2].text = FormatTime(entry.timeAlive);
        }

        // Show last run stats
        if (lastRunText != null)
        {
            lastRunText.text = $"Last Run: {GameManager.Instance.Score} coins | " +
                               $"{FormatTime(GameManager.Instance.TimeAlive)}";
        }
    }

    private string FormatTime(float seconds)
    {
        int m = Mathf.FloorToInt(seconds / 60);
        int s = Mathf.FloorToInt(seconds % 60);
        return $"{m:00}:{s:00}";
    }
}