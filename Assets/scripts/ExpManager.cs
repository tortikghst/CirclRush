using UnityEngine;
using UnityEngine.Events;

public class ExpManager : MonoBehaviour
{
    private static ExpManager _instance;
    public static ExpManager Instance => _instance;

    [System.Serializable]
    public class ExpChangedEvent : UnityEvent<int, int> { }
    [System.Serializable]
    public class LevelUpEvent : UnityEvent<int> { }
    [System.Serializable]
    public class LevelUpMenuEvent : UnityEvent<int> { }

    [Header("События")]
    public ExpChangedEvent OnExpChanged;
    public LevelUpEvent OnLevelUp;
    public LevelUpMenuEvent OnLevelUpMenuRequest;

    [Header("Текущие значения")]
    [SerializeField] private int _currentExp = 0;
    [SerializeField] private int _currentLevel = 1;
    
    [Header ("Настройки прогрессии (Вариант Б)")]
    [SerializeField] private AnimationCurve expCurve;
    
    private int[] _expRequirementsCache;
    private const int MAX_LEVEL = 41;

    public int CurrentExp => _currentExp;
    public int CurrentLevel => _currentLevel;
    public int ExpToNextLevel => GetExpRequiredForLevel(_currentLevel + 1);
    
    public float ExpProgress 
    {
        get
        {
            if (_currentLevel >= MAX_LEVEL) return 1f;
            int expNeeded = ExpToNextLevel;
            if (expNeeded <= 0) return 1f;
            return Mathf.Clamp01((float)_currentExp / expNeeded);
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        InitializeExpCache();
    }

    private void Start()
    {
        ResetExp();
    }

    private void InitializeExpCache()
    {
        _expRequirementsCache = new int[MAX_LEVEL + 1];
        _expRequirementsCache[1] = 0;
        
        // Вариант Б: Ступенчатая прогрессия до 41 уровня
        for (int level = 2; level <= MAX_LEVEL; level++)
        {
            int exp = 0;
            
            if (level <= 5) // 2-5: быстрый старт
                exp = 5 + (level - 2) * 3; // 5, 8, 11, 14
            else if (level <= 10) // 6-10: умеренный рост
                exp = 18 + (level - 6) * 4; // 18, 22, 26, 30, 34
            else if (level <= 15) // 11-15: средний
                exp = 38 + (level - 11) * 5; // 38, 43, 48, 53, 58
            else if (level <= 20) // 16-20: выше среднего
                exp = 63 + (level - 16) * 6; // 63, 69, 75, 81, 87
            else if (level <= 25) // 21-25: высокий
                exp = 93 + (level - 21) * 7; // 93, 100, 107, 114, 121
            else if (level <= 30) // 26-30: очень высокий
                exp = 128 + (level - 26) * 8; // 128, 136, 144, 152, 160
            else if (level <= 35) // 31-35: экстремальный
                exp = 168 + (level - 31) * 9; // 168, 177, 186, 195, 204
            else // 36-41: максимальный
                exp = 213 + (level - 36) * 10; // 213, 223, 233, 243, 253, 263
            
            _expRequirementsCache[level] = exp;
        }
        
        // Создаём кривую для визуализации (опционально)
        CreateCurveFromCache();
    }
    
    private void CreateCurveFromCache()
    {
        Keyframe[] keys = new Keyframe[MAX_LEVEL];
        for (int i = 0; i < MAX_LEVEL; i++)
        {
            keys[i] = new Keyframe(i + 1, _expRequirementsCache[i + 1]);
        }
        expCurve = new AnimationCurve(keys);
    }

    public void AddExp(int amount)
    {
        if (amount <= 0 || _currentLevel >= MAX_LEVEL) return;
        
        _currentExp += amount;
        CheckLevelUp();
        UpdateExpDisplay();
    }

    private void CheckLevelUp()
{
    int requiredExp = GetExpRequiredForLevel(_currentLevel + 1);
    
    while (requiredExp > 0 && _currentExp >= requiredExp)
    {
        _currentExp -= requiredExp;
        _currentLevel++;
        
        AudioManager.Instance?.PlayLevelUp(); // <-- добавить
        
        OnLevelUp?.Invoke(_currentLevel);
        OnLevelUpMenuRequest?.Invoke(_currentLevel);
        
        requiredExp = GetExpRequiredForLevel(_currentLevel + 1);
    }
}

    public int GetExpRequiredForLevel(int targetLevel)
    {
        if (targetLevel <= 1 || targetLevel > MAX_LEVEL) return 0;
        
        if (targetLevel <= MAX_LEVEL && _expRequirementsCache != null)
        {
            return _expRequirementsCache[targetLevel];
        }
        
        return 0;
    }

    public void ResetExp()
    {
        _currentExp = 0;
        _currentLevel = 1;
        UpdateExpDisplay();
    }

    private void UpdateExpDisplay()
    {
        OnExpChanged?.Invoke(_currentExp, _currentLevel);
    }

    public float GetProgressPercentage()
    {
        if (_currentLevel >= MAX_LEVEL) return 1f;
        int expNeeded = ExpToNextLevel;
        if (expNeeded <= 0) return 1f;
        return Mathf.Clamp01((float)_currentExp / expNeeded);
    }

    [ContextMenu("Показать таблицу уровней")]
    public void DebugLevelTable()
    {
        Debug.Log("=== ТАБЛИЦА УРОВНЕЙ (1-41) ===");
        int total = 0;
        for (int i = 1; i <= MAX_LEVEL; i++)
        {
            int req = GetExpRequiredForLevel(i);
            if (i > 1) total += req;
            Debug.Log($"Уровень {i}: {req} опыта (всего до этого уровня: {total})");
        }
    }
}