using UnityEngine;
using TMPro;

public class HUDManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timerText;

    private void Update()
    {
        if (GameManager.Instance == null) return;

        scoreText.text = $"Coins: {GameManager.Instance.Score}";

        float t = GameManager.Instance.TimeAlive;
        int m = Mathf.FloorToInt(t / 60);
        int s = Mathf.FloorToInt(t % 60);
        timerText.text = $"{m:00}:{s:00}";
    }
}