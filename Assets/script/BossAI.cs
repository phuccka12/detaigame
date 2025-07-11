using UnityEngine;

public class BossAI_2D : MonoBehaviour
{
    [Header("References")]
    public Transform player; // Kéo object của Player vào đây
    private Animator animator;
    private Rigidbody2D rb;

    [Header("Stats")]
    public float moveSpeed = 3f; // Tốc độ di chuyển của boss
    public int maxHealth = 100;
    private int currentHealth;

    [Header("AI Behavior")]
    public float lookRadius = 15f; // Tầm nhìn của boss
    public float attackRange = 3f; // Tầm đánh của boss
    public float attackCooldown = 2f; // Thời gian chờ giữa các đòn đánh
    private float lastAttackTime = 0f;
    private bool isFlipped = false;

    void Start()
    {
        // Tìm Player tự động nếu chưa gán
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (currentHealth <= 0)
        {
            // Nếu boss đã chết, ngừng di chuyển
            rb.velocity = Vector2.zero;
            animator.SetBool("IsChasing", false);
            return;
        }

        float distance = Vector2.Distance(player.position, transform.position);

        if (distance <= lookRadius)
        {
            // Quay mặt về phía người chơi
            LookAtPlayer();

            if (distance <= attackRange)
            {
                // Ở trong tầm đánh: Dừng lại và tấn công
                rb.velocity = Vector2.zero; // Dừng di chuyển
                animator.SetBool("IsChasing", false);

                if (Time.time >= lastAttackTime + attackCooldown)
                {
                    Attack();
                    lastAttackTime = Time.time;
                }
            }
            else
            {
                // Ngoài tầm đánh nhưng trong tầm nhìn: Đuổi theo
                Vector2 direction = (player.position - transform.position).normalized;
                rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
                animator.SetBool("IsChasing", true);
            }
        }
        else
        {
            // Ngoài tầm nhìn: Dừng lại
            rb.velocity = Vector2.zero;
            animator.SetBool("IsChasing", false);
        }
    }

    // Hàm để lật sprite của boss qua lại
    void LookAtPlayer()
    {
        Vector3 flipped = transform.localScale;
        flipped.z *= -1f;

        if (transform.position.x > player.position.x && isFlipped)
        {
            transform.localScale = flipped;
            isFlipped = false;
        }
        else if (transform.position.x < player.position.x && !isFlipped)
        {
            transform.localScale = flipped;
            isFlipped = true;
        }
    }

    void Attack()
    {
        animator.SetTrigger("Attack");
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;
        animator.SetTrigger("TakeHit");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Boss đã bị tiêu diệt!");
        animator.SetBool("IsDead", true);

        // Vô hiệu hóa các component
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
    }

    // Vẽ các vòng tròn để dễ căn chỉnh trong Editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    // Hàm DealDamage vẫn giữ nguyên để Animation Event gọi
    public void DealDamage()
    {
        float distance = Vector2.Distance(player.position, transform.position);
        if (distance <= attackRange)
        {
            Debug.Log("Boss đã đánh trúng Player!");
            // Gọi hàm nhận sát thương trên script của Player
            // PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            // if (playerHealth != null)
            // {
            //     playerHealth.TakeDamage(20);
            // }
        }
    }
}