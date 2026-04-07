using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Move")]
    public float moveSpeed = 7f;

    [Header("Jump")]
    public float jumpForce = 12f;
    public int maxJumps = 2;
    public float wallJumpForceX = 8f;
    public float wallJumpForceY = 12f;
    public float wallJumpInputLockTime = 0.15f;

    [Header("Dash")]
    public float dashSpeed = 16f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 0.5f;

    [Header("Raycasts")]
    public LayerMask groundMask;
    public float groundRayLength = 0.15f;
    public float wallRayLength = 0.15f;

    [Header("Camera")]
    public Transform cameraTransform;
    public Vector3 cameraOffset = new Vector3(0f, 0f, -10f);
    public float cameraSmoothSpeed = 10f;

    [Header("Audio")]
    public AudioSource moveLoopSource;
    public AudioSource sfxSource;
    public AudioClip walkClip;
    public AudioClip jumpClip;

    private Rigidbody2D rb;
    private Collider2D col;

    private float moveInput;
    private bool facingRight = true;

    private bool isGrounded;
    private bool touchingWallLeft;
    private bool touchingWallRight;
    private bool touchingWall;

    private int jumpsUsed;
    private bool isDashing;
    private bool canDash = true;

    private bool wallJumpInputLocked;
    private float lockedMoveDirection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        rb.freezeRotation = true;

        if (moveLoopSource != null)
        {
            moveLoopSource.loop = true;
            moveLoopSource.playOnAwake = false;
            moveLoopSource.clip = walkClip;
        }

        if (sfxSource != null)
        {
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
        }
    }

    private void Update()
    {
        float rawInput = 0f;
        if (Input.GetKey(KeyCode.LeftArrow)) rawInput = -1f;
        if (Input.GetKey(KeyCode.RightArrow)) rawInput = 1f;

        moveInput = wallJumpInputLocked ? lockedMoveDirection : rawInput;

        CheckSurroundings();

        if (isGrounded)
            jumpsUsed = 0;

        if (moveInput > 0f && !facingRight) Flip();
        else if (moveInput < 0f && facingRight) Flip();

        if (Input.GetKeyDown(KeyCode.UpArrow))
            Jump();

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && !isDashing)
            StartCoroutine(Dash());
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            UpdateWalkAudio();
            return;
        }

        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        UpdateWalkAudio();
    }

    private void LateUpdate()
    {
        FollowCamera();
    }

    private void Jump()
    {
        if (!isGrounded && touchingWall)
        {
            float dir = touchingWallLeft ? 1f : -1f;
            rb.linearVelocity = new Vector2(dir * wallJumpForceX, wallJumpForceY);

            if (dir > 0f && !facingRight) Flip();
            else if (dir < 0f && facingRight) Flip();

            jumpsUsed = 1;
            PlayOneShot(jumpClip);
            StartCoroutine(LockWallJumpInput(dir));
            return;
        }

        if (jumpsUsed < maxJumps)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpsUsed++;
            PlayOneShot(jumpClip);
        }
    }

    private IEnumerator LockWallJumpInput(float dir)
    {
        wallJumpInputLocked = true;
        lockedMoveDirection = dir;

        yield return new WaitForSeconds(wallJumpInputLockTime);

        wallJumpInputLocked = false;
        lockedMoveDirection = 0f;
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        float dashDir = moveInput != 0f ? Mathf.Sign(moveInput) : (facingRight ? 1f : -1f);
        rb.linearVelocity = new Vector2(dashDir * dashSpeed, 0f);

        yield return new WaitForSeconds(dashDuration);

        rb.gravityScale = originalGravity;
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private void CheckSurroundings()
    {
        Bounds b = col.bounds;

        Vector2 groundOrigin = new Vector2(b.center.x, b.min.y);
        Vector2 leftOrigin = new Vector2(b.min.x, b.center.y);
        Vector2 rightOrigin = new Vector2(b.max.x, b.center.y);

        isGrounded = Physics2D.Raycast(groundOrigin, Vector2.down, groundRayLength, groundMask);
        touchingWallLeft = Physics2D.Raycast(leftOrigin, Vector2.left, wallRayLength, groundMask);
        touchingWallRight = Physics2D.Raycast(rightOrigin, Vector2.right, wallRayLength, groundMask);
        touchingWall = touchingWallLeft || touchingWallRight;
    }

    private void FollowCamera()
    {
        if (cameraTransform == null) return;

        Vector3 targetPosition = transform.position + cameraOffset;
        cameraTransform.position = Vector3.Lerp(
            cameraTransform.position,
            targetPosition,
            cameraSmoothSpeed * Time.deltaTime
        );
    }

    private void UpdateWalkAudio()
    {
        if (moveLoopSource == null || walkClip == null) return;

        bool shouldPlay = isGrounded && Mathf.Abs(rb.linearVelocity.x) > 0.1f && !isDashing;

        if (shouldPlay)
        {
            if (!moveLoopSource.isPlaying)
                moveLoopSource.Play();
        }
        else
        {
            if (moveLoopSource.isPlaying)
                moveLoopSource.Stop();
        }
    }

    private void PlayOneShot(AudioClip clip)
    {
        if (sfxSource == null || clip == null) return;
        sfxSource.PlayOneShot(clip);
    }

    private void Flip()
    {
        facingRight = !facingRight;

        Vector3 scale = transform.localScale;
        scale.x *= -1f;
        transform.localScale = scale;
    }

    private void OnDisable()
    {
        if (moveLoopSource != null && moveLoopSource.isPlaying)
            moveLoopSource.Stop();
    }

    private void OnDrawGizmosSelected()
    {
        Collider2D c = GetComponent<Collider2D>();
        if (c == null) return;

        Bounds b = c.bounds;

        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector2(b.center.x, b.min.y), new Vector2(b.center.x, b.min.y) + Vector2.down * groundRayLength);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector2(b.min.x, b.center.y), new Vector2(b.min.x, b.center.y) + Vector2.left * wallRayLength);
        Gizmos.DrawLine(new Vector2(b.max.x, b.center.y), new Vector2(b.max.x, b.center.y) + Vector2.right * wallRayLength);
    }
}