using UnityEngine;
using UnityEngine.Audio;

public class SectionTrigger : MonoBehaviour
{
    public Transform spawnPoint; // Assign an empty GameObject at the end of your level section

    private bool triggered = false; // Prevents double-spawning



    private void OnTriggerEnter2D(Collider2D other)
    {

        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            triggered = true;

            Object.FindAnyObjectByType<LevelManager>().SpawnNextSection(spawnPoint.position);

            // Optional: destroy this trigger after use
            Destroy(gameObject, 0.5f);
        }
    }
}