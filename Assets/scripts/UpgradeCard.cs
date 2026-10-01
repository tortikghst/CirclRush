using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeCard : MonoBehaviour
{
    public ShopUpgrade upgradeData;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI valueText;
    public TextMeshProUGUI priceText;
    public Button buyButton;
    public TextMeshProUGUI buttonText;

    private ShopManager shopManager;

    void Start()
    {
        shopManager = ShopManager.Instance;
        if (buyButton != null) buyButton.onClick.AddListener(OnBuyClick);
        UpdateCardDisplay();
    }

    public void UpdateCardDisplay()
    {
        if (upgradeData == null || shopManager == null) return;

        int level = shopManager.GetCurrentLevel(upgradeData.upgradeType);
        LevelData next = shopManager.GetNextLevel(upgradeData);

        if (nameText != null) nameText.text = upgradeData.upgradeName;

        if (level == 0)
        {
            if (valueText != null) valueText.text = $"0 → {upgradeData.levels[0].value}";
        }
        else if (level < upgradeData.levels.Length)
        {
            float oldVal = upgradeData.levels[level - 1].value;
            float newVal = upgradeData.levels[level].value;
            if (valueText != null) valueText.text = $"{oldVal} → {newVal}";
        }
        else
        {
            if (valueText != null) valueText.text = "MAX";
        }

        if (next != null)
        {
            if (priceText != null) priceText.text = next.price.ToString();
            if (buyButton != null) buyButton.interactable = true;
            if (buttonText != null) buttonText.text = "Купить";
        }
        else
        {
            if (priceText != null) priceText.text = "";
            if (buyButton != null) buyButton.interactable = false;
            if (buttonText != null) buttonText.text = "MAX";
        }
    }

    void OnBuyClick()
    {
        if (shopManager == null || upgradeData == null) return;
        if (shopManager.TryPurchaseUpgrade(upgradeData))
            UpdateCardDisplay();
    }
}