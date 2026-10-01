using UnityEngine;

public class DamageReflect : MonoBehaviour
{
    public static DamageReflect Instance { get; private set; }

    private float reflectPercent = 0f;

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
        RefreshReflect();
    }

    public void RefreshReflect()
    {
        reflectPercent = PlayerPrefs.GetFloat("Bonus_DamageReflect", 0f);
        Debug.Log($"Отражение урона обновлено: {reflectPercent * 100}%");
    }

    public void UpgradeReflect(float newPercent)
    {
        reflectPercent = newPercent;
        PlayerPrefs.SetFloat("Bonus_DamageReflect", reflectPercent);
        PlayerPrefs.Save();
        Debug.Log($"Отражение урона улучшено: {reflectPercent * 100}%");
    }

    public void OnPlayerTakeDamage(int damage, GameObject attacker)
    {
        if (reflectPercent <= 0) return;
        if (attacker == null) return;

        int reflectDamage = Mathf.RoundToInt(damage * reflectPercent);
        if (reflectDamage <= 0) return;

        Health enemyHealth = attacker.GetComponent<Health>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(reflectDamage);
            Debug.Log($"Отражено {reflectDamage} урона обратно");
        }
    }

    public float GetReflectPercent() => reflectPercent;
}