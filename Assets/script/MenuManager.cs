using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.EventSystems;

public class MenuManager : MonoBehaviour
{
    [Header("UI Buttons")]
    // Kéo các nút từ Hierarchy vào đây trong Inspector
    public Button startButton;
    public Button settingsButton;
    public Button quitButton;

    [Header("Scene To Load")]
    // Tên của màn chơi chính để tải
    public string mainGameSceneName = "map3";

    // Biến static để truyền tên scene cho màn hình loading
    public static string sceneToLoad;

    // Biến để quản lý coroutine nhấp nháy đang hoạt động
    private Coroutine activeBlinkCoroutine;

    void Start()
    {
        // Gán sự kiện cho các nút bằng code
        startButton.onClick.AddListener(StartGame);
        settingsButton.onClick.AddListener(OpenSettings);
        quitButton.onClick.AddListener(QuitGame);

        // Thêm hiệu ứng hover cho từng nút
        AddHoverEffect(startButton);
        AddHoverEffect(settingsButton);
        AddHoverEffect(quitButton);
    }

    // --- CÁC HÀM CHỨC NĂNG CHÍNH ---

    public void StartGame()
    {
        // 1. Gán tên scene cần tải vào biến static
        sceneToLoad = mainGameSceneName;

        // 2. Tải scene màn hình chờ
        SceneManager.LoadScene("LoadingScreen");
    }

    public void OpenSettings()
    {
        Debug.Log("Mở Cài đặt!");
        // Thêm logic mở panel cài đặt ở đây
    }

    public void QuitGame()
    {
        Debug.Log("Thoát Game!");
        Application.Quit();
    }

    // --- LOGIC HIỆU ỨNG NHẤP NHÁY ---

    private void AddHoverEffect(Button button)
    {
        // Thêm CanvasGroup nếu chưa có để làm mờ cả nút
        if (button.GetComponent<CanvasGroup>() == null)
        {
            button.gameObject.AddComponent<CanvasGroup>();
        }

        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>() ?? button.gameObject.AddComponent<EventTrigger>();
        trigger.triggers.Clear(); // Xóa các trigger cũ để tránh trùng lặp

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
        // Dừng coroutine cũ nếu có trước khi bắt đầu cái mới
        if (activeBlinkCoroutine != null) StopCoroutine(activeBlinkCoroutine);
        activeBlinkCoroutine = StartCoroutine(BlinkEffect(canvasGroup));
    }

    private void OnPointerExit(CanvasGroup canvasGroup)
    {
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
            // Dùng hàm Sin để tạo hiệu ứng nhấp nháy lên xuống mượt mà
            canvasGroup.alpha = Mathf.Lerp(minAlpha, 1f, (Mathf.Sin(Time.time * blinkSpeed) + 1f) / 2f);
            yield return null;
        }
    }
}