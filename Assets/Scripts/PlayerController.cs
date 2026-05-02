using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float jumpForceY = 7.5f;
    public float wallJumpForceX = 7.7f;
    public float runSpeed = 3.3f;

    [Header("Crouch")]
    public float crouchSpeedMultiplier = 0f;

    [Header("Wall")]
    public float wallSlideSpeed = 1f;
    public float jumpCooldown = 0.25f;
    public float wallSlideMaxTime = 3f;
    public float wallJumpDuration = 0.3f;
    public float wallGrabDuration = 1f;
    public float moveDirection = 1f;

    public Animator animator;

    private Rigidbody2D rb;

    [SerializeField] private SpriteRenderer sr;

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
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (jumpCooldownTimer > 0f)
            jumpCooldownTimer -= Time.deltaTime;

        HandleInput();

        float yVel = rb.linearVelocity.y;
        bool isWallGrabbing = isOnWall && !isGrounded && wallGrabTimer > 0f;
        bool isWallSliding = isOnWall && !isGrounded && !isWallGrabbing;

        animator.SetFloat("YVelocity", yVel);
        animator.SetFloat("Magnitude", Mathf.Abs(rb.linearVelocity.x));
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetBool("IsWallSliding", isOnWall && !isGrounded && rb.linearVelocity.y < 0f);
        animator.SetBool("IsOnWall", isOnWall && !isGrounded);
        animator.SetBool("IsCrouching", isCrouching);

        //FLIP LOGIC HERE
        if (isWallSliding || isWallGrabbing)
        {
           
            sr.flipX = (wallSide < 0); 
        }
        else if (moveDirection > 0)
            sr.flipX = false;
        else if (moveDirection < 0)
            sr.flipX = true;

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

        Vector2 vel = rb.linearVelocity;

        // ===== WALL =====
        if (isOnWall && !isGrounded)
        {
            isCrouching = false;

            if (wallGrabTimer > 0f)
            {
                wallGrabTimer -= Time.fixedDeltaTime;
                wallGrabTimer = Mathf.Max(0f, wallGrabTimer);
                rb.gravityScale = 0f;
                rb.linearVelocity = Vector2.zero;
                return;
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
                    return;
                }
            }
        }

        if (isGrounded)
        {
            isWallJumping = false;
            rb.gravityScale = 1f;

            if (isCrouching)
            {
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            }
            else
            {
                rb.linearVelocity = new Vector2(moveDirection * runSpeed, rb.linearVelocity.y);
            }
        }
        else if (!isWallJumping)
        {
            // AIR CONTROL (THIS WAS MISSING)
            float airControl = 0.8f; // tweak 0–1

            rb.linearVelocity = new Vector2(
                moveDirection * runSpeed * airControl,
                rb.linearVelocity.y
            );

            rb.gravityScale = (rb.linearVelocity.y < 0f) ? 3.3f : 2f;
        }
    }

    public void ForceExternalVelocity(Vector2 velocity)
    {
        rb.linearVelocity = velocity;

        isOnWall = false;
        isWallJumping = false;
        wallGrabTimer = 0f;
        isGrounded = false;

        animator.SetTrigger("Jump");
        animator.SetBool("IsGrounded", false);
    }

    public void ResetWallState()
    {
        isOnWall = false;
        isWallJumping = false;
        wallGrabTimer = 0f;
    }

    void GroundJump()
    {
        isGrounded = false;
        rb.linearVelocity = new Vector2(moveDirection * runSpeed, jumpForceY);

        animator.SetTrigger("Jump");
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

        animator.SetTrigger("Jump");
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