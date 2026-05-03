using UnityEngine;

public class GoldCoin : MonoBehaviour, Items
{
    public void Collect()
    {
        Destroy(gameObject);
    }

}
