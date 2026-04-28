using Unity.VectorGraphics;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Jump and movement settings
    public float jumpForceX = 8f;
    public float jumpForceY = 12f;
    public float runSpeed = 3f;
    public float wallStickTime = 2f;
    public float jumpCooldown = 0.25f;

    private Rigidbody2D rb;

    // State checks
    private bool isGrounded = false;
    private bool isOnWall = false;
    private int wallSide = 0;
    private float moveDirection = 1f;

    // Timers
    private float wallTimer;
    private float jumpCooldownTimer = 0f;

    void Start()
    {
        // Get Rigidbody2D component from the player
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // reduce cooldown timer
        if (jumpCooldownTimer > 0f)
            jumpCooldownTimer -= Time.deltaTime;

        // Jump input
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // block jump if still in cooldown
            if (jumpCooldownTimer > 0f)
                return;

            // Wall jump
            if (isOnWall)
            {
                JumpToOtherWall();
                jumpCooldownTimer = jumpCooldown;
            }
            // Ground jump
            else if (isGrounded)
            {
                LaunchFromGround();
                jumpCooldownTimer = jumpCooldown;
            }
        }

        // Wall timer
        if (isOnWall)
        {
            wallTimer -= Time.deltaTime;

            // Force release when timer ends
            if (wallTimer <= 0f)
            {
                ReleaseFromWall();
            }
        }
    }

    void ReleaseFromWall()
    {
        // Stop wall state
        isOnWall = false;
        wallTimer = 0f;

        // Restore gravity and keep vertical velocity
        rb.gravityScale = 1;

    }

    void LaunchFromGround()
    {
        // Mark as airborne
        isGrounded = false;

        // Apply jump force
        rb.gravityScale = 1;
        rb.linearVelocity = new Vector2(jumpForceX, jumpForceY);
    }

    void JumpToOtherWall()
    {
        // Exit wall state
        isOnWall = false;

        // Restore gravity
        rb.gravityScale = 1;

        // Jump away from wall in opposite direction
        float direction = moveDirection;
        rb.linearVelocity = new Vector2(direction * jumpForceX, jumpForceY);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if player touched ground
        if (collision.gameObject.CompareTag("Ground") ||
            collision.gameObject.CompareTag("Platform"))
        {
            isGrounded = true;
        }

        // Check if player touched wall
        if (collision.gameObject.CompareTag("Wall"))
        {

            if (!isOnWall)
            {
                isOnWall = true;
                wallTimer = wallStickTime;

                transform.position += new Vector3(moveDirection * 0.1f, 0, 0);

                // flip movement direction
                moveDirection *= -1f;

                // Stick to wall (disable gravity and stop movement)
                rb.gravityScale = 1;
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // Left ground
        if (collision.gameObject.CompareTag("Ground") ||
            collision.gameObject.CompareTag("Platform"))
        {
            isGrounded = false;
        }

        // Left wall
        if (collision.gameObject.CompareTag("Wall"))
        {
            isOnWall = false;
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveDirection * runSpeed, rb.linearVelocity.y);
    }
}
