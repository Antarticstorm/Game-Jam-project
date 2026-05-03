using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class CameraFollow : MonoBehaviour
{
    [Header("AutoScroll")]
    public bool autoScrollEnabled = true;

    public Transform player;
    public float smoothSpeed = 3f;
    public float verticalOffset = 6.5f;
    public float descendSmoothing = 2.5f;
    public float autoScrollSpeed = 2f;
    public float deathZoneOffset = 14f;

    private float targetY;
    private bool playerDead = false;
    private float deathFollowDistance = 0f;
    private float deathFollowAmount = 10f;
    private Rigidbody2D playerRb;
    private bool playerKilled = false;

    private CameraShake cameraShake;

    void Start()
    {
        targetY = transform.position.y;

        if (player != null)
            playerRb = player.GetComponent<Rigidbody2D>();

        cameraShake = GetComponent<CameraShake>();
    }

    void LateUpdate()
    {
        if (player == null) return;

        float desiredY = player.position.y + verticalOffset;

        // Death check
        if (!playerKilled && player.position.y < transform.position.y - deathZoneOffset)
        {
            if (!GameManager.Instance.TryTriggerDeath()) return;
            playerKilled = true;
            autoScrollEnabled = false;
            StartCoroutine(DirectDeath(player.gameObject));
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
            if (desiredY > targetY)
            {
                float distance = desiredY - targetY;

                float dynamicSpeed = Mathf.Lerp(
                    smoothSpeed * 0.4f,
                    smoothSpeed * 3f,
                    Mathf.Clamp01(distance / 30f)
                );

                targetY = Mathf.Lerp(targetY, desiredY, dynamicSpeed * Time.deltaTime);
            }
            else
            {
                if (autoScrollEnabled)
                    targetY += autoScrollSpeed * Time.deltaTime;
                else
                {
                    float dropSpeed = smoothSpeed * 3f;
                    targetY = Mathf.Lerp(targetY, desiredY, dropSpeed * Time.deltaTime);
                }
            }
        }

        // FINAL POSITION + SHAKE (always applied)
        Vector3 targetPos = new Vector3(
            transform.position.x,
            targetY,
            transform.position.z
        );

        Vector3 offset = Vector3.zero;

        if (cameraShake != null)
            offset = cameraShake.GetShakeOffset();

        transform.position = targetPos + offset;
    }

    public void OnPlayerDeath()
    {
        playerDead = true;
        deathFollowDistance = 0f;
    }
    IEnumerator LoadGameOver()
    {
        AsyncOperation load = SceneManager.LoadSceneAsync("GameOver");
        load.allowSceneActivation = false;
        yield return new WaitForSeconds(0.2f);
        load.allowSceneActivation = true;
    }

    IEnumerator DirectDeath(GameObject player)
    {
        playerDead = true;
        deathFollowDistance = 2f;

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
        StartCoroutine(LoadGameOver());
    }
}