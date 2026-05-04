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

        foreach (ContactPoint2D contact in collision.contacts)
        {
            // Ignore side hits entirely
            if (Mathf.Abs(contact.normal.x) > 0.3f) return;

            // Only top hits
            if (contact.normal.y < -0.5f && !isFalling)
            {
                PlayerController pc = collision.gameObject.GetComponent<PlayerController>();
                if (pc != null)
                    pc.DisableJumpBriefly(0.13f);
                StartCoroutine(Fall(collision.gameObject));
                break;
            }
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        // If player is below the platform center, ignore collision
        if (collision.transform.position.y < transform.position.y)
        {
            Physics2D.IgnoreCollision(col, collision.collider, true);
        }
    }

    IEnumerator Fall(GameObject player)
    {
        isFalling = true;

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
            PlayerController pc = player.GetComponent<PlayerController>();
            if (pc != null) pc.ForceUnground();
        }

        // Fall by moving transform directly — no bodyType switch
        float fallSpeed = 0f;
        float fallTimer = 0f;
        while (fallTimer < respawnDelay)
        {
            fallSpeed += fallGravity * Time.deltaTime;
            transform.position += Vector3.down * fallSpeed * Time.deltaTime;
            fallTimer += Time.deltaTime;
            yield return null;
        }

        // Reset
        transform.position = startPosition;
        isFalling = false;
    }
}