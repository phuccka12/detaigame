using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.EventSystems;

public class MenuManager : MonoBehaviour
{
    [Header("UI Buttons")]
    public Button startButton;
    public Button settingsButton;
    public Button quitButton;
    public Button backButton;  // Nút quay lại từ Settings1

    [Header("Scene To Load")]
    public string mainGameSceneName = "map1";

    public static string sceneToLoad;

    // Các UI Panels
    public GameObject mainMenuPanel;  // Menu chính
    public GameObject settingPanel;   // Menu cài đặt

    private Coroutine activeBlinkCoroutine;

    void Start()
    {
        startButton.onClick.AddListener(StartGame);
        settingsButton.onClick.AddListener(OpenSettings);
        quitButton.onClick.AddListener(QuitGame);
        backButton.onClick.AddListener(CloseSettings);  // Gắn nút Back để quay lại menu chính

        AddHoverEffect(startButton);
        AddHoverEffect(settingsButton);
        AddHoverEffect(quitButton);
    }

    // --- CÁC HÀM CHỨC NĂNG CHÍNH ---

    public void StartGame()
    {
        sceneToLoad = mainGameSceneName;
        SceneManager.LoadScene("LoadingScreen");
    }

    public void OpenSettings()
    {
        Debug.Log("Mở Cài đặt!");
        mainMenuPanel.SetActive(false);  // Ẩn menu chính
        settingPanel.SetActive(true);    // Hiển thị menu cài đặt
    }

    public void CloseSettings()
    {
        Debug.Log("Đóng Cài đặt!");
        settingPanel.SetActive(false);  // Ẩn menu cài đặt
        mainMenuPanel.SetActive(true);  // Hiển thị menu chính
    }

    public void QuitGame()
    {
        Debug.Log("Thoát Game!");
        Application.Quit();
    }

    // --- LOGIC HIỆU ỨNG NHẤP NHÁY ---

    private void AddHoverEffect(Button button)
    {
        if (button.GetComponent<CanvasGroup>() == null)
        {
            button.gameObject.AddComponent<CanvasGroup>();
        }

        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>() ?? button.gameObject.AddComponent<EventTrigger>();
        trigger.triggers.Clear();

        // Sự kiện khi chuột đi vào
        EventTrigger.Entry pointerEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        pointerEnter.callback.AddListener((data) => { OnPointerEnter(button.GetComponent<CanvasGroup>()); });
        trigger.triggers.Add(pointerEnter);

        // Sự kiện khi chuột đi ra
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
    // Tránh lỗi khi object đã bị destroy hoặc canvasGroup null
    if (this == null || canvasGroup == null) return;

    if (activeBlinkCoroutine != null)
    {
        StopCoroutine(activeBlinkCoroutine);
        activeBlinkCoroutine = null;
    }

    canvasGroup.alpha = 1f; // Reset độ trong suốt về 100%
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
