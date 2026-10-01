using UnityEngine;
using UnityEngine.Events;
using System;

public class Health : MonoBehaviour
{
    [Header("Здоровье")]
    public float maxHealth = 250f;
    
    private float _currentHealth;
    public float currentHealth 
    { 
        get => _currentHealth;
        private set 
        { 
            _currentHealth = value;
            OnHealthChanged?.Invoke();
        }
    }

    public float hp { get => maxHealth; set => maxHealth = value; }
    public float currentHp { get => currentHealth; set => currentHealth = value; }
    public float damageResist { get; set; } = 1f;

    [Header("События")]
    public UnityEvent OnHealthChanged;
    public UnityEvent OnDie;
    public UnityEvent OnTakeDamage;

    [Header("Настройки")]
    public bool isPlayer = false;
    public bool autoDestroy = true;
    public bool isFinalBoss = false;
    public bool isBoss = false;  // добавим флаг для боссов

    [Header("Бонусы от магазина")]
    private int bonusMaxHealth = 0;

    public static Health PlayerInstance { get; private set; }
    public static event Action OnAnyBossDied;

    void Start()
    {
        if (isPlayer)
        {
            PlayerInstance = this;
            LoadAllBonuses();
            Debug.Log($"Health: Start завершён, maxHealth={maxHealth}, currentHealth={currentHealth}");
        }
        else
        {
            currentHealth = maxHealth;
        }
    }

    void LoadAllBonuses()
    {
        RefreshHealthBonus();
        RefreshDamageResist();
    }

    public void RefreshHealthBonus()
    {
        bonusMaxHealth = PlayerPrefs.GetInt("Bonus_MaxHealth", 0);
        maxHealth = 250 + bonusMaxHealth;
        currentHealth = maxHealth;
        Debug.Log($"Здоровье обновлено: {maxHealth} (база 250 + бонус {bonusMaxHealth})");
    }

    public void UpgradeMaxHealth(int newBonus)
    {
        bonusMaxHealth = newBonus;
        PlayerPrefs.SetInt("Bonus_MaxHealth", bonusMaxHealth);
        PlayerPrefs.Save();
        RefreshHealthBonus();
    }

    public void RefreshDamageResist()
    {
        float bonus = PlayerPrefs.GetFloat("Bonus_DamageResist", 0f);
        damageResist = 1f - Mathf.Clamp01(bonus);
        Debug.Log($"Сопротивление обновлено: {damageResist} (бонус: {bonus})");
    }

    public void UpgradeDamageResist(float newBonus)
    {
        PlayerPrefs.SetFloat("Bonus_DamageResist", newBonus);
        PlayerPrefs.Save();
        RefreshDamageResist();
    }

    public void Hit(float amount, GameObject source = null)
    {
        if (currentHealth <= 0) return;

        if (isPlayer && source != null)
        {
            if (source.CompareTag("Player") || source.CompareTag("Player_bullet"))
            {
                Debug.Log("Игрок не может наносить урон сам себе");
                return;
            }
        }

        float damage = amount * damageResist;
        
        if (damage <= 0) return;
        
        currentHealth -= damage;
        
        OnTakeDamage?.Invoke();

        // Звук получения урона для игрока
        if (isPlayer && currentHealth > 0)
        {
            AudioManager.Instance?.PlayPlayerHurt();
        }
        
        // Звук получения урона для врагов (не игрок, не босс)
        if (!isPlayer && !isBoss && !isFinalBoss && currentHealth > 0)
        {
            AudioManager.Instance?.PlayEnemyHit();
        }

        if (currentHealth <= 0)
        {
            // Звук смерти
            if (isPlayer)
            {
                AudioManager.Instance?.PlayPlayerDeath();
            }
            else if (isFinalBoss)
            {
                AudioManager.Instance?.PlayBossDeath();
            }
            else if (isBoss)
            {
                AudioManager.Instance?.PlayBossDeath();
            }
            else
            {
                AudioManager.Instance?.PlayEnemyDeath();
            }
            
            OnDie?.Invoke();
            if (isFinalBoss) OnAnyBossDied?.Invoke();
            if (autoDestroy) Destroy(gameObject);
        }
    }

    public void Hit(float amount) => Hit(amount, null);
    public void TakeDamage(int damage) => Hit(damage);
    public void TakeDamage(int damage, GameObject source) => Hit(damage, source);
    public void Heal(float amount) => currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    public void Restore() => currentHealth = maxHealth;

    void OnDestroy()
    {
        if (isPlayer && PlayerInstance == this) PlayerInstance = null;
    }
}