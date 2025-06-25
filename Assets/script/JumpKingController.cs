using UnityEngine;

public class JumpKingController : MonoBehaviour
{
    [Header("Move Settings")]
    public float moveSpeed = 3f;
    public float maxChargeTime = 1.5f;
    public float maxJumpForce = 12f;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;

    private Rigidbody2D rb;
    private bool isGrounded;
    private float chargeTime;
    private bool isCharging;
    private bool jumpReady;

    private SpriteRenderer sprite;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        CheckGrounded();
        HandleInput();
        FlipCharacter();
    }

    void FixedUpdate()
    {
        if (jumpReady)
        {
            Jump();
        }
    }

    void HandleInput()
    {
        float move = Input.GetAxisRaw("Horizontal");

        // Di chuyển trái/phải khi đang ở dưới đất và không charge
        if (isGrounded && !isCharging)
        {
            rb.velocity = new Vector2(move * moveSpeed, rb.velocity.y);
        }

        // Nhấn giữ để bắt đầu tích lực
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            isCharging = true;
            chargeTime = 0f;
        }

        // Trong khi giữ thì tăng dần lực
        if (isCharging)
        {
            chargeTime += Time.deltaTime;
            chargeTime = Mathf.Clamp(chargeTime, 0f, maxChargeTime);
        }

        // Nhả phím -> nhảy
        if (Input.GetKeyUp(KeyCode.Space) && isCharging)
        {
            isCharging = false;
            jumpReady = true;
        }
    }

    void Jump()
    {
        float chargeRatio = chargeTime / maxChargeTime;
        float force = chargeRatio * maxJumpForce;

        // Nhảy xiên hướng người chơi đang nhấn
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        Vector2 jumpDirection = new Vector2(horizontalInput, 1f).normalized;

        rb.velocity = Vector2.zero;
        rb.AddForce(jumpDirection * force, ForceMode2D.Impulse);

        jumpReady = false;
    }

    void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    void FlipCharacter()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        if (horizontalInput > 0.1f)
            sprite.flipX = false;
        else if (horizontalInput < -0.1f)
            sprite.flipX = true;
    }
}
