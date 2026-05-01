using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float jumpForceY = 7.5f;
    public float wallJumpForceX = 7.7f;
    public float runSpeed = 3.3f;

    [Header("Crouch")]
    public float crouchSpeedMultiplier = 0f; // fully stop movement

    [Header("Wall")]
    public float wallSlideSpeed = 1f;
    public float jumpCooldown = 0.25f;
    public float wallSlideMaxTime = 3f;
    public float wallJumpDuration = 0.3f;
    public float wallGrabDuration = 1f;
    public float moveDirection = 1f;

    private Rigidbody2D rb;

    private bool isGrounded = false;
    private bool isOnWall = false;
    private bool isWallJumping = false;
    private bool isCrouching = false;

    private float wallGrabTimer = 0f;
    private int wallSide = 0;

    private float groundBufferTime = 0.1f;
    private float groundBufferCounter = 0f;

    private float wallSlideTimer = 0f;
    private float wallJumpTimer = 0f;
    private float jumpCooldownTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (jumpCooldownTimer > 0f)
            jumpCooldownTimer -= Time.deltaTime;

        HandleInput();
    }

    void HandleInput()
    {
        bool jumpKey = Input.GetKeyDown(KeyCode.X) || Input.GetMouseButtonDown(0);
        bool crouchHeld = Input.GetKey(KeyCode.Z) || Input.GetMouseButton(1);

        if (crouchHeld)
        {
            if (isGrounded && !isOnWall && !isWallJumping)
            {
                isCrouching = true;
            }
        }
        else
        {
            isCrouching = false;
        }

        // ===== JUMP =====
        if (jumpKey)
        {
            if (jumpCooldownTimer > 0f) return;

            isCrouching = false; // cancel crouch on jump

            if (isOnWall)
            {
                WallJump();
                jumpCooldownTimer = jumpCooldown;
            }
            else if (isGrounded)
            {
                GroundJump();
                jumpCooldownTimer = jumpCooldown;
            }
        }
    }

    void FixedUpdate()
    {
        if (isGrounded)
        {
            groundBufferCounter = groundBufferTime;
        }
        else
        {
            groundBufferCounter -= Time.fixedDeltaTime;
            if (groundBufferCounter <= 0f)
                isGrounded = false;
        }

        if (wallJumpTimer > 0f)
        {
            wallJumpTimer -= Time.deltaTime;
            if (wallJumpTimer <= 0f)
                isWallJumping = false;
        }

        // ===== WALL =====
        if (isOnWall && !isGrounded)
        {
            isCrouching = false;

            if (wallGrabTimer > 0f)
            {
                wallGrabTimer -= Time.fixedDeltaTime;
                rb.gravityScale = 0f;
                rb.linearVelocity = Vector2.zero;
            }
            else
            {
                wallSlideTimer -= Time.fixedDeltaTime;

                if (wallSlideTimer <= 0f)
                {
                    isOnWall = false;
                    rb.gravityScale = 4f;
                }
                else
                {
                    rb.gravityScale = 0f;
                    rb.linearVelocity = new Vector2(0f, -wallSlideSpeed);
                }
            }
        }

        // ===== GROUND MOVEMENT =====
        else if (isGrounded)
        {
            isWallJumping = false;
            rb.gravityScale = 1f;

            if (isCrouching)
            {
                // freeze movement while crouching
                rb.linearVelocity = new Vector2(0f, 0f);
            }
            else
            {
                rb.linearVelocity = new Vector2(moveDirection * runSpeed, rb.linearVelocity.y);
            }
        }

        // ===== AIR =====
        else if (!isWallJumping)
        {
            if (rb.linearVelocity.y < 0f)
                rb.gravityScale = 3.3f;
            else
                rb.gravityScale = 2f;
        }
    }

    void GroundJump()
    {
        isGrounded = false;
        rb.gravityScale = 1f;
        rb.linearVelocity = new Vector2(moveDirection * runSpeed, jumpForceY);
    }

    void WallJump()
    {
        isOnWall = false;
        isWallJumping = true;
        wallJumpTimer = wallJumpDuration;
        wallSlideTimer = 0f;

        float direction = -wallSide;
        moveDirection = direction;

        rb.gravityScale = 1f;
        rb.linearVelocity = new Vector2(direction * wallJumpForceX, jumpForceY);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            Vector2 normal = contact.normal;

            if (normal.y > 0.5f)
            {
                if (collision.gameObject.CompareTag("Ground") ||
                    collision.gameObject.CompareTag("Platform"))
                {
                    isGrounded = true;
                    groundBufferCounter = groundBufferTime;
                }
            }

            if (Mathf.Abs(normal.x) > 0.5f)
            {
                if (collision.gameObject.CompareTag("Wall") ||
                    collision.gameObject.CompareTag("Platform"))
                {
                    isOnWall = true;
                    isWallJumping = false;
                    wallJumpTimer = 0f;
                    wallSide = normal.x > 0 ? -1 : 1;
                    moveDirection = -wallSide;
                    wallSlideTimer = wallSlideMaxTime;
                    wallGrabTimer = wallGrabDuration;
                }
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = false;

        if (collision.gameObject.CompareTag("Wall") ||
            collision.gameObject.CompareTag("Platform"))
            isOnWall = false;
    }
}