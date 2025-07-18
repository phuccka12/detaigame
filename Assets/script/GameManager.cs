using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : PersistentSingleton<GameManager>
{
    // --- CÁC BIẾN MỚI CHO TIMER ---
    [Header("Timing & Score")]
    private float startTime;
    private float elapsedTime;
    private bool timerIsRunning = false;
    // --- KẾT THÚC PHẦN BIẾN MỚI ---

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
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // --- THÊM HÀM UPDATE ĐỂ ĐẾM GIỜ ---
    void Update()
    {
        if (timerIsRunning)
        {
            elapsedTime = Time.time - startTime;
        }
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
        timerIsRunning = false; // Tạm dừng đếm giờ khi load game
        currentLives = PlayerPrefs.GetInt("SavedLives");
        loadedPlayerPosition = new Vector3(PlayerPrefs.GetFloat("PlayerPosX"), PlayerPrefs.GetFloat("PlayerPosY"), PlayerPrefs.GetFloat("PlayerPosZ"));
        nextSceneToLoad = PlayerPrefs.GetString("SavedScene");

        SceneManager.LoadScene("LoadingScreen");
    }

    // --- QUẢN LÝ TRẠNG THÁI ---
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MenuStart" || scene.name == "LoadingScreen") return;

        UIManager uiManagerInScene = FindObjectOfType<UIManager>();
        if (uiManagerInScene != null)
        {
            this.heartImages = uiManagerInScene.heartImages;
            UpdateHeartUI();
        }

        if (isLoadingFromSave)
        {
            StartCoroutine(ApplyLoadedData());
        }
        else
        {
            GameObject spawnPoint = GameObject.FindGameObjectWithTag("SpawnPoint");
            if (spawnPoint != null && JumpKingController.instance != null)
            {
                JumpKingController.instance.transform.position = spawnPoint.transform.position;
                JumpKingController.instance.SetNewSpawnPosition(spawnPoint.transform.position);
            }
        }
    }

    private IEnumerator ApplyLoadedData()
    {
        yield return new WaitForEndOfFrame();
        if (JumpKingController.instance != null)
        {
            JumpKingController.instance.transform.position = loadedPlayerPosition;
            JumpKingController.instance.SetNewSpawnPosition(loadedPlayerPosition);
        }
        isLoadingFromSave = false;
    }

    public void Heal(int amount)
    {
        currentLives += amount;
        if (currentLives > maxLives)
        {
            currentLives = maxLives;
        }
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

    public void StartNewGame()
    {
        currentLives = maxLives;
        PlayerPrefs.DeleteKey("SaveExists");

        // --- CẬP NHẬT: BẮT ĐẦU ĐẾM GIỜ ---
        startTime = Time.time;
        timerIsRunning = true;
    }

    // --- CÁC HÀM MỚI CHO TIMER & CHIẾN THẮNG ---
    public void PlayerWon()
    {
        timerIsRunning = false;
        float bestTime = PlayerPrefs.GetFloat("BestTime", -1f);

        if (bestTime < 0 || elapsedTime < bestTime)
        {
            PlayerPrefs.SetFloat("BestTime", elapsedTime);
            bestTime = elapsedTime;
        }

        // --- PHẦN GỠ LỖI ---
        Debug.Log("<color=yellow>1. GameManager: Hàm PlayerWon() đã được gọi. Bắt đầu tìm kiếm VictoryScreen...</color>");

        VictoryScreen victoryScreen = FindObjectOfType<VictoryScreen>();

        if (victoryScreen != null)
        {
            Debug.Log("<color=green>2. GameManager: ĐÃ TÌM THẤY VictoryScreen! Sẽ gọi hàm Show().</color>");
            victoryScreen.Show(elapsedTime, bestTime);
        }
        else
        {
            Debug.LogError("### LỖI: GameManager KHÔNG TÌM THẤY đối tượng nào có script VictoryScreen trong scene!");
        }
        // --- KẾT THÚC PHẦN GỠ LỖI ---
    }

    public float GetCurrentTime()
    {
        return elapsedTime;
    }

    public float GetBestTime()
    {
        return PlayerPrefs.GetFloat("BestTime", -1f);
    }
    // --- KẾT THÚC PHẦN HÀM MỚI ---

    public void RestartLevel() { currentLives = maxLives; Time.timeScale = 1f; SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); }
    public void ReturnToMainMenu() { if (JumpKingController.instance != null) Destroy(JumpKingController.instance.gameObject); Time.timeScale = 1f; this.heartImages = null; SceneManager.LoadScene("MenuStart"); timerIsRunning = false; }
    public void LoseLife() { if (currentLives > 0) { currentLives--; UpdateHeartUI(); if (currentLives <= 0) GameOver(); else RespawnPlayer(); } }

    void GameOver()
    {
        timerIsRunning = false; // Dừng đếm giờ khi thua
        if (JumpKingController.instance != null) Destroy(JumpKingController.instance.gameObject);
        SceneManager.LoadScene("GameOverScence");
    }

    public void RespawnPlayer() { if (JumpKingController.instance != null) { JumpKingController.instance.transform.position = JumpKingController.instance.GetSpawnPosition(); } }
}