using UnityEngine;

public class CollectPopup : MonoBehaviour
{
    public float lifetime = 1f;
    public float moveSpeed = 1f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        // Float upward
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

    }
}