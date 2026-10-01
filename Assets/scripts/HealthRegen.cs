using UnityEngine;

public class HealthRegen : MonoBehaviour
{
    public static HealthRegen Instance { get; private set; }

    private float regenAmount = 0f;
    private float regenInterval = 2f;
    private float timer = 0f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        RefreshRegen();
    }

    public void RefreshRegen()
    {
        float bonus = PlayerPrefs.GetFloat("Bonus_HealthRegen", 0);
        SetRegen(bonus);
        Debug.Log($"Регенерация обновлена: {regenAmount} за {regenInterval} сек");
    }

    public void SetRegen(float amount)
    {
        regenAmount = amount;
        if (regenAmount <= 0) enabled = false;
        else enabled = true;
    }

    void Update()
    {
        if (Health.PlayerInstance == null) return;
        if (Health.PlayerInstance.currentHealth <= 0) return;

        timer += Time.deltaTime;
        if (timer >= regenInterval)
        {
            timer = 0f;
            Health.PlayerInstance.Heal(regenAmount);
        }
    }
}