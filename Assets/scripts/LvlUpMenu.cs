using UnityEngine;

public class LevelUpMenu : MonoBehaviour
{
    [Header("UI Canvas")]
    [SerializeField] private GameObject _levelUpCanvas;
    [SerializeField] private bool _showCursor = true;
    [SerializeField] private CursorLockMode _cursorMode = CursorLockMode.None;

    [Header("Настройки")]
    [SerializeField] private float _timeScaleWhenOpen = 0f;
    [SerializeField] private bool _pauseGame = true;

    private bool _isMenuOpen = false;
    private float _previousTimeScale = 1f;
    private CursorLockMode _previousCursorMode;
    private bool _previousCursorVisible;

    void Start()
    {
        if (_levelUpCanvas != null)
            _levelUpCanvas.SetActive(false);

        // ПОДПИСКА НА СОБЫТИЕ
        if (ExpManager.Instance != null)
        {
            // Подписываемся на событие открытия меню
            ExpManager.Instance.OnLevelUpMenuRequest.AddListener(OpenLevelUpMenu);
            Debug.Log("LevelUpMenu подписался на события ExpManager");
        }
        else
        {
            Debug.LogError("ExpManager не найден!");
        }
    }

    void OnDestroy()
    {
        // ОТПИСКА при уничтожении
        if (ExpManager.Instance != null)
        {
            ExpManager.Instance.OnLevelUpMenuRequest.RemoveListener(OpenLevelUpMenu);
        }
        
        // Восстанавливаем время
        if (_isMenuOpen)
        {
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    // Метод для открытия меню (вызывается через событие)
    public void OpenLevelUpMenu(int newLevel)
    {
        if (_isMenuOpen) return;

        _isMenuOpen = true;

        // Сохраняем состояние
        _previousTimeScale = Time.timeScale;
        _previousCursorMode = Cursor.lockState;
        _previousCursorVisible = Cursor.visible;

        // Пауза
        if (_pauseGame)
        {
            Time.timeScale = _timeScaleWhenOpen;
        }

        // Курсор
        if (_showCursor)
        {
            Cursor.lockState = _cursorMode;
            Cursor.visible = true;
        }

        // Показываем UI
        if (_levelUpCanvas != null)
        {
            _levelUpCanvas.SetActive(true);
        }

        Debug.Log($"Level Up Menu открыто (уровень {newLevel})");
    }

    public void CloseLevelUpMenu()
    {
        if (!_isMenuOpen) return;

        _isMenuOpen = false;

        // Восстанавливаем
        Time.timeScale = _previousTimeScale;
        Cursor.lockState = _previousCursorMode;
        Cursor.visible = _previousCursorVisible;

        // Скрываем UI
        if (_levelUpCanvas != null)
        {
            _levelUpCanvas.SetActive(false);
        }

        Debug.Log("Level Up Menu закрыто");
    }

    // Методы для кнопок
    public void SelectUpgrade(string upgradeType)
    {
        
        
        switch (upgradeType)
        {
            case "Skill1":
                
                break;
            case "Skill2":
                
                break;
            case "Skill3":
                
                break;
        }
        
        CloseLevelUpMenu();
    }
}