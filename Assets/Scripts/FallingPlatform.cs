using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

public class FallingPlatform : MonoBehaviour
{
    public float fallDelay = 1.5f;
    public float respawnDelay = 3f;
    public float fallGravity = 3f;

    private Rigidbody2D rb;
    private Vector3 startPosition;
    private bool isFalling = false;
    private SpriteRenderer sr;
    private Tilemap tilemap;
    private Color originalColor;
    private Collider2D col;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        tilemap = GetComponent<Tilemap>();
        col = GetComponent<Collider2D>();
        if (sr != null) originalColor = sr.color;
        else if (tilemap != null) originalColor = tilemap.color;
        rb.bodyType = RigidbodyType2D.Static;
        startPosition = transform.position;
        gameObject.tag = "TemporaryPlatform";
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player") || isFalling) return;

        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (Mathf.Abs(contact.normal.x) <= 0.3f && contact.normal.y < -0.5f)
            {
                StartCoroutine(Fall());
                break;
            }
        }
    }

    private void SetWarningColor(Color color)
    {
        if (sr != null) sr.color = color;
        else if (tilemap != null) tilemap.color = color;
    }

    IEnumerator Fall()
    {
        isFalling = true;

        float elapsed = 0f;
        while (elapsed < fallDelay)
        {
            // Keep the solid surface still: shaking its collider pushes the player.
            SetWarningColor(elapsed % 0.15f < 0.075f ? Color.red : originalColor);
            elapsed += Time.deltaTime;
            yield return null;
        }

        SetWarningColor(originalColor);

        // The player's current contacts determine grounding, even if they have
        // already moved to another platform since triggering this timer.
        if (col != null) col.enabled = false;

        float fallSpeed = 0f;
        float fallTimer = 0f;
        while (fallTimer < respawnDelay)
        {
            fallSpeed += fallGravity * Time.deltaTime;
            transform.position += Vector3.down * fallSpeed * Time.deltaTime;
            fallTimer += Time.deltaTime;
            yield return null;
        }

        transform.position = startPosition;
        yield return new WaitForSeconds(0.5f);

        // Keep checking instead of restoring the collider through a nearby player.
        while (IsPlayerNearby())
            yield return new WaitForSeconds(0.1f);

        if (col != null) col.enabled = true;
        isFalling = false;
    }

    private bool IsPlayerNearby()
    {
        Collider2D[] nearby = Physics2D.OverlapCircleAll(startPosition, 2f);
        foreach (Collider2D other in nearby)
        {
            if (other.CompareTag("Player")) return true;
        }
        return false;
    }
}
