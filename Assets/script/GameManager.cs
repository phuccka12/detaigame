using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : PersistentSingleton<GameManager>
{
    public int maxLives = 3;
    public int currentLives;
    [HideInInspector] public Image[] heartImages;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    private bool isLoadingFromSave = false;
    private Vector3 loadedPlayerPosition;
    private bool hasBeenInitialized = false;
    public static string nextSceneToLoad;

    protected override void Awake()
    {
        base.Awake();
        if (!hasBeenInitialized)
        {
            currentLives = maxLives;
            hasBeenInitialized = true;
        }
        // Đăng ký lắng nghe sự kiện khi một scene được tải xong
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        // Luôn hủy đăng ký khi đối tượng bị phá hủy để tránh lỗi
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // --- HỆ THỐNG SAVE/LOAD ---

    public void SaveGame()
    {
        if (JumpKingController.instance == null) return;

        string currentScene = SceneManager.GetActiveScene().name;
        Vector3 playerPosition = JumpKingController.instance.transform.position;

        PlayerPrefs.SetString("SavedScene", currentScene);
        PlayerPrefs.SetInt("SavedLives", currentLives);
        PlayerPrefs.SetFloat("PlayerPosX", playerPosition.x);
        PlayerPrefs.SetFloat("PlayerPosY", playerPosition.y);
        PlayerPrefs.SetFloat("PlayerPosZ", playerPosition.z);
        PlayerPrefs.SetInt("SaveExists", 1);
        PlayerPrefs.Save();
        Debug.Log("Game Saved! Lives: " + currentLives);
    }

    public void LoadGame()
    {
        if (!PlayerPrefs.HasKey("SaveExists")) return;

        isLoadingFromSave = true;
        currentLives = PlayerPrefs.GetInt("SavedLives");
        loadedPlayerPosition = new Vector3(PlayerPrefs.GetFloat("PlayerPosX"), PlayerPrefs.GetFloat("PlayerPosY"), PlayerPrefs.GetFloat("PlayerPosZ"));
        nextSceneToLoad = PlayerPrefs.GetString("SavedScene");

        SceneManager.LoadScene("LoadingScreen");
    }

    // --- QUẢN LÝ TRẠNG THÁI ---

    // Hàm này sẽ tự động chạy MỖI KHI một scene tải xong
    // Dán hàm này vào để thay thế hàm OnSceneLoaded cũ trong GameManager.cs

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Bỏ qua nếu là Menu hoặc Loading
        if (scene.name == "MenuStart" || scene.name == "LoadingScreen") return;

        // 1. Tìm UIManager để cập nhật UI
        UIManager uiManagerInScene = FindObjectOfType<UIManager>();
        if (uiManagerInScene != null)
        {
            this.heartImages = uiManagerInScene.heartImages;
            UpdateHeartUI();
        }

        // 2. Kiểm tra xem nên đặt Player ở đâu
        if (isLoadingFromSave)
        {
            // Nếu đang tải từ file save, đặt Player ở vị trí đã lưu
            StartCoroutine(ApplyLoadedData());
        }
        else
        {
            // CẬP NHẬT QUAN TRỌNG:
            // Nếu là chuyển màn thông thường, tìm SpawnPoint và dịch chuyển Player đến đó
            GameObject spawnPoint = GameObject.FindGameObjectWithTag("SpawnPoint");
            if (spawnPoint != null && JumpKingController.instance != null)
            {
                JumpKingController.instance.transform.position = spawnPoint.transform.position;
                // Cập nhật cả vị trí hồi sinh để nếu chết sẽ quay lại đây
                JumpKingController.instance.SetNewSpawnPosition(spawnPoint.transform.position);
            }
        }
    }

    // Coroutine để đảm bảo mọi thứ trong scene đã sẵn sàng
    private IEnumerator ApplyLoadedData()
    {
        // Đợi đến cuối frame để đảm bảo hàm Start() của Player đã chạy xong
        yield return new WaitForEndOfFrame();

        if (JumpKingController.instance != null)
        {
            JumpKingController.instance.transform.position = loadedPlayerPosition;
            JumpKingController.instance.SetNewSpawnPosition(loadedPlayerPosition);
        }

        // Reset cờ sau khi hoàn tất
        isLoadingFromSave = false;
    }
    // Thêm hàm này vào trong file GameManager.cs
    public void Heal(int amount)
    {
        // Tăng mạng sống hiện tại
        currentLives += amount;

        // Đảm bảo mạng sống không vượt quá tối đa
        if (currentLives > maxLives)
        {
            currentLives = maxLives;
        }

        // Cập nhật lại UI trái tim
        UpdateHeartUI();
    }
    public void UpdateHeartUI()
    {
        if (heartImages == null) return;
        for (int i = 0; i < heartImages.Length; i++)
        {
            if (heartImages[i] != null) heartImages[i].enabled = i < currentLives;
        }
    }

    // Các hàm còn lại không đổi
    public void StartNewGame() { currentLives = maxLives; PlayerPrefs.DeleteKey("SaveExists"); }
    public void RestartLevel() { currentLives = maxLives; Time.timeScale = 1f; SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); }
    public void ReturnToMainMenu() { if (JumpKingController.instance != null) Destroy(JumpKingController.instance.gameObject); Time.timeScale = 1f; this.heartImages = null; SceneManager.LoadScene("MenuStart"); }
    public void LoseLife() { if (currentLives > 0) { currentLives--; UpdateHeartUI(); if (currentLives <= 0) GameOver(); else RespawnPlayer(); } }
    void GameOver() { ReturnToMainMenu(); }
    public void RespawnPlayer() { if (JumpKingController.instance != null) { JumpKingController.instance.transform.position = JumpKingController.instance.GetSpawnPosition(); } }
}