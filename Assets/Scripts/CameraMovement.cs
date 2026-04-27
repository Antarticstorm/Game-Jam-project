using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Reference to the player object
    public Transform player;

    // How fast the camera smoothly follows upward movement
    public float smoothSpeed = 5f;

    // Tracks the highest Y position reached by the player
    private float highestY;

    void Start()
    {
        // Initialize camera starting Y position as the baseline
        highestY = transform.position.y;
    }

    void LateUpdate()
    {
        // Only update camera height if player goes higher than previous peak
        if (player.position.y > highestY)
        {
            highestY = player.position.y;
        }

        // Target camera position (only Y changes, X stays fixed)
        Vector3 targetPos = new Vector3(
            transform.position.x,
            highestY,
            transform.position.z
        );

        // Smoothly move camera toward target position
        transform.position = Vector3.Lerp(
            transform.position,
            targetPos,
            smoothSpeed * Time.deltaTime
        );
    }
}