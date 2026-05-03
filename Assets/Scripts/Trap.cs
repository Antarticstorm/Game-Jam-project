using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Trap : MonoBehaviour
{
    public TrapType trapType;
    public float jumpPadForce = 15f;
    private static bool anyTriggered = false;

    public enum TrapType
    {
        SpikeTrap,
        JumpPad
    }

    private void OnEnable()
    {
        anyTriggered = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (anyTriggered) return;
        if (!collision.CompareTag("Player")) return;

        if (trapType == TrapType.JumpPad)
        {
            ActivateJumpPad(collision.gameObject);
            return;
        }

        anyTriggered = true;
        StartCoroutine(DeathSequence(collision.gameObject));
    }

    void ActivateJumpPad(GameObject player)
    {
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        PlayerController pc = player.GetComponent<PlayerController>();
        if (rb != null)
        {
            Vector2 vel = rb.linearVelocity;
            vel.y = 0f;
            rb.linearVelocity = vel;
            rb.AddForce(Vector2.up * jumpPadForce, ForceMode2D.Impulse);
        }
        if (pc != null)
            pc.ForceExternalVelocity(new Vector2(0f, jumpPadForce));
    }

    public IEnumerator DeathSequence(GameObject player)
    {
        if (player == null) yield break;

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        PlayerController pc = player.GetComponent<PlayerController>();
        Collider2D col = player.GetComponent<Collider2D>();
        Animator anim = player.GetComponent<Animator>();
        CameraFollow cam = Camera.main?.GetComponent<CameraFollow>();

        // Disable player immediately
        if (pc != null) pc.enabled = false;
        if (col != null) col.enabled = false;
        if (cam != null) cam.OnPlayerDeath(); // only called ONCE now
        if (anim != null) anim.SetTrigger("Death");

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.gravityScale = 4f;
            rb.linearDamping = 0f;
            StartCoroutine(SmoothKnockUp(rb));
        }

        yield return new WaitForSeconds(1.5f);

        if (rb != null && player != null)
            rb.linearVelocity = new Vector2(0f, -13f);

        yield return new WaitForSeconds(1.2f);

        if (player != null)
        {
            GameManager.Instance.GameOver();
            Destroy(player);
        }

        StartCoroutine(LoadGameOver());
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
            if (rb == null) yield break;
            rb.linearVelocity = Vector2.Lerp(startVel, targetVel, time / duration);
            yield return null;
        }
        if (rb != null)
            rb.linearVelocity = targetVel;
    }

    IEnumerator LoadGameOver()
    {
        AsyncOperation load = SceneManager.LoadSceneAsync("GameOver");
        load.allowSceneActivation = false;
        yield return new WaitForSeconds(0.2f);
        load.allowSceneActivation = true;
    }
}