using UnityEngine;

public class SavePoint : MonoBehaviour
{
    // Biến để thêm hiệu ứng, ví dụ như đổi màu khi đã lưu
    private SpriteRenderer spriteRenderer;
    public Color savedColor = Color.cyan; // Màu sẽ chuyển sang sau khi lưu

    private bool hasBeenUsed = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Hàm này được tự động gọi khi có một đối tượng khác đi vào trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra xem đối tượng va chạm có phải là Player không và điểm lưu này chưa được dùng
        if (!hasBeenUsed && other.CompareTag("Player"))
        {
            Debug.Log("Player entered Save Point. Saving game...");

            // Gọi hàm lưu game từ GameManager
            GameManager.instance?.SaveGame();

            // Đánh dấu là đã sử dụng
            hasBeenUsed = true;

            // (Tùy chọn) Tạo hiệu ứng hình ảnh để báo cho người chơi biết đã lưu
            if (spriteRenderer != null)
            {
                spriteRenderer.color = savedColor;
            }

            // (Tùy chọn) Bạn có thể chơi một âm thanh báo hiệu đã lưu ở đây
        }
    }
}