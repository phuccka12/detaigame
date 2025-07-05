using UnityEngine;

public class SawController : MonoBehaviour
{
    [Header("Movement Settings")]
    // Tốc độ di chuyển của lưỡi cưa
    public float moveSpeed = 3f;
    // Khoảng cách di chuyển từ điểm xuất phát
    public float patrolDistance = 5f;

    [Header("Rotation Settings")]
    // Tốc độ xoay (độ/giây)
    public float rotationSpeed = 360f;

    // Các biến nội bộ để xử lý di chuyển
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private bool isMovingForward = true;

    void Start()
    {
        // Lưu lại vị trí ban đầu khi game bắt đầu
        startPosition = transform.position;
        // Thiết lập mục tiêu đầu tiên là vị trí ở phía trước
        targetPosition = startPosition + new Vector3(patrolDistance, 0, 0);
    }

    void Update()
    {
        HandleRotation();
        HandleMovement();
    }

    // Hàm xử lý việc xoay lưỡi cưa
    private void HandleRotation()
    {
        // Xoay lưỡi cưa quanh trục Z liên tục
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }

    // Hàm xử lý việc di chuyển qua lại
    private void HandleMovement()
    {
        // Di chuyển lưỡi cưa về phía mục tiêu hiện tại
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // Kiểm tra nếu lưỡi cưa đã đến rất gần mục tiêu
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            // Nếu đang đi về phía trước (đến điểm patrolDistance)
            if (isMovingForward)
            {
                // Đổi mục tiêu thành vị trí ban đầu
                targetPosition = startPosition;
            }
            // Nếu đang đi về (về lại điểm xuất phát)
            else
            {
                // Đổi mục tiêu thành vị trí phía trước
                targetPosition = startPosition + new Vector3(patrolDistance, 0, 0);
            }
            // Đảo ngược hướng di chuyển
            isMovingForward = !isMovingForward;
        }
    }

    // Xử lý va chạm với người chơi (giữ nguyên như cũ)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Kiểm tra nếu va chạm với đối tượng có tag "Player"
        if (collision.gameObject.CompareTag("Player"))
        {
            JumpKingController playerController = collision.gameObject.GetComponent<JumpKingController>();
            // Nếu người chơi không đang đỡ đòn
            if (playerController != null && !playerController.isBlocking)
            {
                Debug.Log("Player bị trúng lưỡi cưa!");
                GameManager gameManager = FindObjectOfType<GameManager>();
                if (gameManager != null)
                {
                    // Trừ mạng của người chơi
                    gameManager.LoseLife();
                }
            }
        }
    }
}