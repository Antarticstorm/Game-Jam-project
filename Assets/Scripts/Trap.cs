using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Trap : MonoBehaviour
{
    public GameObject gameOverUI;

    private bool triggered;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (triggered) return;

        if (collision.CompareTag("Player"))
        {
            triggered = true;

            CameraFollow cam = Camera.main.GetComponent<CameraFollow>();
            if (cam != null)
            {
                cam.MiniShake(0.1f, 0.08f);
            }

            StartCoroutine(DeathSequence(collision.gameObject));
        }
    }

    IEnumerator DeathSequence(GameObject player)
    {
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        PlayerController pc = player.GetComponent<PlayerController>();
        Collider2D col = player.GetComponent<Collider2D>();

        // stop player input/movement system first
        if (pc != null)
            pc.enabled = false;

        // disable collisions so nothing interferes with fall
        if (col != null)
            col.enabled = false;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;

            // make physics fully responsible
            rb.gravityScale = 4f;

            // Velocity handler
            StartCoroutine(SmoothKnockUp(rb));
        }

        // short delay before falling
        yield return new WaitForSeconds(1.5f);

        if (rb != null)
        {
            // force downward fall (Mario-style drop)
            rb.linearVelocity = new Vector2(0f, -13f);
        }

        yield return new WaitForSeconds(1.2f);

        Destroy(player);

        if (gameOverUI != null)
            gameOverUI.SetActive(true);

        Time.timeScale = 0f;
    }
    IEnumerator SmoothKnockUp(Rigidbody2D rb)
    {
        float time = 0.1f;
        float duration = 0.2f;

        Vector2 startVel = rb.linearVelocity;
        Vector2 targetVel = new Vector2(0f, 15f);

        while (time < duration)
        {
            time += Time.deltaTime;

            rb.linearVelocity = Vector2.Lerp(startVel, targetVel, time / duration);

            yield return null;
        }

        rb.linearVelocity = targetVel;
    }

}