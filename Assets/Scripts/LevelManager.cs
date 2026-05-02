using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public GameObject[] levelSections;
    public GameObject backgroundPrefab;
    public float offsetX = 0f; // Tweak this in Inspector
    public float offsetY = 0f; // Tweak this in Inspector

    public void SpawnNextSection(Vector3 spawnPosition)
    {
        int randomIndex = Random.Range(0, levelSections.Length);
        Instantiate(levelSections[randomIndex], spawnPosition, Quaternion.identity);

        if (backgroundPrefab != null)
        {
            Vector3 bgPosition = new Vector3(
                spawnPosition.x + offsetX,
                spawnPosition.y + offsetY,
                0
            );
            Instantiate(backgroundPrefab, bgPosition, Quaternion.identity);
        }
    }
}