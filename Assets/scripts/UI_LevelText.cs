using UnityEngine;
using TMPro;

public class UI_LevelText : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private TextMeshProUGUI _levelText;

    private void Start()
    {
        // Находим текстовый компонент если не задан
        if (_levelText == null)
            _levelText = GetComponent<TextMeshProUGUI>();

        // Подписываемся на события
        if (ExpManager.Instance != null)
        {
            ExpManager.Instance.OnExpChanged.AddListener(OnExpChanged);
        }

        // Устанавливаем начальное значение
        UpdateLevelText();
    }

    private void OnDestroy()
    {
        // Отписываемся при уничтожении
        if (ExpManager.Instance != null)
        {
            ExpManager.Instance.OnExpChanged.RemoveListener(OnExpChanged);
        }
    }

    // Вызывается при изменении опыта
    private void OnExpChanged(int currentExp, int currentLevel)
    {
        UpdateLevelText();
    }

    // Обновить текст уровня
    private void UpdateLevelText()
    {
        if (_levelText == null || ExpManager.Instance == null) return;

        _levelText.text = ExpManager.Instance.CurrentLevel.ToString();
    }
}
