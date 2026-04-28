using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Jump and movement settings
    public float jumpForceX = 8f;
    public float jumpForceY = 12f;
    public float runSpeed = 3f;

    // Wall behavior tuning (slide, cooldowns, feel)
    public float wallSlideSpeed = 1f;
    public float jumpCooldown = 0.25f;
    public float wallSlideMaxTime = 3f;
    public float wallJumpDuration = 0.3f;

    private Rigidbody2D rb;

    // State checking (movement conditions)
    private bool isGrounded = false;
    private bool isOnWall = false;
<<<<<<< Updated upstream
    private int wallSide = 0;
=======
    private bool isWallJumping = false;
>>>>>>> Stashed changes

    private float moveDirection = 1f;
    private int wallSide = 0;

    // Timers (control feel of movement)
    private float wallSlideTimer = 0f;
    private float wallJumpTimer = 0f;
    private float jumpCooldownTimer = 0f;

    void Start()
    {
        // Get Rigidbody2D component from the player
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Reduce cooldown timer
        if (jumpCooldownTimer > 0f)
            jumpCooldownTimer -= Time.deltaTime;

        // Handle jump input
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Block input if still in cooldown
            if (jumpCooldownTimer > 0f) return;

            // Wall jump
            if (isOnWall)
            {
                WallJump();
                jumpCooldownTimer = jumpCooldown;
            }
            // Ground jump
            else if (isGrounded)
            {
                GroundJump();
                jumpCooldownTimer = jumpCooldown;
            }
        }
    }

    void FixedUpdate()
    {
        // Handle wall jump lock timer (prevents instant re-wall stick)
        if (wallJumpTimer > 0f)
        {
            wallJumpTimer -= Time.deltaTime;

            if (wallJumpTimer <= 0f)
                isWallJumping = false;
        }

        // Wall sliding logic (controlled descent)
        if (isOnWall && !isGrounded)
        {
            wallSlideTimer -= Time.deltaTime;

            // Force release after max wall slide time
            if (wallSlideTimer <= 0f)
            {
                isOnWall = false;
                rb.gravityScale = 4f;
            }
            else
            {
                // Controlled slide down wall
                rb.gravityScale = 0f;
                rb.linearVelocity = new Vector2(0f, -wallSlideSpeed);
            }
        }
        // Ground movement (auto-run behavior)
        else if (isGrounded)
        {
            isWallJumping = false;
            rb.gravityScale = 1f;

            rb.linearVelocity = new Vector2(moveDirection * runSpeed, rb.linearVelocity.y);
        }
        // Air physics tuning (fall vs rise feel)
        else if (!isWallJumping)
        {
            if (rb.linearVelocity.y < 0f)
                rb.gravityScale = 3.3f; // faster fall
            else
                rb.gravityScale = 2f;   // softer jump rise
        }
    }

    void GroundJump()
    {
<<<<<<< Updated upstream
        // Stop wall state
        isOnWall = false;
        wallTimer = 0f;

        // Restore gravity and keep vertical velocity
        rb.gravityScale = 1;
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }

    void LaunchFromGround()
    {
        // Mark as airborne
=======
        // Exit grounded state
        rb.gravityScale = 1f;
>>>>>>> Stashed changes
        isGrounded = false;

        // Apply upward + forward jump
        rb.linearVelocity = new Vector2(moveDirection * runSpeed, jumpForceY);
    }

    void WallJump()
    {
        // Exit wall state and enter wall jump state
        isOnWall = false;
        isWallJumping = true;

        wallJumpTimer = wallJumpDuration;
        wallSlideTimer = 0f;

        rb.gravityScale = 1f;

        // Jump away from wall direction
        float direction = -wallSide;
        moveDirection = direction;

<<<<<<< Updated upstream
        // Jump away from wall in opposite direction
        float direction = -wallSide;
=======
>>>>>>> Stashed changes
        rb.linearVelocity = new Vector2(direction * jumpForceX, jumpForceY);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
<<<<<<< Updated upstream
        // Check if player touched ground
        if (collision.gameObject.CompareTag("Ground"))
=======
        foreach (ContactPoint2D contact in collision.contacts)
>>>>>>> Stashed changes
        {
            Vector2 normal = contact.normal;

<<<<<<< Updated upstream
        // Check if player touched wall
        if (collision.gameObject.CompareTag("Wall"))
        {
            isOnWall = true;
            wallTimer = wallStickTime;

            // Determine which side the wall is on
            wallSide = (collision.transform.position.x < transform.position.x) ? -1 : 1;

            // Stick to wall (disable gravity and stop movement)
            rb.gravityScale = 0;
            rb.linearVelocity = Vector2.zero;
=======
            // Ground / platform detection (top surface contact)
            if (normal.y > 0.5f)
            {
                if (collision.gameObject.CompareTag("Ground") ||
                    collision.gameObject.CompareTag("Platform"))
                {
                    isGrounded = true;
                }
            }

            // Wall detection (side contact only)
            if (Mathf.Abs(normal.x) > 0.5f)
            {
                if (collision.gameObject.CompareTag("Wall") ||
                    collision.gameObject.CompareTag("Platform"))
                {
                    isOnWall = true;
                    isWallJumping = false;
                    wallJumpTimer = 0f;

                    wallSide = normal.x > 0 ? -1 : 1;

                    // Start wall slide timer when airborne
                    wallSlideTimer = wallSlideMaxTime;
                }
            }
>>>>>>> Stashed changes
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
<<<<<<< Updated upstream
        // Left ground
        if (collision.gameObject.CompareTag("Ground"))
        {
=======
        // Leaving ground
        if (collision.gameObject.CompareTag("Ground"))
>>>>>>> Stashed changes
            isGrounded = false;

        // Leaving wall
        if (collision.gameObject.CompareTag("Wall"))
            isOnWall = false;

        // Leaving platform (re-evaluate contact state safely)
        if (collision.gameObject.CompareTag("Platform"))
        {
            Collider2D col = GetComponent<Collider2D>();
            ContactPoint2D[] contacts = new ContactPoint2D[10];
            int count = col.GetContacts(contacts);

            bool stillGrounded = false;
            bool stillOnWall = false;

            // Re-check remaining contacts
            for (int i = 0; i < count; i++)
            {
                Vector2 normal = contacts[i].normal;

                if (normal.y > 0.5f)
                    stillGrounded = true;

                if (Mathf.Abs(normal.x) > 0.5f)
                    stillOnWall = true;
            }

            isGrounded = stillGrounded;
            isOnWall = stillOnWall;
        }
    }
}