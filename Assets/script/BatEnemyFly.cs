using UnityEngine;

public class BatEnemyFlySmoothChase : MonoBehaviour
{
    [Header("Z Patrol Settings")]
    public Transform[] waypoints;
    public float patrolSpeed = 2f;
    public float smoothTime = 0.3f;

    [Header("Chase Settings")]
    public float detectionRange = 4f;
    public float chaseSpeed = 5f;
    public float returnSpeed = 3f;

    private Transform player;
    private Animator animator;
    private Vector3 velocity = Vector3.zero;
    private int currentIndex = 0;
    private bool chasing = false;
    private bool returning = false;
    private Vector3 returnPoint;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 0;
            rb.freezeRotation = true;
        }

        transform.position = waypoints[0].position;
    }

    void Update()
    {
        float distToPlayer = Vector2.Distance(transform.position, player.position);

        if (!chasing && distToPlayer < detectionRange)
        {
            chasing = true;
            returning = false;
            animator?.SetBool("IsChasing", true);
            returnPoint = transform.position;
        }
        else if (chasing && distToPlayer > detectionRange)
        {
            chasing = false;
            returning = true;
            animator?.SetBool("IsChasing", false);
        }

        if (chasing)
        {
            ChasePlayer();
        }
        else if (returning)
        {
            ReturnToPath();
        }
        else
        {
            SmoothPatrol();
        }

        FixZ();
    }

    void SmoothPatrol()
    {
        if (waypoints.Length == 0) return;

        Transform target = waypoints[currentIndex];
        Vector3 targetPos = new Vector3(target.position.x, target.position.y, -0.1f);

        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime, patrolSpeed);
        FlipToFace(target.position.x);

        if (Vector2.Distance(transform.position, target.position) < 0.2f)
        {
            currentIndex = (currentIndex + 1) % waypoints.Length;
        }
    }

    void ChasePlayer()
    {
        Vector3 target = player.position;
        target.z = -0.1f;
        transform.position = Vector2.MoveTowards(transform.position, target, chaseSpeed * Time.deltaTime);
        FlipToFace(target.x);

        if (Vector2.Distance(transform.position, player.position) < 1f)
        {
            AttackPlayer();
        }
    }

    void ReturnToPath()
    {
        Vector3 target = waypoints[currentIndex].position;
        target.z = -0.1f;

        transform.position = Vector2.MoveTowards(transform.position, target, returnSpeed * Time.deltaTime);
        FlipToFace(target.x);

        if (Vector2.Distance(transform.position, target) < 0.2f)
        {
            returning = false;
        }
    }

    void AttackPlayer()
    {
        animator?.SetTrigger("Attack");
        JumpKingController jumpKingController = player.GetComponent<JumpKingController>();
        if (jumpKingController != null)
        {
            jumpKingController.TakeDamage();
        }
    }

    void FlipToFace(float targetX)
    {
        Vector3 scale = transform.localScale;
        if (targetX > transform.position.x)
            scale.x = -Mathf.Abs(scale.x);
        else
            scale.x = Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    void FixZ()
    {
        Vector3 pos = transform.position;
        pos.z = -0.1f;
        transform.position = pos;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            JumpKingController playerController = collision.gameObject.GetComponent<JumpKingController>();
            if (playerController != null && !playerController.isBlocking)
            {
                AttackPlayer();
            }
        }
    }
}
