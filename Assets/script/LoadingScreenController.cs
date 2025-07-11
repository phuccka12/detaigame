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
    public float minimumLoadTime = 2.0f;

    void Start()
    {
        // Lấy tên scene từ đúng nơi: GameManager
        string sceneToLoad = GameManager.nextSceneToLoad;

        // Kiểm tra xem tên scene có hợp lệ không
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            StartCoroutine(LoadSceneAsync(sceneToLoad));
        }
        else
        {
            // Nếu có lỗi, quay về Menu
            Debug.LogError("Lỗi: Không tìm thấy scene để tải. Quay về MenuStart.");
            SceneManager.LoadScene("MenuStart");
        }
    }

    IEnumerator LoadSceneAsync(string sceneName)
    {
        float elapsedTime = 0f;

        // CẬP NHẬT: Sử dụng biến sceneName đã được truyền vào
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        // Vòng lặp này đảm bảo thanh tiến trình chạy trong ít nhất 'minimumLoadTime' giây
        while (elapsedTime < minimumLoadTime)
        {
            elapsedTime += Time.deltaTime;
            float fakeProgress = Mathf.Clamp01(elapsedTime / minimumLoadTime);

            // Cập nhật giao diện với tiến trình "ảo"
            progressBar.value = fakeProgress;
            if (progressText != null)
            {
                progressText.text = Mathf.RoundToInt(fakeProgress * 100) + "%";
            }

            yield return null;
        }

        // Vòng lặp này đợi cho đến khi scene thực sự tải gần xong
        while (operation.progress < 0.9f)
        {
            // Nếu bạn muốn thanh progress bar hiển thị tiến trình tải thật sau khi chạy xong tiến trình ảo
            // bạn có thể cập nhật progressBar.value = operation.progress ở đây.
            yield return null;
        }

        // Cập nhật lần cuối cho đẹp
        progressBar.value = 1f;
        if (progressText != null)
        {
            progressText.text = "100%";
        }

        // Cho phép kích hoạt scene mới
        operation.allowSceneActivation = true;
    }
}