using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LoadingScreenController : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider progressBar;
    public TextMeshProUGUI progressText;

    [Header("Settings")]
    // Đặt thời gian tải tối thiểu (tính bằng giây) mà bạn muốn
    public float minimumLoadTime = 2.0f;

    void Start()
    {
        StartCoroutine(LoadSceneAsync());
    }

    IEnumerator LoadSceneAsync()
    {
        // Bắt đầu đếm giờ
        float elapsedTime = 0f;

        // Bắt đầu tải scene trong nền
        AsyncOperation operation = SceneManager.LoadSceneAsync(MenuManager.sceneToLoad);
        operation.allowSceneActivation = false;

        // Vòng lặp này đảm bảo thanh tiến trình chạy trong ít nhất 'minimumLoadTime' giây
        while (elapsedTime < minimumLoadTime)
        {
            elapsedTime += Time.deltaTime;

            // Tính toán tiến trình "ảo" dựa trên thời gian đã trôi qua
            float fakeProgress = Mathf.Clamp01(elapsedTime / minimumLoadTime);

            // Cập nhật giao diện với tiến trình "ảo" này
            progressBar.value = fakeProgress;
            if (progressText != null)
            {
                progressText.text = Mathf.RoundToInt(fakeProgress * 100) + "%";
            }

            yield return null; // Chờ đến frame tiếp theo
        }

 
        while (operation.progress < 0.9f)
        {
            yield return null;
        }

        // Cho phép kích hoạt scene mới
        operation.allowSceneActivation = true;
    }
}