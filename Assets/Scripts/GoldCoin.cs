using UnityEngine;

public class GoldCoin : MonoBehaviour, Items
{
    [SerializeField] private GameObject collectPopupPrefab;

    public void Collect()
    {
        if (collectPopupPrefab != null)
        {
            // Spawn popup at coin's position
            Instantiate(collectPopupPrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }

}
