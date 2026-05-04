using UnityEngine;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    public GameObject[] levelSections;
    public GameObject backgroundPrefab;
    public float offsetX = 0f;
    public float offsetY = 0f;

    [Range(1, 5)]
    public int recentHistorySize = 3; // how many recent levels to avoid

    private List<int> recentIndexes = new List<int>();

    public void SpawnNextSection(Vector3 spawnPosition)
    {
        int index = GetWeightedRandomIndex();

        // Track recent indexes
        recentIndexes.Add(index);
        if (recentIndexes.Count > recentHistorySize)
            recentIndexes.RemoveAt(0);

        Instantiate(levelSections[index], spawnPosition, Quaternion.identity);

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

    private int GetWeightedRandomIndex()
    {
        // Build weight list — recent levels get much lower weight
        float[] weights = new float[levelSections.Length];
        for (int i = 0; i < weights.Length; i++)
        {
            int howRecentlyUsed = recentIndexes.LastIndexOf(i);
            if (howRecentlyUsed == -1)
                weights[i] = 10f; // not used recently — high chance
            else
            {
                // The more recently used, the lower the weight
                int stepsAgo = recentIndexes.Count - 1 - howRecentlyUsed;
                weights[i] = Mathf.Max(0.5f, stepsAgo * 2f);
            }
        }

        // Pick based on weights
        float totalWeight = 0f;
        foreach (float w in weights) totalWeight += w;

        float roll = Random.Range(0f, totalWeight);
        float cumulative = 0f;

        for (int i = 0; i < weights.Length; i++)
        {
            cumulative += weights[i];
            if (roll <= cumulative)
                return i;
        }

        return levelSections.Length - 1;
    }
}