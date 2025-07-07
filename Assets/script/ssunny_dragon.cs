using UnityEngine;

public class FlyingDragonAI : MonoBehaviour
{
    [Header("Patrol Settings")]
    public float moveSpeed = 2f;
    public float moveRange = 3f;

    [Header("Chase Settings")]
    public float detectionRange = 5f;
    public float chaseSpeed = 4f;
    public float returnSpeed = 2.5f;

    private Vector3 patrolCenter;
    private bool movingRight = true;
    private bool isChasing = false;
    private bool isReturning = false;
    private Transform player;
    private Animator animator;

    void Start()
    {
        patrolCenter = transform.position;
        animator = GetComponent<Animator>();
        if (JumpKingController.instance != null)
        {
            player = JumpKingController.instance.transform;
        }
    }

    void Update()
    {
        if (player == null)
        {
            if (JumpKingController.instance != null) player = JumpKingController.instance.transform;
            else return;
        }

        float distToPlayer = Vector2.Distance(transform.position, player.position);

        if (!isChasing && !isReturning && distToPlayer < detectionRange)
        {
            isChasing = true;
            animator.SetBool("IsChasing", true);
        }
        else if (isChasing && distToPlayer > detectionRange)
        {
            isChasing = false;
            isReturning = true;
            animator.SetBool("IsChasing", false);
        }

        if (isChasing) ChasePlayer();
        else if (isReturning) ReturnToPatrolZone();
        else Patrol();

        FlipSprite();
    }

    void Patrol()
    {
        float moveStep = moveSpeed * Time.deltaTime;
        if (movingRight)
        {
            transform.Translate(Vector2.right * moveStep);
            if (transform.position.x >= patrolCenter.x + moveRange)
                movingRight = false;
        }
        else
        {
            transform.Translate(Vector2.left * moveStep);
            if (transform.position.x <= patrolCenter.x - moveRange)
                movingRight = true;
        }
    }

    void ChasePlayer()
    {
        transform.position = Vector2.MoveTowards(transform.position, player.position, chaseSpeed * Time.deltaTime);
    }

    void ReturnToPatrolZone()
    {
        transform.position = Vector2.MoveTowards(transform.position, patrolCenter, returnSpeed * Time.deltaTime);
        if (Vector2.Distance(transform.position, patrolCenter) < 0.1f)
        {
            isReturning = false;
        }
    }

    void FlipSprite()
    {
        // Giả định: hình gốc của rồng quay mặt sang PHẢI.
        // Nếu rồng của bạn quay mặt sang TRÁI, hãy đảo ngược giá trị 1 và -1 ở dưới.
        float horizontalDirection = 0;

        if (isChasing && player != null)
        {
            horizontalDirection = player.position.x - transform.position.x;
        }
        else
        {
            // Xác định hướng di chuyển khi tuần tra hoặc quay về
            Vector3 targetPoint = movingRight ? patrolCenter + Vector3.right : patrolCenter + Vector3.left;
            horizontalDirection = targetPoint.x - transform.position.x;
        }

        if (Mathf.Abs(horizontalDirection) > 0.01f) // Chỉ lật khi có hướng di chuyển rõ ràng
        {
            if (horizontalDirection > 0)
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            else
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isChasing && collision.gameObject.CompareTag("Player"))
        {
            JumpKingController playerController = collision.gameObject.GetComponent<JumpKingController>();
            if (playerController != null && !playerController.isBlocking)
            {
                if (GameManager.instance != null)
                {
                    GameManager.instance.LoseLife();
                }
                if (animator != null)
                {
                    animator.SetTrigger("Attack");
                }
            }
        }
    }
}