using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Trap : MonoBehaviour
{
    public TrapType trapType;
    public float jumpPadForce = 15f;
    private bool triggered;

    public enum TrapType
    {
        SpikeTrap,
        JumpPad
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (triggered) return;
        if (!collision.CompareTag("Player")) return;

        if (trapType == TrapType.JumpPad)
        {
            ActivateJumpPad(collision.gameObject);
            return;
        }

        triggered = true;
        CameraFollow cam = Camera.main.GetComponent<CameraFollow>();
        if (cam != null)
            cam.MiniShake(0.1f, 0.08f);

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
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        PlayerController pc = player.GetComponent<PlayerController>();
        Collider2D col = player.GetComponent<Collider2D>();

        CameraFollow cam = Camera.main.GetComponent<CameraFollow>();
        if (cam != null)
            cam.OnPlayerDeath();

        Animator anim = player.GetComponent<Animator>();
        if (anim != null)
            anim.SetTrigger("Death");

        if (pc != null)
            pc.enabled = false;

        if (col != null)
            col.enabled = false;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.gravityScale = 4f;
            StartCoroutine(SmoothKnockUp(rb));
        }

        yield return new WaitForSeconds(1.5f);

        if (rb != null)
            rb.linearVelocity = new Vector2(0f, -13f);

        yield return new WaitForSeconds(1.2f);

        Destroy(player);
        SceneManager.LoadScene("GameOver");
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