using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

    public TextMeshProUGUI totalGoldText;
    public Button backButton;
    public List<ShopUpgrade> allUpgrades;
    
    private Dictionary<UpgradeType, int> purchasedLevels = new Dictionary<UpgradeType, int>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
    }

    void Start()
    {
        Time.timeScale = 1f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        AudioManager.Instance?.PlayShopOpen(); // звук открытия магазина
        LoadPurchasedLevels();
        
        if (GoldManager.Instance != null)
        {
            GoldManager.Instance.OnGoldChanged += UpdateGoldDisplay;
            UpdateGoldDisplay();
        }

        if (backButton != null)
        {
            backButton.onClick.RemoveAllListeners();
            backButton.onClick.AddListener(GoToMainMenu);
        }
        
        UpdateAllCards();
        RefreshAllBonuses();
    }

    void OnDestroy()
    {
        if (GoldManager.Instance != null)
            GoldManager.Instance.OnGoldChanged -= UpdateGoldDisplay;
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("main");
    }

    void UpdateAllCards()
    {
        UpgradeCard[] cards = FindObjectsByType<UpgradeCard>(FindObjectsSortMode.None);
        foreach (var card in cards)
        {
            card.UpdateCardDisplay();
        }
    }

    void LoadPurchasedLevels()
    {
        foreach (var upgrade in allUpgrades)
        {
            int level = PlayerPrefs.GetInt($"Shop_{upgrade.upgradeType}", 0);
            if (level > 0) purchasedLevels[upgrade.upgradeType] = level;
        }
    }

    public int GetCurrentLevel(UpgradeType type) =>
        purchasedLevels.ContainsKey(type) ? purchasedLevels[type] : 0;

    public LevelData GetNextLevel(ShopUpgrade upgrade)
    {
        int current = GetCurrentLevel(upgrade.upgradeType);
        return current < upgrade.levels.Length ? upgrade.levels[current] : null;
    }

   public bool TryPurchaseUpgrade(ShopUpgrade upgrade)
{
    int current = GetCurrentLevel(upgrade.upgradeType);
    if (current >= upgrade.levels.Length)
    {
        AudioManager.Instance?.PlayError();
        return false;
    }

    LevelData next = upgrade.levels[current];
    if (GoldManager.Instance == null || !GoldManager.Instance.SpendGold(next.price))
    {
        AudioManager.Instance?.PlayError();
        return false;
    }

    purchasedLevels[upgrade.upgradeType] = current + 1;
    PlayerPrefs.SetInt($"Shop_{upgrade.upgradeType}", current + 1);
    PlayerPrefs.Save();

    Debug.Log($"{upgrade.upgradeName} улучшен до {next.value}");
    
    AudioManager.Instance?.PlayPurchase(); // звук успешной покупки
    
    ApplyUpgradeEffect(upgrade, next.value);
    
    return true;
}

    void RefreshAllBonuses()
    {
        if (Health.PlayerInstance != null)
        {
            Health.PlayerInstance.RefreshHealthBonus();
            Health.PlayerInstance.RefreshDamageResist();
        }
        
        if (HealthRegen.Instance != null)
            HealthRegen.Instance.RefreshRegen();
        
        if (PlayerBehavour.Instance != null)
            PlayerBehavour.Instance.RefreshSpeed();
        
        if (DamageReflect.Instance != null)
            DamageReflect.Instance.RefreshReflect();
    }

    void ApplyUpgradeEffect(ShopUpgrade upgrade, float newValue)
    {
        switch (upgrade.upgradeType)
        {
            case UpgradeType.MaxHealth:
                int healthBonus = Mathf.RoundToInt(newValue);
                PlayerPrefs.SetInt("Bonus_MaxHealth", healthBonus);
                PlayerPrefs.Save();
                if (Health.PlayerInstance != null)
                    Health.PlayerInstance.RefreshHealthBonus();
                break;
                
            case UpgradeType.HealthRegen:
                float regenBonus = newValue;
                PlayerPrefs.SetFloat("Bonus_HealthRegen", regenBonus);
                PlayerPrefs.Save();
                if (HealthRegen.Instance != null)
                    HealthRegen.Instance.RefreshRegen();
                break;
                
            case UpgradeType.Damage:
                float damageBonus = newValue;
                PlayerPrefs.SetFloat("Bonus_Damage", damageBonus);
                PlayerPrefs.Save();
                break;
                
            case UpgradeType.MoveSpeed:
                float speedBonus = newValue;
                PlayerPrefs.SetFloat("Bonus_MoveSpeed", speedBonus);
                PlayerPrefs.Save();
                if (PlayerBehavour.Instance != null)
                    PlayerBehavour.Instance.RefreshSpeed();
                break;
                
            case UpgradeType.CooldownReduction:
                float cooldownBonus = newValue;
                PlayerPrefs.SetFloat("Bonus_CooldownReduction", cooldownBonus);
                PlayerPrefs.Save();
                break;
                
            case UpgradeType.Range:
                float rangeBonus = newValue;
                PlayerPrefs.SetFloat("Bonus_Range", rangeBonus);
                PlayerPrefs.Save();
                break;
                
            case UpgradeType.DamageResist:
                float resistBonus = newValue;
                PlayerPrefs.SetFloat("Bonus_DamageResist", resistBonus);
                PlayerPrefs.Save();
                if (Health.PlayerInstance != null)
                    Health.PlayerInstance.RefreshDamageResist();
                break;
                
            case UpgradeType.DamageReflect:
                float reflectBonus = newValue;
                PlayerPrefs.SetFloat("Bonus_DamageReflect", reflectBonus);
                PlayerPrefs.Save();
                if (DamageReflect.Instance != null)
                    DamageReflect.Instance.RefreshReflect();
                break;
                
            case UpgradeType.ExpMultiplier:
                float expBonus = newValue;
                PlayerPrefs.SetFloat("Bonus_ExpMultiplier", expBonus);
                PlayerPrefs.Save();
                break;
                
            case UpgradeType.PickupRadius:
                float radiusBonus = newValue;
                PlayerPrefs.SetFloat("Bonus_PickupRadius", radiusBonus);
                PlayerPrefs.Save();
                break;
                
            case UpgradeType.SkillSwap:
                int currentSwapBonus = PlayerPrefs.GetInt("Bonus_SkillSwap", 0);
                int addedSwapBonus = Mathf.RoundToInt(newValue);
                int totalSwapBonus = currentSwapBonus + addedSwapBonus;
                PlayerPrefs.SetInt("Bonus_SkillSwap", totalSwapBonus);
                PlayerPrefs.Save();
                Debug.Log($"Бонус замены сохранён: {totalSwapBonus}");
                break;
                
            case UpgradeType.ProjectileCount:
                int projectileBonus = Mathf.RoundToInt(newValue);
                PlayerPrefs.SetInt("Bonus_ProjectileCount", projectileBonus);
                PlayerPrefs.Save();
                break;
        }
    }

    void UpdateGoldDisplay()
    {
        if (totalGoldText != null && GoldManager.Instance != null)
            totalGoldText.text = $"{GoldManager.Instance.totalGold} ";
    }
    
}