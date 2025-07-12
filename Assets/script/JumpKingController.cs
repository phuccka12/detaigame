using UnityEngine;
using System.Collections;
public class JumpKingController : PersistentSingleton<JumpKingController>
{
    [Header("Power-ups")]
    public bool hasShield = false;
    public float shieldDuration = 10f; // Khiên có hiệu lực trong 5 giây

    public float moveSpeed = 5f;
    public float maxJumpHeight = 10f;
    public float jumpChargeTime = 1f;
    public float gravity = 20f;

    private float currentJumpHeight = 0f;
    private float jumpStartTime;
    private bool isJumping = false;
    private bool isGrounded = false;
    public bool isBlocking = false;

    private float attack1Cooldown = 0.5f;
    private float attack2Cooldown = 1.5f;
    private float lastAttack1Time = 0f;
    private float lastAttack2Time = 0f;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Vector3 spawnPosition;
    public AudioClip jumpSound;
    // CẬP NHẬT: Chuyển việc lấy component vào Awake()
    protected override void Awake()
    {
        base.Awake(); // Gọi hàm Awake của lớp cha (PersistentSingleton)

        // Lấy các component ở đây để đảm bảo chúng sẵn sàng sớm nhất có thể
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        // Chỉ giữ lại logic cần chạy sau khi mọi thứ đã Awake
        spawnPosition = transform.position;
    }

    void Update()
    {
        HandleMovement();
        HandleJumping();
        HandleBlocking();
        HandleAttackInput();
        UpdateAnimations();
    }

    // --- Toàn bộ logic còn lại của bạn được giữ nguyên 100% ---

    void HandleMovement()
    {
        float moveInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        if (moveInput != 0)
        {
            animator.SetBool("IsRunning", true);
            if (moveInput > 0)
            {
                spriteRenderer.flipX = false;
            }
            else if (moveInput < 0)
            {
                spriteRenderer.flipX = true;
            }
        }
        else
        {
            animator.SetBool("IsRunning", false);
        }
    }

    void HandleJumping()
    {
        if (isGrounded && !isJumping && Input.GetButtonDown("Jump"))
        {
            jumpStartTime = Time.time;
            isJumping = true;
            animator.SetTrigger("Jump");
        }
        if (isJumping)
        {
            ChargeJump();
        }
        if (!isGrounded)
        {
            ApplyGravity();
        }
    }

    void ChargeJump()
    {
        if (Time.time - jumpStartTime < jumpChargeTime)
        {
            currentJumpHeight = Mathf.Lerp(0, maxJumpHeight, (Time.time - jumpStartTime) / jumpChargeTime);
        }
        if (Input.GetButtonUp("Jump"))
        {
            Jump();
        }
    }

    void Jump()
    {
        if (AudioManager.instance != null && jumpSound != null)
        {
            // Ra lệnh cho AudioManager phát hiệu ứng âm thanh nhảy
            AudioManager.instance.PlaySFX(jumpSound);
        }

        rb.velocity = new Vector2(rb.velocity.x, currentJumpHeight);
        currentJumpHeight = 0f;
        isJumping = false;
    }

    void ApplyGravity()
    {
        if (rb.velocity.y > 0)
        {
            rb.velocity += Vector2.up * -gravity * Time.deltaTime;
        }
    }

    void UpdateAnimations()
    {
        if (isGrounded && !isJumping)
        {
            animator.SetBool("IsJumping", false);
        }
        if (isJumping)
        {
            animator.SetBool("IsJumping", true);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
        if (collision.gameObject.CompareTag("Bee") && !isBlocking)
        {
            Debug.Log("Player hit by Bee!");
            if (GameManager.instance != null)
            {
                GameManager.instance.LoseLife();
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    public void TakeDamage()
    {
        // NẾU CÓ KHIÊN, bỏ qua sát thương
        if (hasShield)
        {
            Debug.Log("Khiên đã chặn một đòn tấn công!");
            return; // Thoát khỏi hàm, không bị mất mạng
        }

        // Nếu không có khiên, mất mạng như bình thường (logic cũ của bạn)
        if (GameManager.instance != null)
        {
            GameManager.instance.LoseLife();
        }
    }

    public Vector3 GetSpawnPosition()
    {
        return spawnPosition;
    }

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

    void Attack1()
    {
        animator.SetTrigger("Attack1");
        lastAttack1Time = Time.time;
    }

    void Attack2()
    {
        animator.SetTrigger("Attack2");
        lastAttack2Time = Time.time;
    }

    void HandleBlocking()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            isBlocking = true;
            animator.SetBool("isBlocking", true);
        }
        if (Input.GetKeyUp(KeyCode.Q))
        {
            isBlocking = false;
            animator.SetBool("isBlocking", false);
        }
    }

    public void SetNewSpawnPosition(Vector3 newPosition)
    {
        spawnPosition = newPosition;
    }
    public void ActivateShield()
    {
        // Dừng coroutine cũ (nếu có) và bắt đầu một cái mới để reset thời gian
        StopCoroutine("ShieldCoroutine");
        StartCoroutine("ShieldCoroutine");
    }

    private IEnumerator ShieldCoroutine()
    {
        hasShield = true; // Bật trạng thái có khiên
        Debug.Log("Khiên được kích hoạt! Bất tử trong " + shieldDuration + " giây.");

        // Bạn có thể thêm hiệu ứng hình ảnh cho khiên ở đây

        // Đợi hết thời gian hiệu lực
        yield return new WaitForSeconds(shieldDuration);

        hasShield = false; // Tắt trạng thái có khiên
        Debug.Log("Khiên đã hết hiệu lực.");

        // Tắt hiệu ứng hình ảnh của khiên ở đây
    }
}