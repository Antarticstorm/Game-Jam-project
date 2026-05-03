using UnityEngine;

public class MouseFollow : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 220f;
    [SerializeField] private float deadZone = 2f;
    [SerializeField] private float boundsPadding = 30f;
    [SerializeField] private float crouchDelay = 0.2f;

    private Camera mainCam;
    private float leftBound;
    private float rightBound;
    private Animator anim;
    private float mouseStillTimer = 0f;

    private void Start()
    {
        mainCam = Camera.main;
        anim = GetComponent<Animator>();

        float camHeight = mainCam.orthographicSize;
        float camWidth = camHeight * mainCam.aspect;
        leftBound = mainCam.transform.position.x - camWidth + boundsPadding;
        rightBound = mainCam.transform.position.x + camWidth - boundsPadding;
    }

    private void Update()
    {
        Vector3 mouseWorld = mainCam.ScreenToWorldPoint(Input.mousePosition);
        float diff = mouseWorld.x - transform.position.x;

        bool hasReachedMouse = Mathf.Abs(diff) <= deadZone;

        if (!hasReachedMouse)
        {
            // Still chasing mouse — reset crouch timer
            mouseStillTimer = 0f;

            float direction = Mathf.Sign(diff);
            Vector3 pos = transform.position;
            pos.x += direction * moveSpeed * Time.deltaTime;
            pos.x = Mathf.Clamp(pos.x, leftBound, rightBound);
            transform.position = pos;

            // Flip sprite
            transform.localScale = new Vector3(
                direction > 0 ? Mathf.Abs(transform.localScale.x) : -Mathf.Abs(transform.localScale.x),
                transform.localScale.y,
                transform.localScale.z
            );

            if (anim != null)
            {
                anim.SetFloat("Magnitude", moveSpeed);
                anim.SetBool("IsCrouching", false);
                anim.SetBool("IsGrounded", true);
            }
        }
        else
        {
            // Caught up to mouse — count timer then crouch
            mouseStillTimer += Time.deltaTime;

            if (anim != null)
            {
                anim.SetFloat("Magnitude", 0f);
                anim.SetBool("IsGrounded", true);
                anim.SetBool("IsCrouching", mouseStillTimer >= crouchDelay);
            }
        }
    }
}