using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    // Kéo 2 thanh trượt từ Hierarchy vào đây trong Inspector
    public Slider musicSlider;
    public Slider sfxSlider;

    void Start()
    {
        // 1. Lấy giá trị âm lượng đã lưu để gán cho thanh trượt khi bắt đầu
        // Nếu chưa từng lưu, giá trị mặc định sẽ là 1 (lớn nhất)
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);

        // 2. Lắng nghe sự kiện khi người dùng kéo thanh trượt
        // Mỗi khi thanh trượt thay đổi, hàm tương ứng sẽ được gọi
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void SetMusicVolume(float volume)
    {
        // Ra lệnh cho AudioManager thay đổi âm lượng nhạc
        if (AudioManager.instance != null)
        {
            AudioManager.instance.SetMusicVolume(volume);
        }
    }

    public void SetSFXVolume(float volume)
    {
        // Ra lệnh cho AudioManager thay đổi âm lượng SFX
        if (AudioManager.instance != null)
        {
            AudioManager.instance.SetSFXVolume(volume);
        }
    }
}