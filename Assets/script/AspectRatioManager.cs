using UnityEngine;
using UnityEngine.SceneManagement;

public class AspectRatioManager : MonoBehaviour
{
    // Tạo một đối tượng duy nhất và giữ nó khi chuyển scene
    public static AspectRatioManager instance;

    // Thiết lập độ phân giải cho màn hình ngang (landscape)
    public int horizontalWidth = 1920;
    public int horizontalHeight = 1080;

    // Thiết lập độ phân giải cho màn hình dọc (portrait)
    public int verticalWidth = 1080;
    public int verticalHeight = 1920;

    void Awake()
    {
        // --- Logic để giữ đối tượng này tồn tại qua các scene ---
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            // Đăng ký lắng nghe sự kiện khi một scene được tải
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            // Nếu đã có một instance khác, hủy đối tượng này đi
            Destroy(gameObject);
            return;
        }

        // Chạy lần đầu khi game bắt đầu
        AdjustAspectRatio(SceneManager.GetActiveScene());
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AdjustAspectRatio(scene);
    }

    void AdjustAspectRatio(Scene scene)
    {
        // Lấy tên của scene hiện tại
        string sceneName = scene.name;

        // Kiểm tra nếu scene là "map8" thì chuyển sang màn hình ngang
        if (sceneName == "map8")
        {
            Debug.Log("Chuyển sang chế độ ngang cho " + sceneName);
            Screen.SetResolution(horizontalWidth, horizontalHeight, FullScreenMode.Windowed);
        }
        // Nếu không phải, chuyển về màn hình dọc
        else
        {
            Debug.Log("Chuyển sang chế độ dọc cho " + sceneName);
            Screen.SetResolution(verticalWidth, verticalHeight, FullScreenMode.Windowed);
        }
    }

    // Hủy đăng ký sự kiện khi đối tượng bị hủy
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}