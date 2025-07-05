using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Kế thừa từ PersistentSingleton để tự động trở thành đối tượng "bất tử" và duy nhất
public class GameManager : PersistentSingleton<GameManager>
{
    // KHÔNG cần dòng "public static GameManager instance;" ở đây nữa, vì PersistentSingleton đã lo việc đó.

    public int maxLives = 3;
    public int currentLives;
    public Image[] heartImages;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    // Hàm Awake được sửa lại cho đúng với việc kế thừa
    protected override void Awake()
    {
        // Gọi hàm Awake() của lớp cha (PersistentSingleton) để xử lý việc không bị phá hủy
        base.Awake();
        // Bạn có thể thêm các mã khởi tạo khác ở đây nếu cần
    }

    // Start() chỉ được gọi một lần duy nhất trong suốt vòng đời của đối tượng "bất tử" này.
    // Đây là nơi hoàn hảo để thiết lập số mạng ban đầu.
    void Start()
    {
        currentLives = maxLives;
        // Chúng ta không gọi UpdateHeartUI() ở đây vì UIManager ở mỗi màn sẽ lo việc đó.
    }

    // Hàm giảm số mạng và cập nhật UI
    public void LoseLife()
    {
        if (currentLives > 0)
        {
            currentLives--;
            UpdateHeartUI();

            if (currentLives <= 0)
            {
                GameOver();
            }
            else
            {
                RespawnPlayer();
            }
        }
    }

    // Cập nhật UI trái tim
    public void UpdateHeartUI()
    {
        // Nếu heartImages chưa được gán (ví dụ: đang ở giữa lúc chuyển cảnh), không làm gì cả
        if (heartImages == null || heartImages.Length == 0) return;

        for (int i = 0; i < heartImages.Length; i++)
        {
            if (i < currentLives)
            {
                heartImages[i].sprite = fullHeart;
                heartImages[i].enabled = true;
            }
            else
            {
                heartImages[i].sprite = emptyHeart;
                heartImages[i].enabled = false; // Trái tim rỗng thì nên ẩn đi thay vì chỉ đổi hình
            }
        }
    }

    void GameOver()
    {
        Debug.Log("Game Over!");
        // Reset lại mạng cho lần chơi tiếp theo và tải scene Game Over
        currentLives = maxLives;
        SceneManager.LoadScene("GameOverScene"); // Thay "GameOverScene" bằng tên scene Game Over của bạn
    }

    public void RespawnPlayer()
    {
        // Tìm instance của Player thông qua Singleton thay vì FindWithTag
        if (JumpKingController.instance != null)
        {
            JumpKingController playerController = JumpKingController.instance;
            playerController.transform.position = playerController.GetSpawnPosition();
        }
    }
}