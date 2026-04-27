using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float jumpForceX = 8f;
    public float jumpForceY = 12f;
    public float runSpeed = 3f;

    private Rigidbody2D rb;

    private bool isGrounded = false;
    private bool isOnWall = false;
    private int wallSide = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                LaunchFromGround();
            }
            else if (isOnWall)
            {
                JumpToOtherWall();
            }
        }
    }

    void LaunchFromGround()
    {
        isGrounded = false;

        rb.gravityScale = 1;
        rb.linearVelocity = new Vector2(jumpForceX, jumpForceY);
    }

    void JumpToOtherWall()
    {
        isOnWall = false;

        rb.gravityScale = 1;

        float direction = -wallSide;
        rb.linearVelocity = new Vector2(direction * jumpForceX, jumpForceY);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }

        if (collision.gameObject.CompareTag("Wall"))
        {
            isOnWall = true;

            wallSide = (collision.transform.position.x < transform.position.x) ? -1 : 1;

            rb.gravityScale = 0;
            rb.linearVelocity = Vector2.zero;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }

        if (collision.gameObject.CompareTag("Wall"))
        {
            isOnWall = false;
        }
    }
}