using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.EventSystems; // THÊM LẠI: Thư viện cần thiết cho hiệu ứng

public class MenuManager : MonoBehaviour
{
    [Header("UI Buttons")]
    public Button startButton;
    public Button continueButton;
    public Button settingsButton;
    public Button quitButton;
    public Button backButton;

    [Header("Scene To Load")]
    public string mainGameSceneName = "map1";
    public string loadingSceneName = "LoadingScreen";

    [Header("UI Panels")]
    public GameObject mainMenuPanel;
    public GameObject settingPanel;

    // THÊM LẠI: Biến để quản lý hiệu ứng nhấp nháy
    private Coroutine activeBlinkCoroutine;

    void Start()
    {
        // Gán sự kiện cho các nút
        startButton.onClick.AddListener(StartGame);
        continueButton.onClick.AddListener(ContinueGame);
        settingsButton.onClick.AddListener(OpenSettings);
        quitButton.onClick.AddListener(QuitGame);
        backButton.onClick.AddListener(CloseSettings);

        // Bật/tắt nút Continue dựa trên file save
        bool saveExists = PlayerPrefs.HasKey("SaveExists");
        continueButton.interactable = saveExists;

        // THÊM LẠI: Kích hoạt hiệu ứng cho các nút
        AddHoverEffect(startButton);
        AddHoverEffect(settingsButton);
        AddHoverEffect(quitButton);
        // Chỉ thêm hiệu ứng cho nút Continue nếu nó được bật
        if (saveExists)
        {
            AddHoverEffect(continueButton);
        }
    }

    // --- CÁC HÀM CHỨC NĂNG CHÍNH ---

    public void StartGame()
    {
        if (GameManager.instance != null) GameManager.instance.StartNewGame();
        GameManager.nextSceneToLoad = mainGameSceneName;
        SceneManager.LoadScene(loadingSceneName);
    }

    public void ContinueGame()
    {
        if (GameManager.instance != null) GameManager.instance.LoadGame();
    }

    public void OpenSettings()
    {
        mainMenuPanel.SetActive(false);
        settingPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Debug.Log("Thoát Game!");
        Application.Quit();
    }

    // --- THÊM LẠI: TOÀN BỘ LOGIC HIỆU ỨNG NHẤP NHÁY CỦA BẠN ---

    private void AddHoverEffect(Button button)
    {
        if (button.GetComponent<CanvasGroup>() == null)
        {
            button.gameObject.AddComponent<CanvasGroup>();
        }

        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>() ?? button.gameObject.AddComponent<EventTrigger>();
        trigger.triggers.Clear();

        EventTrigger.Entry pointerEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        pointerEnter.callback.AddListener((data) => { OnPointerEnter(button.GetComponent<CanvasGroup>()); });
        trigger.triggers.Add(pointerEnter);

        EventTrigger.Entry pointerExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        pointerExit.callback.AddListener((data) => { OnPointerExit(button.GetComponent<CanvasGroup>()); });
        trigger.triggers.Add(pointerExit);
    }

    private void OnPointerEnter(CanvasGroup canvasGroup)
    {
        if (activeBlinkCoroutine != null) StopCoroutine(activeBlinkCoroutine);
        activeBlinkCoroutine = StartCoroutine(BlinkEffect(canvasGroup));
    }

    private void OnPointerExit(CanvasGroup canvasGroup)
    {
        if (this == null || canvasGroup == null) return;

        if (activeBlinkCoroutine != null)
        {
            StopCoroutine(activeBlinkCoroutine);
            activeBlinkCoroutine = null;
        }

        canvasGroup.alpha = 1f;
    }

    private IEnumerator BlinkEffect(CanvasGroup canvasGroup)
    {
        float blinkSpeed = 3f;
        float minAlpha = 0.5f;

        while (true)
        {
            canvasGroup.alpha = Mathf.Lerp(minAlpha, 1f, (Mathf.Sin(Time.time * blinkSpeed) + 1f) / 2f);
            yield return null;
        }
    }
}