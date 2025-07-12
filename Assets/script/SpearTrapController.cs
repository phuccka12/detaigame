using UnityEngine;
using System.Collections;

public class SpearTrapController : MonoBehaviour
{
    [Header("Trap Settings")]
    [Tooltip("Phạm vi mà bẫy sẽ phát hiện người chơi để tấn công")]
    public float detectionRange = 5f;

    [Tooltip("Thời gian chờ (giây) sau mỗi lần tấn công trước khi có thể tấn công lại")]
    public float attackCooldown = 3f;

    // Biến nội bộ
    private Transform player;
    private Animator animator;
    private bool canAttack = true; // Cờ để kiểm tra xem bẫy có thể tấn công không

    void Start()
    {
        // Lấy component Animator từ chính đối tượng bẫy
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Nếu chưa tìm thấy Player, hãy thử tìm lại
        if (player == null)
        {
            if (JumpKingController.instance != null)
            {
                player = JumpKingController.instance.transform;
            }
            else
            {
                // Nếu không có player trong game, không cần làm gì cả
                return;
            }
        }

        // Nếu bẫy đã sẵn sàng tấn công
        if (canAttack)
        {
            // Kiểm tra khoảng cách tới người chơi
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            // Nếu người chơi đi vào phạm vi phát hiện
            if (distanceToPlayer < detectionRange)
            {
                // Bắt đầu chuỗi tấn công
                StartCoroutine(AttackSequence());
            }
        }
    }

    // Coroutine để quản lý chuỗi hành động tấn công và hồi chiêu
    private IEnumerator AttackSequence()
    {
        // 1. Đánh dấu là không thể tấn công ngay lập tức để bắt đầu hồi chiêu
        canAttack = false;

        // 2. Ra lệnh cho Animator chạy animation tấn công
        if (animator != null)
        {
            animator.SetTrigger("Attack"); // Bạn cần một Trigger tên là "Attack" trong Animator
        }

        // 3. Đợi hết thời gian hồi chiêu
        yield return new WaitForSeconds(attackCooldown);

        // 4. Cho phép bẫy tấn công trở lại
        canAttack = true;
    }

    // Vẽ vùng phát hiện trong Editor để dễ dàng căn chỉnh
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}