using UnityEngine;
using UnityEngine.Events;

public class CoinManager : MonoBehaviour
{
    // === 1. SINGLETON (только для текущего забега) ===
    private static CoinManager _instance;
    public static CoinManager Instance => _instance;
    
    // === 2. СОБЫТИЯ ЧЕРЕЗ UNITYEVENT ===
    [System.Serializable]
    public class IntEvent : UnityEvent<int> { }
    
    [Header("События")]
    public IntEvent OnCoinsChanged;
    
    // === 3. ДАННЫЕ ===
    [Header("Настройки")]
    [SerializeField] private int _currentCoins = 0;
    [SerializeField] private int _totalCoinsCollectedThisRun = 0; 
    
    // === 4. МЕТОДЫ ЖИЗНЕННОГО ЦИКЛА ===
    private void Awake()
    {
        // Проверяем, не существует ли уже другой CoinManager
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        // Сохраняем этот экземпляр как единственный
        _instance = this;
        
        // НЕ используем DontDestroyOnLoad - сбрасывается при новой сцене
    }
    
    private void Start()
    {
        // Сбрасываем при старте забега
        ResetCoins();
        
        // Уведомляем всех подписчиков о текущем количестве
        NotifyCoinsChanged();
    }
    
    // === 5. ОСНОВНЫЕ МЕТОДЫ ===
    
    // Добавить монеты
    public void AddCoins(int amount)
    {
        _currentCoins += amount;
        _totalCoinsCollectedThisRun += amount;
        // Уведомляем всех подписчиков
        NotifyCoinsChanged();
        
        // эффекты если успею
        PlayCoinAddedEffects(amount);
    }
    
    // Потратить монеты
    public bool SpendCoins(int amount)
    {
        // Проверяем, достаточно ли монет
        if (_currentCoins >= amount)
        {
            _currentCoins -= amount;
            NotifyCoinsChanged();
            //эфект и звук мб
            PlayCoinSpentEffects(amount);
            
            return true; // Успешно потратили
        }
        else
        {
            //вывести что недостаточно и проиграть эффект
            PlayNotEnoughCoinsEffects();
            
            return false; // Не удалось потратить
        }
    }
    
    // Получить текущее количество монет
    public int GetCoinsCount()
    {
        return _currentCoins;
    }
    
    // Получить общее количество собранных в этом забеге
    public int GetTotalCoinsCollectedThisRun()
    {
        return _totalCoinsCollectedThisRun;
    }
    
    // Сбросить монеты (при начале нового забега)
    public void ResetCoins()
    {
        _currentCoins = 0;
        _totalCoinsCollectedThisRun = 0;
        NotifyCoinsChanged();
    }
    
    // === 6. ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ ===
    
    private void NotifyCoinsChanged()
    {
        // Вызываем UnityEvent
        OnCoinsChanged?.Invoke(_currentCoins);
    }
    
    private void PlayCoinAddedEffects(int amount)
    {
        // - Визуальные эффекты для UI
        // - Звуки сбора монет
    }
    
    private void PlayCoinSpentEffects(int amount)
    {
        // Эффекты при трате монет
        // Звук покупки, анимация UI
    }
    
    private void PlayNotEnoughCoinsEffects()
    {
        // Эффекты при недостатке монет
        // Красное мигание UI, звук отказа
    }
    
    // === 7. ДОПОЛНИТЕЛЬНЫЕ МЕТОДЫ ===
    
    // Проверить, достаточно ли монет
    public bool HasEnoughCoins(int amount)
    {
        return _currentCoins >= amount;
    }
    
    // Получить монеты с множителем (для бонусов)
    public void AddCoinsWithMultiplier(int baseAmount, float multiplier)
    {
        int finalAmount = Mathf.RoundToInt(baseAmount * multiplier);
        AddCoins(finalAmount);
    }
}