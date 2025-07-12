using UnityEngine;

public class AudioManager : PersistentSingleton<AudioManager>
{
    [Header("Audio Sources")]
    // Kéo 2 đối tượng con ở trên vào đây
    public AudioSource musicSource;
    public AudioSource sfxSource;

    void Start()
    {
        // Khi game bắt đầu, tải và áp dụng các cài đặt đã lưu
        LoadVolumeSettings();
    }

    void LoadVolumeSettings()
    {
        // Lấy giá trị đã lưu, nếu không có thì mặc định là 1 (to nhất)
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);

        // Áp dụng trực tiếp vào volume của AudioSource
        SetMusicVolume(musicVolume);
        SetSFXVolume(sfxVolume);
    }

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    // Hàm tiện ích để các script khác có thể phát hiệu ứng âm thanh
    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }
}