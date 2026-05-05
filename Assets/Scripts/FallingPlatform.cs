using UnityEngine;
using System.Collections;

public class FallingPlatform : MonoBehaviour
{
    public float fallDelay = 1.5f;
    public float respawnDelay = 3f;
    public float fallGravity = 3f;
    public float shakeIntensity = 0.05f;

    private Rigidbody2D rb;
    private Vector3 startPosition;
    private bool isFalling = false;
    private SpriteRenderer sr;
    private Color originalColor;
    private Collider2D col;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        if (sr != null) originalColor = sr.color;
        rb.bodyType = RigidbodyType2D.Static;
        startPosition = transform.position;
        gameObject.tag = "TemporaryPlatform";
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;
        if (isFalling)
        {
            Debug.Log($"[FallingPlatform] Collision ignored — already falling");
            return;
        }

        foreach (ContactPoint2D contact in collision.contacts)
        {
            Debug.Log($"[FallingPlatform] Contact normal: {contact.normal} | normalX:{contact.normal.x:F2} normalY:{contact.normal.y:F2}");

            if (Mathf.Abs(contact.normal.x) > 0.3f)
            {
                Debug.Log($"[FallingPlatform] Skipping side hit normalX:{contact.normal.x:F2}");
                continue;
            }

            if (contact.normal.y < -0.5f)
            {
                Debug.Log($"[FallingPlatform] Top hit detected — starting fall");

                PlayerController pc = collision.gameObject.GetComponent<PlayerController>();
                if (pc != null)
                {
                    Debug.Log($"[FallingPlatform] Player velocity on land: {pc.GetComponent<Rigidbody2D>()?.linearVelocity}");
                    pc.DisableJumpBriefly(0.1f);
                    Debug.Log($"[FallingPlatform] DisableJumpBriefly called");
                }

                StartCoroutine(Fall(collision.gameObject));
                break;
            }
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (Mathf.Abs(contact.normal.x) > 0.3f)
            {
                Debug.Log($"[FallingPlatform] STAY side contact detected — normalX:{contact.normal.x:F2} — this may cause boost!");
            }
        }
    }

    IEnumerator Fall(GameObject player)
    {
        isFalling = true;
        Debug.Log($"[FallingPlatform] Fall started");

        float elapsed = 0f;
        while (elapsed < fallDelay)
        {
            if (sr != null)
                sr.color = elapsed % 0.15f < 0.075f ? Color.red : originalColor;
            float shakeX = Random.Range(-shakeIntensity, shakeIntensity);
            float shakeY = Random.Range(-shakeIntensity, shakeIntensity);
            transform.position = startPosition + new Vector3(shakeX, shakeY, 0f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = startPosition;
        if (sr != null) sr.color = originalColor;

        if (player != null)
        {
            Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
            Debug.Log($"[FallingPlatform] Player velocity before ForceUnground: {playerRb?.linearVelocity}");

            if (playerRb != null && playerRb.linearVelocity.y <= 0.5f)
            {
                PlayerController pc = player.GetComponent<PlayerController>();
                if (pc != null)
                {
                    pc.ForceUnground();
                    Debug.Log($"[FallingPlatform] ForceUnground called — player velocity after: {playerRb?.linearVelocity}");
                }
            }
            else
            {
                Debug.Log($"[FallingPlatform] Skipped ForceUnground — player already jumping Y:{playerRb?.linearVelocity.y:F2}");
            }
        }

        if (col != null)
        {
            col.enabled = false;
            Debug.Log($"[FallingPlatform] Collider disabled");
        }

        float fallSpeed = 0f;
        float fallTimer = 0f;
        while (fallTimer < respawnDelay)
        {
            fallSpeed += fallGravity * Time.deltaTime;
            transform.position += Vector3.down * fallSpeed * Time.deltaTime;
            fallTimer += Time.deltaTime;
            yield return null;
        }

        // Reset position
        transform.position = startPosition;

        // Wait before re-enabling collider
        yield return new WaitForSeconds(0.5f);

        // Check if player is nearby before re-enabling
        bool playerNearby = false;
        Collider2D[] nearby = Physics2D.OverlapCircleAll(startPosition, 2f);
        foreach (var c in nearby)
        {
            if (c.CompareTag("Player"))
            {
                playerNearby = true;
                Debug.Log("[FallingPlatform] Player nearby on reset — waiting longer");
                break;
            }
        }

        if (playerNearby)
            yield return new WaitForSeconds(1f);

        if (col != null)
        {
            col.enabled = true;
            Debug.Log($"[FallingPlatform] Collider re-enabled — platform reset");
        }

        isFalling = false;
        Debug.Log($"[FallingPlatform] Fall complete — ready again");
    }
}