using UnityEngine;
using System.Collections;

public class FallingPlatform : MonoBehaviour
{
    public float fallDelay = 1.5f;
    public float respawnDelay = 3f;
    public float fallGravity = 3f;
    public float shakeIntensity = 0.05f; // how hard it shakes, tweak this

    private Rigidbody2D rb;
    private Vector3 startPosition;
    private bool isFalling = false;
    private SpriteRenderer sr;
    private Color originalColor;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        if (sr != null) originalColor = sr.color;
        rb.bodyType = RigidbodyType2D.Static;
        startPosition = transform.position;
        gameObject.tag = "Platform";
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y < -0.5f && !isFalling)
            {
                StartCoroutine(Fall());
                break;
            }
        }
    }

    IEnumerator Fall()
    {
        isFalling = true;

        // flash + shake before falling
        float elapsed = 0f;
        while (elapsed < fallDelay)
        {
            // flash
            if (sr != null)
                sr.color = elapsed % 0.15f < 0.075f ? Color.red : originalColor;

            // shake — offset from startPosition
            float shakeX = Random.Range(-shakeIntensity, shakeIntensity);
            float shakeY = Random.Range(-shakeIntensity, shakeIntensity);
            transform.position = startPosition + new Vector3(shakeX, shakeY, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // reset position and color before falling
        transform.position = startPosition;
        if (sr != null) sr.color = originalColor;

        // fall
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = fallGravity;

        yield return new WaitForSeconds(respawnDelay);

        // reset everything
        rb.bodyType = RigidbodyType2D.Static;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        transform.position = startPosition;
        transform.rotation = Quaternion.identity;

        isFalling = false;
    }
}