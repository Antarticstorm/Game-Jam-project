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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
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
            if (pc != null)
                pc.ForceUnground();
        }

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = fallGravity;

        yield return new WaitForSeconds(respawnDelay);

        rb.bodyType = RigidbodyType2D.Static;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        transform.position = startPosition;
        transform.rotation = Quaternion.identity;
        isFalling = false;
    }
}