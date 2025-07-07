using UnityEngine;

public class MovinghPlatform : MonoBehaviour
{
    [Header("Cài đặt di chuyển")]
    // Tốc độ di chuyển của platform
    public float moveSpeed = 3f;

    // Giới hạn di chuyển theo trục X
    public float leftBoundary = -5f;
    public float rightBoundary = 5f;

    // Biến nội bộ
    private Vector3 targetPosition;
    private float startY; // Để giữ cho platform không bị trôi lên hoặc xuống

    void Start()
    {
        // Lưu lại độ cao ban đầu
        startY = transform.position.y;

        // Chọn một điểm đến ngẫu nhiên đầu tiên
        PickNewRandomDestination();
    }

    void Update()
    {
        // Di chuyển platform về phía mục tiêu hiện tại
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // Nếu platform đã đến rất gần mục tiêu, hãy chọn một mục tiêu mới
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            PickNewRandomDestination();
        }
    }

    // Hàm để chọn một điểm đến ngẫu nhiên mới
    void PickNewRandomDestination()
    {
        // Tạo một vị trí X ngẫu nhiên trong khoảng giới hạn
        float randomX = Random.Range(leftBoundary, rightBoundary);

        // Thiết lập mục tiêu mới, giữ nguyên độ cao Y
        targetPosition = new Vector3(randomX, startY, transform.position.z);
    }

    // Hàm va chạm để "chở" nhân vật (giữ nguyên)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }
}