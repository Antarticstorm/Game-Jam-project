using UnityEngine;

public class GoldCoin : MonoBehaviour, Items
{
    [SerializeField] private GameObject collectPopupPrefab;
    [SerializeField] private int scoreValue = 10;

    public void Collect()
    {
        if (collectPopupPrefab != null)
            Instantiate(collectPopupPrefab, transform.position, Quaternion.identity);

        AudioManager.Instance.PlayCoin();
        GameManager.Instance.AddScore(scoreValue);

        Destroy(gameObject);
    }
}
