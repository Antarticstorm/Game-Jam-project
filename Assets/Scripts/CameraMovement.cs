using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public float smoothSpeed = 3f;
    public float verticalOffset = 7.5f;
    public float descendSmoothing = 2.5f;

    private Vector3 shakeOffset = Vector3.zero;
    private float shakeTimer = 0f;
    private float shakeIntensity = 0f;
    private float targetY;


    private Rigidbody2D playerRb;

    void Start()
    {
        targetY = transform.position.y;
        if (player != null)
            playerRb = player.GetComponent<Rigidbody2D>();
    }


    void LateUpdate()
    {
        if (player == null) return;

        float velocityY = (playerRb != null) ? playerRb.linearVelocity.y : 0f;
        bool playerGrounded = (playerRb != null) && Mathf.Abs(velocityY) < 0.1f;

        float desiredY = player.position.y + verticalOffset;

        if (desiredY > targetY)
            targetY = Mathf.Lerp(targetY, desiredY, smoothSpeed * Time.deltaTime);
        else
        {
            float dropSpeed = playerGrounded ? smoothSpeed * 3f : descendSmoothing;
            targetY = Mathf.Lerp(targetY, desiredY, dropSpeed * Time.deltaTime);
        }


        Vector3 targetPos = new Vector3(
            transform.position.x,
            targetY,
            transform.position.z
        );

        shakeOffset = Vector3.zero;
        if (shakeTimer > 0f)
        {
            shakeTimer -= Time.deltaTime;
            float x = Random.Range(-1f, 1f) * shakeIntensity;
            float y = Random.Range(-1f, 1f) * shakeIntensity;
            shakeOffset = new Vector3(x, y, 0f);
        }

        transform.position = targetPos + shakeOffset;
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