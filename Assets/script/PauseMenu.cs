using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject pauseMenuPanel; // Kéo Panel của menu pause vào đây

    [Header("Buttons")]
    public Button resumeButton;
    public Button saveButton;
    public Button restartButton;
    public Button homeButton;

    void Awake()
    {
        // Gán sự kiện cho các nút để chúng gọi đúng hàm
        resumeButton.onClick.AddListener(Resume);
        saveButton.onClick.AddListener(Save);
        restartButton.onClick.AddListener(Restart);
        homeButton.onClick.AddListener(Home);
    }

    void Start()
    {
        // Luôn ẩn menu khi màn chơi bắt đầu
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }
    }

    void Update()
    {
        // Lắng nghe phím ESC để mở/đóng menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseMenuPanel.activeInHierarchy)
            {
                Resume(); // Gọi hàm Resume() gốc của bạn
            }
            else
            {
                Pause(); // Gọi hàm Pause() gốc của bạn
            }
        }
    }

    // --- CÁC HÀM GỐC CỦA BẠN ĐƯỢC GIỮ LẠI VÀ NÂNG CẤP ---

    // Hàm này được gọi bởi cả phím ESC và nút Pause trên màn hình
    public void Pause()
    {
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    // Hàm mới cho nút Save, gọi tới GameManager
    public void Save()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.SaveGame();
        }
    }

    // Các hàm này giờ sẽ gọi tới GameManager để đảm bảo mọi thứ chạy đúng
    public void Restart()
    {
        Time.timeScale = 1f;
        if (GameManager.instance != null)
        {
            GameManager.instance.RestartLevel();
        }
    }

    public void Home()
    {
        Time.timeScale = 1f;
        if (GameManager.instance != null)
        {
            GameManager.instance.ReturnToMainMenu();
        }
    }
}