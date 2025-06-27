// Cần thêm dòng này để có thể sử dụng các chức năng quản lý Scene
using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    // Hàm này sẽ được gọi bởi nút START GAME / PLAY
    public void PlayGame()
    {
        // In ra Console để kiểm tra xem nút có hoạt động không
        Debug.Log("Bắt đầu chơi!");

        // Lệnh quan trọng nhất: Tải Scene có tên là "GameScene"
        // Hãy chắc chắn tên "GameScene" viết chính xác 100% giống với tên file Scene của bạn.
        SceneManager.LoadScene("map2");
    }

    // Hàm này sẽ được gọi bởi nút SETTING
    public void OpenSettings()
    {
        // Tạm thời chỉ in ra Console
        Debug.Log("Mở màn hình cài đặt!");
        // Sau này bạn có thể code để bật một panel cài đặt ở đây
    }

    // Hàm này sẽ được gọi bởi nút EXIT
    public void ExitGame()
    {
        Debug.Log("Thoát game!");

        // Lệnh này sẽ đóng ứng dụng (chỉ hoạt động khi game đã được build)
        Application.Quit();
    }
}