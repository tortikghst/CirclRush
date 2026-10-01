using UnityEngine;
using UnityEngine.UI;

public class WaveProgressBar : MonoBehaviour
{
    [Header("Ссылки")]
    public WaveManager waveManager;  // ссылка на WaveManager
    public Image fillImage;          // изображение заполнения бара

    [Header("Настройки")]
    public bool showTimeText = true; // показывать ли текст со временем
    public Text timeText;            // текст для отображения времени (опционально)

    void Update()
    {
        if (waveManager == null || fillImage == null) return;

        // Получаем прогресс от WaveManager
        float progress = waveManager.GetProgress();
        
        // Обновляем заполнение бара
        fillImage.fillAmount = progress;

        // Обновляем текст со временем
        if (showTimeText && timeText != null)
        {
            float currentTime = waveManager.GetCurrentTime();
            float totalTime = waveManager.totalGameTime;
            
            int minutes = Mathf.FloorToInt(currentTime / 60f);
            int seconds = Mathf.FloorToInt(currentTime % 60f);
            
            timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
}