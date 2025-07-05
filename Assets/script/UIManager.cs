using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Image[] heartImages; // Kéo các Image trái tim của màn này vào đây

    void Start()
    {
        // Tìm GameManager đang tồn tại
        GameManager gm = GameManager.instance;

        if (gm != null)
        {
            // Gán lại mảng UI cho GameManager và cập nhật hiển thị
            gm.heartImages = this.heartImages;
            gm.UpdateHeartUI();
        }
    }
}