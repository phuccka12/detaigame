using UnityEngine;

public class BeeEnemyFly : MonoBehaviour
{
    [Header("Patrol Settings")]
    public float moveSpeed = 2f;          // Tốc độ di chuyển
    public float moveRange = 3f;          // Phạm vi bay qua lại (nếu cần tuần tra trong vùng nhỏ)

    [Header("Chase Settings")]
    public float detectionRange = 4f;     // Phạm vi phát hiện player
    public float chaseSpeed = 4f;         // Tốc độ đuổi theo player
    public float returnSpeed = 2f;        // Tốc độ quay lại điểm tuần tra ban đầu

    private Vector3 patrolCenter;         // Vị trí bắt đầu tuần tra
    private bool movingRight = true;      // Hướng di chuyển hiện tại
    private bool chasing = false;         // Kiểm tra trạng thái đuổi theo player
    private bool returning = false;       // Kiểm tra trạng thái quay lại điểm tuần tra
    private Transform player;             // Nhân vật player

    private Animator animator;            // Animator để điều khiển hoạt ảnh của ong

    void Start()
    {
        patrolCenter = transform.position;  // Lưu lại vị trí ban đầu
        player = GameObject.FindGameObjectWithTag("Player").transform; // Tìm kiếm player
        animator = GetComponent<Animator>(); // Lấy Animator của con ong
    }

    void Update()
    {
        float distToPlayer = Vector2.Distance(transform.position, player.position); // Khoảng cách tới player
        float distPlayerToCenter = Vector2.Distance(player.position, patrolCenter); // Khoảng cách từ player đến điểm tuần tra

        // Nếu player vào phạm vi phát hiện
        if (!chasing && distToPlayer < detectionRange)
        {
            chasing = true;
            returning = false;
            animator.SetBool("IsChasing", true); // Bật animation đuổi theo player
        }
        // Nếu player rời xa khỏi phạm vi, quay lại điểm tuần tra
        else if (chasing && distToPlayer > detectionRange)
        {
            chasing = false;
            returning = true;
            animator.SetBool("IsChasing", false); // Tắt animation đuổi theo
        }

        // Nếu đang đuổi theo player
        if (chasing)
        {
            ChasePlayer();
        }
        // Nếu đang quay lại điểm tuần tra
        else if (returning)
        {
            ReturnToPatrolZone();
        }
        // Nếu không đuổi theo player, ong sẽ tuần tra
        else
        {
            Patrol();
        }
    }

    // Đoạn mã tuần tra qua lại
    void Patrol()
    {
        float moveStep = moveSpeed * Time.deltaTime;

        // Di chuyển ong theo chiều phải (right)
        if (movingRight)
        {
            transform.Translate(Vector2.right * moveStep);
            FlipToFace(patrolCenter.x + moveRange); // Lật hướng ong khi di chuyển

            // Kiểm tra nếu đã đi đủ xa, đổi hướng
            if (transform.position.x >= patrolCenter.x + moveRange)
                movingRight = false;
        }
        // Di chuyển ong theo chiều trái (left)
        else
        {
            transform.Translate(Vector2.left * moveStep);
            FlipToFace(patrolCenter.x - moveRange); // Lật hướng ong khi di chuyển

            // Kiểm tra nếu đã đi đủ xa, đổi hướng
            if (transform.position.x <= patrolCenter.x - moveRange)
                movingRight = true;
        }
    }

    // Đuổi theo player khi vào phạm vi
    void ChasePlayer()
    {
        transform.position = Vector2.MoveTowards(transform.position, player.position, chaseSpeed * Time.deltaTime);
        FlipToFace(player.position.x); // Lật hướng khi đuổi theo player

        // Nếu gần player, chuyển sang trạng thái tấn công
        if (Vector2.Distance(transform.position, player.position) < 1f)
        {
            AttackPlayer();
        }
    }

    // Quay lại điểm tuần tra
    void ReturnToPatrolZone()
    {
        transform.position = Vector2.MoveTowards(transform.position, patrolCenter, returnSpeed * Time.deltaTime);
        FlipToFace(patrolCenter.x);

        // Nếu quay về gần điểm tuần tra, dừng lại và tiếp tục tuần tra
        if (Vector2.Distance(transform.position, patrolCenter) < 0.05f)
        {
            returning = false;
            animator.SetBool("IsChasing", false); // Tắt animation quay lại
        }
    }

    // Tấn công player khi tiếp cận
    void AttackPlayer()
    {
        if (animator != null)
        {
            animator.SetTrigger("Attack");  // Gọi animation tấn công
        }

        // Gọi hàm làm mất mạng player (hoặc gây sát thương)
        JumpKingController jumpKingController = player.GetComponent<JumpKingController>();
        if (jumpKingController != null)
        {
            jumpKingController.TakeDamage();  // Gọi hàm mất mạng từ JumpKingController
        }
    }

    // Lật hướng ong khi cần thiết
    void FlipToFace(float targetX)
    {
        if (targetX > transform.position.x)
            transform.localScale = new Vector3(-1, 1, 1); // Hướng phải
        else
            transform.localScale = new Vector3(1, 1, 1);  // Hướng trái
    }

    // Kiểm tra va chạm với player
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Kiểm tra nếu không đang trong trạng thái thủ
            JumpKingController playerController = collision.gameObject.GetComponent<JumpKingController>();
            if (playerController != null && !playerController.isBlocking)
            {
                AttackPlayer();  // Khi va chạm với player, tấn công
            }
        }
    }
}
