using UnityEngine;

public class SectionCleanup : MonoBehaviour
{
    public float destroyDistance = 100f; // increase this to be wider than one full level section

    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

    }

    void Update()
    {
        if (player == null) return;

        float distance = player.position.y - transform.position.y;

        if (distance > destroyDistance)
        {
            Debug.Log("Destroying: " + gameObject.name);
            Destroy(gameObject);
        }
    }
}