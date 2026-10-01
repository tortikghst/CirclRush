using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndGameMenu : MonoBehaviour
{
    [Header("Панели")]
    public GameObject endGamePanel;      // основная панель

    [Header("Изображения")]
    public Image resultImage;            // компонент Image для смены картинки
    public Sprite victorySprite;         // картинка для победы
    public Sprite gameOverSprite;        // картинка для поражения

    [Header("Настройки")]
    public string gameSceneName = "Game";
    public string mainMenuName = "main";

    void Start()
    {
        // Скрываем панель при старте
        if (endGamePanel != null)
            endGamePanel.SetActive(false);
    }

    // Вызывается при победе
    public void ShowVictory()
    {
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Останавливаем игровую музыку и включаем победную
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopMusic();
            AudioManager.Instance.PlaySFX(AudioManager.Instance.victoryMusic);
        }

        if (resultImage != null && victorySprite != null)
            resultImage.sprite = victorySprite;

        if (endGamePanel != null)
            endGamePanel.SetActive(true);
    }

    public void ShowGameOver()
    {
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Останавливаем игровую музыку и включаем музыку поражения
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopMusic();
            AudioManager.Instance.PlaySFX(AudioManager.Instance.defeatMusic);
        }

        if (resultImage != null && gameOverSprite != null)
            resultImage.sprite = gameOverSprite;

        if (endGamePanel != null)
            endGamePanel.SetActive(true);
    }

    // Кнопка "Играть снова"
    public void RestartGame()
{
    AudioManager.Instance?.PlayButtonClick();
    Time.timeScale = 1f;
    SceneManager.LoadScene(gameSceneName);
}

public void GoToMainMenu()
{
    AudioManager.Instance?.PlayButtonClick();
    Time.timeScale = 1f;
    SceneManager.LoadScene(mainMenuName);
}
}