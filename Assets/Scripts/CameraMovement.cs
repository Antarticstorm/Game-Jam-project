using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public float smoothSpeed = 3f;
    public float verticalOffset = 6.5f;
    public float descendSmoothing = 2.5f;
    public float autoScrollSpeed = 2f;
    public float deathZoneOffset = 14f;

    private Vector3 shakeOffset = Vector3.zero;
    private float shakeTimer = 0f;
    private float shakeIntensity = 0f;
    private float targetY;
    private bool playerDead = false;
    private float deathFollowDistance = 0f;
    private float deathFollowAmount = 10f;
    private Rigidbody2D playerRb;
    private bool playerKilled = false;

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
        float desiredY = player.position.y + verticalOffset;

        if (!playerKilled && player.position.y < transform.position.y - deathZoneOffset)
        {
            playerKilled = true;
            Trap trap = FindObjectOfType<Trap>();
            if (trap != null)
                StartCoroutine(trap.DeathSequence(player.gameObject));
            else
            {
                StartCoroutine(DirectDeath(player.gameObject));
            }
        }

        if (playerDead)
        {
            if (deathFollowDistance < deathFollowAmount)
            {
                float progress = deathFollowDistance / deathFollowAmount;
                float slowedSpeed = Mathf.Lerp(descendSmoothing, 0f, progress);
                float prevTargetY = targetY;
                targetY = Mathf.Lerp(targetY, desiredY, slowedSpeed * Time.deltaTime);
                deathFollowDistance += Mathf.Abs(targetY - prevTargetY);
            }
        }
        else
        { 
            {
                if (desiredY > targetY)
                {
                    // How far above the camera target the player is
                    float distance = desiredY - targetY;

                    // Slow when close, fast when far — tweak the 5f threshold
                    float dynamicSpeed = Mathf.Lerp(smoothSpeed * 0.3f, smoothSpeed * 3f, Mathf.Clamp01(distance / 30f));

                    targetY = Mathf.Lerp(targetY, desiredY, dynamicSpeed * Time.deltaTime);
                }
                else
                {
                    targetY += autoScrollSpeed * Time.deltaTime;
                }
            }
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

    public void OnPlayerDeath()
    {
        playerDead = true;
        deathFollowDistance = 0f;
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

    IEnumerator DirectDeath(GameObject player)
    {
        playerDead = true;
        deathFollowDistance = 0f;

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        PlayerController pc = player.GetComponent<PlayerController>();
        Collider2D col = player.GetComponent<Collider2D>();
        Animator anim = player.GetComponent<Animator>();

        if (anim != null) anim.SetTrigger("Death");
        if (pc != null) pc.enabled = false;
        if (col != null) col.enabled = false;
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 4f;
        }

        yield return new WaitForSeconds(2f);
        Destroy(player);
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameOver");
    }
}