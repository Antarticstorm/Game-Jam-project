using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    public Transform player;

    public float smoothSpeed = 3f;
    public float verticalOffset = 4f;

    private float highestY;
    private Vector3 shakeOffset = Vector3.zero;
    private float shakeTimer = 0f;
    private float shakeIntensity = 0f;


    private Rigidbody2D playerRb;

    void Start()
    {
        highestY = transform.position.y;

        if (player != null)
            playerRb = player.GetComponent<Rigidbody2D>();
    }

    void LateUpdate()
    {
        if (player == null) return;

        if (player.position.y > highestY)
            highestY = player.position.y;

        float velocityY = (playerRb != null) ? playerRb.linearVelocity.y : 0f;
        float dynamicOffset = Mathf.Lerp(2f, 5f, Mathf.Clamp01(velocityY * 0.1f));

        Vector3 targetPos = new Vector3(
            transform.position.x,
            highestY + dynamicOffset,
            transform.position.z
        );

        Vector3 smoothPos = Vector3.Lerp(
            transform.position,
            targetPos,
            smoothSpeed * Time.deltaTime
        );

        Vector3 shakeOffset = Vector3.zero;

        if (shakeTimer > 0f)
        {
            shakeTimer -= Time.deltaTime;

            float x = Random.Range(-1f, 1f) * shakeIntensity;
            float y = Random.Range(-1f, 1f) * shakeIntensity;

            shakeOffset = new Vector3(x, y, 0f);
        }

        // FINAL POSITION (follow + shake)
        transform.position = smoothPos + shakeOffset;
    }

    public IEnumerator Shake(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            shakeOffset = new Vector3(x, y, 0f);

            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        shakeOffset = Vector3.zero;
    }
    public void MiniShake(float duration, float intensity)
    {
        shakeTimer = duration;
        shakeIntensity = intensity;
    }
}