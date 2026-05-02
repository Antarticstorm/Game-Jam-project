using UnityEngine;

public class SectionTrigger : MonoBehaviour
{
    public GameObject gridPrefab;
    public Transform spawnPoint; // Assign an empty GameObject at the end of your level section

    private bool triggered = false; // Prevents double-spawning

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            triggered = true;

            // Spawns the next section at the designated spawn point
            Instantiate(gridPrefab, spawnPoint.position, Quaternion.identity);

            // Optional: destroy this trigger after use
            Destroy(gameObject, 0.5f);
        }
    }
}