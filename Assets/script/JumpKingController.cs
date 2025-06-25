using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;               // Tốc độ di chuyển của nhân vật
    public float maxJumpHeight = 10f;          // Độ cao nhảy tối đa
    public float jumpChargeTime = 1f;          // Thời gian tối đa để tích tụ lực nhảy
    public float gravity = 20f;                // Trọng lực

    private float currentJumpHeight = 0f;      // Lực nhảy hiện tại
    private float jumpStartTime;               // Thời gian bắt đầu tích tụ lực nhảy
    private bool isJumping = false;            // Kiểm tra trạng thái nhảy
    private bool isGrounded = false;           // Kiểm tra nhân vật có chạm đất không

    private Rigidbody2D rb;
    private Animator animator;                 // Animator để kiểm soát các animation
    private SpriteRenderer spriteRenderer;     // Để xử lý lật nhân vật

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();      // Lấy component Rigidbody2D
        animator = GetComponent<Animator>();   // Lấy Animator
        spriteRenderer = GetComponent<SpriteRenderer>();  // Lấy SpriteRenderer
    }

    void Update()
    {
        // Nếu nhân vật đang ở trên mặt đất và không phải nhảy
        if (isGrounded && !isJumping)
        {
            if (Input.GetButtonDown("Jump"))
            {
                jumpStartTime = Time.time;
                isJumping = true;
                animator.SetTrigger("Jump");  // Bắt đầu animation nhảy
            }
        }

        // Tích tụ lực nhảy
        if (isJumping)
        {
            ChargeJump();
        }

        // Nhảy
        if (!isGrounded)
        {
            ApplyGravity();
        }

        // Di chuyển nhân vật theo chiều ngang
        Move();

        // Cập nhật trạng thái animation
        UpdateAnimations();
    }

    void Move()
    {
        float moveInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        // Nếu nhân vật di chuyển, chuyển sang animation "Run", nếu không thì "Idle"
        if (moveInput != 0)
        {
            animator.SetBool("IsRunning", true);  // Chạy
            if (moveInput > 0)  // Di chuyển sang phải
            {
                spriteRenderer.flipX = false;  // Không lật
            }
            else if (moveInput < 0)  // Di chuyển sang trái
            {
                spriteRenderer.flipX = true;  // Lật nhân vật khi di chuyển trái
            }
        }
        else
        {
            animator.SetBool("IsRunning", false); // Đứng yên (Idle)
        }
    }

    void ChargeJump()
    {
        // Tính toán lực nhảy dựa trên thời gian giữ nút
        if (Time.time - jumpStartTime < jumpChargeTime)
        {
currentJumpHeight = Mathf.Lerp(0, maxJumpHeight, (Time.time - jumpStartTime) / jumpChargeTime);
        }

        // Khi thả nút, bắt đầu nhảy
        if (Input.GetButtonUp("Jump"))
        {
            Jump();
        }
    }

    void Jump()
    {
        // Áp dụng lực nhảy cho nhân vật
        rb.velocity = new Vector2(rb.velocity.x, currentJumpHeight);
        currentJumpHeight = 0f;  // Reset lực nhảy
        isJumping = false;       // Đánh dấu kết thúc quá trình nhảy
    }

    void ApplyGravity()
    {
        // Áp dụng trọng lực
        if (rb.velocity.y > 0)
        {
            rb.velocity += Vector2.up * -gravity * Time.deltaTime;
        }
    }

    void UpdateAnimations()
    {
        // Khi nhân vật chạm đất, chuyển sang trạng thái đứng (Idle)
        if (isGrounded && !isJumping)
        {
            animator.SetBool("IsJumping", false);  // Đang đứng trên mặt đất
        }

        // Kiểm tra xem nhân vật có đang nhảy hay không
        if (isJumping)
        {
            animator.SetBool("IsJumping", true);  // Đang nhảy
        }

        // Nếu không phải nhảy và không phải đang di chuyển, thì nhân vật đang đứng yên
        if (isGrounded && !isJumping && rb.velocity.x == 0)
        {
            animator.SetBool("IsRunning", false);  // Không chạy khi đứng yên
        }
    }

    // Kiểm tra va chạm với mặt đất thông qua tag "Ground"
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = true;  // Chạm đất
        }
    }

    // Kiểm tra khi không còn chạm đất
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = false;  // Không còn chạm đất
        }
    }
}
