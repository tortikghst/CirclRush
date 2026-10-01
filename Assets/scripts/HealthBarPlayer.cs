using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBarPlayer : MonoBehaviour
{
    [Header("Компоненты")]
    public Image fillImage;
    public TextMeshProUGUI hpText;

    [Header("Настройки")]
    public string textFormat = "{0}/{1}";

    private Health playerHealth;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("HealthBarPlayer: Player not found!");
            return;
        }

        playerHealth = player.GetComponent<Health>();
        if (playerHealth == null)
        {
            Debug.LogError("HealthBarPlayer: Health component not found on Player!");
            return;
        }

        // Подписываемся на событие
        playerHealth.OnHealthChanged.AddListener(UpdateBar);
        
        // Первоначальное обновление
        UpdateBar();
    }

    public void UpdateBar()
    {
        if (playerHealth == null) return;

        if (fillImage != null)
        {
            float fillAmount = playerHealth.currentHealth / playerHealth.maxHealth;
            fillImage.fillAmount = fillAmount;
        }

        if (hpText != null)
        {
            hpText.text = string.Format(textFormat, playerHealth.currentHealth, playerHealth.maxHealth);
        }
    }

    void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged.RemoveListener(UpdateBar);
        }
    }
}