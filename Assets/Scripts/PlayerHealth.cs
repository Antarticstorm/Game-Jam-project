using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public void Die()
    {
        GameManager.Instance.GameOver();
    }
}