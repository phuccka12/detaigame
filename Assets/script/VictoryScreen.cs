using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VictoryScreen : MonoBehaviour
{
    public GameObject victoryPanel;
    public TextMeshProUGUI finalTimeText; // <-- SỬA LẠI Ở ĐÂY
    public TextMeshProUGUI bestTimeText;
    public Button mainMenuButton;

    void Start()
    {
        mainMenuButton.onClick.AddListener(ReturnToMenu);
        if (victoryPanel != null) victoryPanel.SetActive(false);
    }

    public void Show(float finalTime, float bestTime)
    {
        if (victoryPanel != null) victoryPanel.SetActive(true);
        Time.timeScale = 0f; // Dừng game

        finalTimeText.text = "Thời gian: " + FormatTime(finalTime);
        bestTimeText.text = "Kỷ lục: " + FormatTime(bestTime);
    }

    private string FormatTime(float time)
    {
        int minutes = (int)time / 60;
        int seconds = (int)time % 60;
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void ReturnToMenu()
    {
        Time.timeScale = 1f;
        if (GameManager.instance != null) GameManager.instance.ReturnToMainMenu();
    }
}