using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    public Button retryButton;

    void Start()
    {
        retryButton.onClick.AddListener(Retry);
    }

    void Retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainGameplay");
    }
}