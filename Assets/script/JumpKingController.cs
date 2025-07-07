using UnityEngine;
public class JumpKingController : PersistentSingleton<JumpKingController>
{
    public float moveSpeed = 5f;               // Tốc độ di chuyển của nhân vật
    public float maxJumpHeight = 10f;          // Độ cao nhảy tối đa
    public float jumpChargeTime = 1f;          // Thời gian tối đa để tích tụ lực nhảy
    public float gravity = 20f;                // Trọng lực

    private float currentJumpHeight = 0f;      // Lực nhảy hiện tại
    private float jumpStartTime;               // Thời gian bắt đầu tích tụ lực nhảy
    private bool isJumping = false;            // Kiểm tra trạng thái nhảy
    private bool isGrounded = false;           // Kiểm tra nhân vật có chạm đất không
    public bool isBlocking = false;           // Kiểm tra trạng thái thủ

    // Khai báo các biến cooldown và thời gian cho các đòn tấn công
    private float attack1Cooldown = 0.5f;      // Cooldown cho đòn đánh 1
    private float attack2Cooldown = 1.5f;      // Cooldown cho đòn đánh 2
    private float lastAttack1Time = 0f;        // Thời gian thực hiện đòn đánh 1
    private float lastAttack2Time = 0f;        // Thời gian thực hiện đòn đánh 2

    private Rigidbody2D rb;
    private Animator animator;                 // Animator để kiểm soát các animation
    private SpriteRenderer spriteRenderer;     // Để xử lý lật nhân vật
    private Vector3 spawnPosition;             // Lưu vị trí hồi sinh

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();      // Lấy component Rigidbody2D
        animator = GetComponent<Animator>();   // Lấy Animator
        spriteRenderer = GetComponent<SpriteRenderer>();  // Lấy SpriteRenderer

        // Lưu vị trí spawn (Vị trí ban đầu của nhân vật)
        spawnPosition = transform.position;
    }

    void Update()
    {
        HandleMovement();
        HandleJumping();
        HandleBlocking();  // Kiểm tra thủ
        HandleAttackInput();
        UpdateAnimations();
    }

    // Di chuyển nhân vật
    void HandleMovement()
    {
        float moveInput = Input.GetAxis("Horizontal"); // Lấy input di chuyển ngang
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y); // Di chuyển theo chiều ngang

        // Cập nhật animation khi di chuyển
        if (moveInput != 0)
        {
            animator.SetBool("IsRunning", true); // Chạy
            if (moveInput > 0)
            {
                spriteRenderer.flipX = false; // Không lật khi di chuyển sang phải
            }
            else if (moveInput < 0)
            {
                spriteRenderer.flipX = true; // Lật nhân vật khi di chuyển sang trái
            }
        }
        else
        {
            animator.SetBool("IsRunning", false); // Đứng yên (Idle)
        }
    }

    // Kiểm tra nhảy
    void HandleJumping()
    {
        if (isGrounded && !isJumping && Input.GetButtonDown("Jump"))
        {
            jumpStartTime = Time.time; // Lưu thời gian bắt đầu nhảy
            isJumping = true;
            animator.SetTrigger("Jump");  // Bắt đầu animation nhảy
        }

        // Tích tụ lực nhảy
        if (isJumping)
        {
            ChargeJump();
        }

        // Nếu không phải đang nhảy, áp dụng trọng lực
        if (!isGrounded)
        {
            ApplyGravity();
        }
    }

    // Tính toán lực nhảy dựa trên thời gian giữ nút nhảy
    void ChargeJump()
    {
        if (Time.time - jumpStartTime < jumpChargeTime)
        {
            currentJumpHeight = Mathf.Lerp(0, maxJumpHeight, (Time.time - jumpStartTime) / jumpChargeTime); // Lực nhảy theo thời gian
        }

        // Khi người chơi thả nút nhảy
        if (Input.GetButtonUp("Jump"))
        {
            Jump();
        }
    }

    // Thực hiện nhảy
    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, currentJumpHeight); // Áp dụng lực nhảy cho nhân vật
        currentJumpHeight = 0f;  // Reset lực nhảy
        isJumping = false;       // Đánh dấu kết thúc quá trình nhảy
    }

    // Áp dụng trọng lực cho nhân vật
    void ApplyGravity()
    {
        if (rb.velocity.y > 0)
        {
            rb.velocity += Vector2.up * -gravity * Time.deltaTime; // Trọng lực tác động
        }
    }

    // Cập nhật các animation
    void UpdateAnimations()
    {
        if (isGrounded && !isJumping)
        {
            animator.SetBool("IsJumping", false); // Đang đứng trên mặt đất
        }

        if (isJumping)
        {
            animator.SetBool("IsJumping", true);  // Đang nhảy
        }
    }

    // Kiểm tra va chạm với mặt đất
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;  // Nhân vật đã chạm đất
        }

        if (collision.gameObject.CompareTag("Bee") && !isBlocking)
        {
            Debug.Log("Player hit by Bee!");
            GameManager gameManager = FindObjectOfType<GameManager>(); // Lấy GameManager
            if (gameManager != null)
            {
                gameManager.LoseLife();  // Gọi LoseLife khi va chạm với con ong
            }
        }
    }

    // Kiểm tra khi không còn chạm đất
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = false;  // Không còn chạm đất nữa
        }
    }

    // Hàm mất mạng
    public void TakeDamage()
    {
        GameManager gameManager = FindObjectOfType<GameManager>();  // Tìm GameManager trong scene
        if (gameManager != null)
        {
            gameManager.LoseLife();  // Mất mạng
            gameManager.RespawnPlayer();  // Hồi sinh nhân vật
        }
    }

    // Lấy vị trí spawn (để dùng cho Respawn)
    public Vector3 GetSpawnPosition()
    {
        return spawnPosition;
    }

    // Kiểm tra các phím tấn công
    void HandleAttackInput()
    {
        if (Input.GetKeyDown(KeyCode.F) && Time.time >= lastAttack1Time + attack1Cooldown)
        {
            Attack1();
        }

        if (Input.GetKeyDown(KeyCode.G) && Time.time >= lastAttack2Time + attack2Cooldown)
        {
            Attack2();
        }
    }

    // Đòn đánh 1
    void Attack1()
    {
        animator.SetTrigger("Attack1");  // Gọi animation đòn đánh 1
        lastAttack1Time = Time.time;  // Cập nhật thời gian thực hiện đòn đánh 1
    }

    // Đòn đánh 2
    void Attack2()
    {
        animator.SetTrigger("Attack2");  // Gọi animation đòn đánh 2
        lastAttack2Time = Time.time;  // Cập nhật thời gian thực hiện đòn đánh 2
    }

    // Kiểm tra trạng thái thủ
    void HandleBlocking()
    {
        if (Input.GetKeyDown(KeyCode.Q))  // Khi nhấn phím Q
        {
            isBlocking = true;  // Kích hoạt trạng thái thủ
            animator.SetBool("isBlocking", true);  // Bật animation thủ
        }

        if (Input.GetKeyUp(KeyCode.Q))  // Khi thả phím Q
        {
            isBlocking = false;  // Tắt trạng thái thủ
            animator.SetBool("isBlocking", false);  // Tắt animation thủ
        }
    }
    // Thêm hàm này vào bất cứ đâu bên trong class JumpKingController
    public void SetNewSpawnPosition(Vector3 newPosition)
    {
        spawnPosition = newPosition;
    }
    // Thêm hàm này vào bất cứ đâu bên trong class JumpKingController
  
}
