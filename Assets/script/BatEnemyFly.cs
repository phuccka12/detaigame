using UnityEngine;

public class BatController : MonoBehaviour
{
    [Header("Patrol Settings")]
    public float patrolSpeed = 2f;        // Tốc độ di chuyển tuần tra
    public float moveRange = 4f;          // Phạm vi bay qua lại

    [Header("Chase Settings")]
    public float detectionRange = 5f;     // Phạm vi phát hiện người chơi
    public float chaseSpeed = 4f;         // Tốc độ đuổi theo
    public float returnSpeed = 3f;        // Tốc độ quay về

    // Biến nội bộ
    private Vector3 patrolCenter;
    private bool movingRight = true;
    private bool chasing = false;
    private bool returning = false;
    private Transform player;
    private Animator animator;

    void Start()
    {
        // Lưu lại vị trí ban đầu làm trung tâm tuần tra
        patrolCenter = transform.position;
        animator = GetComponent<Animator>();

        // Thiết lập Rigidbody để không bị ảnh hưởng bởi trọng lực
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 0;
            rb.freezeRotation = true;
        }
    }

    void Update()
    {
        // Tìm người chơi một cách an toàn
        if (player == null)
        {
            if (JumpKingController.instance != null)
                player = JumpKingController.instance.transform;
            else
                return; // Nếu không có người chơi, không làm gì cả
        }

        // Logic chuyển trạng thái
        float distToPlayer = Vector2.Distance(transform.position, player.position);

        if (!chasing && !returning && distToPlayer < detectionRange)
        {
            chasing = true;
            animator?.SetBool("IsChasing", true);
        }
        else if (chasing && distToPlayer > detectionRange * 1.5f) // Thêm khoảng đệm để tránh dơi đổi ý liên tục
        {
            chasing = false;
            returning = true;
            animator?.SetBool("IsChasing", false);
        }

        // Thực thi hành động dựa trên trạng thái
        if (chasing)
        {
            ChasePlayer();
        }
        else if (returning)
        {
            ReturnToPatrolCenter();
        }
        else
        {
            Patrol();
        }
    }

    void Patrol()
    {
        // Tuần tra qua lại quanh điểm trung tâm
        if (movingRight)
        {
            transform.position = Vector2.MoveTowards(transform.position, patrolCenter + Vector3.right * moveRange, patrolSpeed * Time.deltaTime);
            if (transform.position.x >= patrolCenter.x + moveRange)
            {
                movingRight = false;
            }
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, patrolCenter + Vector3.left * moveRange, patrolSpeed * Time.deltaTime);
            if (transform.position.x <= patrolCenter.x - moveRange)
            {
                movingRight = true;
            }
        }
        // Luôn lật mặt về phía trung tâm khi tuần tra
        FlipToFace(patrolCenter.x);
    }

    void ChasePlayer()
    {
        transform.position = Vector2.MoveTowards(transform.position, player.position, chaseSpeed * Time.deltaTime);
        FlipToFace(player.position.x);

        if (Vector2.Distance(transform.position, player.position) < 1f)
        {
            AttackPlayer();
        }
    }

    void ReturnToPatrolCenter()
    {
        transform.position = Vector2.MoveTowards(transform.position, patrolCenter, returnSpeed * Time.deltaTime);
        FlipToFace(patrolCenter.x);

        if (Vector2.Distance(transform.position, patrolCenter) < 0.1f)
        {
            returning = false;
        }
    }

    void AttackPlayer()
    {
        animator?.SetTrigger("Attack");
        player.GetComponent<JumpKingController>()?.TakeDamage();
    }

    // Logic va chạm để gây sát thương
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<JumpKingController>()?.TakeDamage();
        }
    }

    void FlipToFace(float targetX)
    {
        if (targetX > transform.position.x)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }
}