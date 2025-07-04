using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;  // Để chuyển scene khi Game Over

public class GameManager : MonoBehaviour
{
    public int maxLives = 3;              // Tổng số trái tim (mạng)
    public int currentLives;              // Số trái tim hiện tại (mạng còn lại)
    public Image[] heartImages;           // Mảng UI Image cho trái tim
    public Sprite fullHeart;              // Sprite trái tim đầy
    public Sprite emptyHeart;             // Sprite trái tim trống

    void Start()
    {
        currentLives = maxLives;  // Khởi tạo số mạng ban đầu
        UpdateHeartUI();          // Cập nhật UI trái tim khi game bắt đầu
    }

    // Hàm giảm số mạng và cập nhật UI
    public void LoseLife()
    {
        // Giảm một mạng
        if (currentLives > 0)
        {
            currentLives--;
            UpdateHeartUI(); // Cập nhật lại UI trái tim

            // Kiểm tra nếu hết mạng
            if (currentLives <= 0)
            {
                GameOver();  // Gọi hàm Game Over khi hết mạng
            }
            else
            {
                RespawnPlayer();  // Hồi sinh nhân vật
            }
        }
    }

    // Cập nhật UI trái tim khi mất mạng
    void UpdateHeartUI()
    {
        // Lặp qua các trái tim trong mảng heartImages
        for (int i = 0; i < heartImages.Length; i++)
        {
            if (i < currentLives)
            {
                heartImages[i].sprite = fullHeart;  // Đặt sprite trái tim đầy
                heartImages[i].enabled = true;      // Hiển thị trái tim
            }
            else
            {
                heartImages[i].sprite = emptyHeart; // Đặt sprite trái tim trống
                heartImages[i].enabled = true;      // Hiển thị trái tim trống
            }
        }

        // Ẩn các trái tim nếu không đủ số mạng
        for (int i = currentLives; i < heartImages.Length; i++)
        {
            heartImages[i].enabled = false;  // Ẩn trái tim thừa
        }
    }

    // Hàm xử lý khi game over
    void GameOver()
    {
        Debug.Log("Game Over!");
        // Bạn có thể thêm logic game over ở đây, ví dụ như dừng game hoặc load lại scene.
        // Ví dụ: Chuyển sang màn hình Game Over
        SceneManager.LoadScene("GameOverScene");  // Thay "GameOverScene" bằng tên scene Game Over của bạn
    }

    // Hàm hồi sinh lại nhân vật
    public void RespawnPlayer()
    {
        // Hồi sinh nhân vật tại vị trí spawn
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            // Lấy vị trí hồi sinh từ PlayerController
            JumpKingController playerController = player.GetComponent<JumpKingController>();
            if (playerController != null)
            {
                player.transform.position = playerController.GetSpawnPosition(); // Hồi sinh tại spawn position
            }
        }
    }
}
