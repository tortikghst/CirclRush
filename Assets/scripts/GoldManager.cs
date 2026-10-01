using UnityEngine;

public class GoldManager : MonoBehaviour
{
    public static GoldManager Instance { get; private set; }

    public int totalGold;
    public System.Action OnGoldChanged;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadGold();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void LoadGold()
    {
        totalGold = PlayerPrefs.GetInt("TotalGold", 0);
        Debug.Log($"GoldManager: загружено {totalGold} монет");
        OnGoldChanged?.Invoke();
    }

    void SaveGold()
    {
        PlayerPrefs.SetInt("TotalGold", totalGold);
        PlayerPrefs.Save();
        Debug.Log($"GoldManager: сохранено {totalGold} монет");
    }

    public void AddGold(int amount)
    {
        if (amount > 0)
        {
            totalGold += amount;
            OnGoldChanged?.Invoke();
            SaveGold(); // сохраняем сразу после добавления
        }
    }

    public bool SpendGold(int amount)
    {
        if (amount <= totalGold)
        {
            totalGold -= amount;
            OnGoldChanged?.Invoke();
            SaveGold(); // сохраняем после траты
            return true;
        }
        return false;
    }

    // Принудительная загрузка (для магазина)
    public void ReloadGold()
    {
        LoadGold();
    }
}