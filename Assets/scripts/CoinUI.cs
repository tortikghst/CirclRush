using UnityEngine;

using TMPro;

public class CoinUI : MonoBehaviour
{
    [Header("Ссылки")]
    [SerializeField] private TextMeshProUGUI coinsText;
    
    void Start()
    {
        // Получаем текущее значение
        if (CoinManager.Instance != null)
        {
            UpdateDisplay(CoinManager.Instance.GetCoinsCount());
        }
        else
        {
            coinsText.text = "0";
        }
    }
    
    void OnEnable()
    {
        // Подписываемся на событие
        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.OnCoinsChanged.AddListener(UpdateDisplay);
        }
    }
    
    void OnDisable()
    {
        // Отписываемся
        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.OnCoinsChanged.RemoveListener(UpdateDisplay);
        }
    }
    
    public void UpdateDisplay(int newAmount)
    {
        coinsText.text = newAmount.ToString();
        
        // мб анимка
    }
}

