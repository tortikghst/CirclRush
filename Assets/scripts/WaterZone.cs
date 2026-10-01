using UnityEngine;
using System.Collections.Generic;

public class WaterZone : MonoBehaviour
{
    [Header("Настройки")]
    public float duration = 5f;
    public float tickInterval = 1f;
    public int damagePerTick = 5;
    public float slowFactor = 0.5f;

    [Header("Цели")]
    public LayerMask enemyLayer;

    private HashSet<GameObject> enemiesInZone = new HashSet<GameObject>();
    private Dictionary<GameObject, float> originalSpeeds = new Dictionary<GameObject, float>();

   void Start()
{
    AudioManager.Instance?.PlayWaterSplash(); // один раз при появлении
    InvokeRepeating(nameof(DealDamage), 0f, tickInterval);
    Destroy(gameObject, duration);
}

    void DealDamage()
    {
        // Создаём копию списка для безопасного перебора
        List<GameObject> enemiesCopy = new List<GameObject>(enemiesInZone);
        
        foreach (var enemy in enemiesCopy)
        {
            if (enemy == null) continue;
            
            Health health = enemy.GetComponent<Health>();
            if (health != null) 
            {
                health.TakeDamage(damagePerTick, gameObject);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & enemyLayer) == 0) return;

        GameObject enemy = other.gameObject;
        
        if (enemiesInZone.Add(enemy))
        {
            ApplySlowToEnemy(enemy);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & enemyLayer) == 0) return;

        GameObject enemy = other.gameObject;
        
        if (enemiesInZone.Remove(enemy))
        {
            RemoveSlowFromEnemy(enemy);
        }
    }

    void ApplySlowToEnemy(GameObject enemy)
    {
        // Пробуем SimpleEnemy (4 направления)
        SimpleEnemy simpleEnemy = enemy.GetComponent<SimpleEnemy>();
        if (simpleEnemy != null)
        {
            if (!originalSpeeds.ContainsKey(enemy))
            {
                originalSpeeds[enemy] = simpleEnemy.moveSpeed;
            }
            simpleEnemy.moveSpeed = originalSpeeds[enemy] * slowFactor;
            return;
        }
        
        // Пробуем SimpleEnemy2Dir (2 направления)
        SimpleEnemy2Dir simpleEnemy2Dir = enemy.GetComponent<SimpleEnemy2Dir>();
        if (simpleEnemy2Dir != null)
        {
            if (!originalSpeeds.ContainsKey(enemy))
            {
                originalSpeeds[enemy] = simpleEnemy2Dir.moveSpeed;
            }
            simpleEnemy2Dir.moveSpeed = originalSpeeds[enemy] * slowFactor;
        }
    }

    void RemoveSlowFromEnemy(GameObject enemy)
    {
        SimpleEnemy simpleEnemy = enemy.GetComponent<SimpleEnemy>();
        if (simpleEnemy != null && originalSpeeds.ContainsKey(enemy))
        {
            simpleEnemy.moveSpeed = originalSpeeds[enemy];
            originalSpeeds.Remove(enemy);
            return;
        }
        
        SimpleEnemy2Dir simpleEnemy2Dir = enemy.GetComponent<SimpleEnemy2Dir>();
        if (simpleEnemy2Dir != null && originalSpeeds.ContainsKey(enemy))
        {
            simpleEnemy2Dir.moveSpeed = originalSpeeds[enemy];
            originalSpeeds.Remove(enemy);
        }
    }

    void OnDestroy()
    {
        // Возвращаем скорость всем врагам перед уничтожением
        List<GameObject> enemiesCopy = new List<GameObject>(enemiesInZone);
        foreach (var enemy in enemiesCopy)
        {
            if (enemy != null)
            {
                RemoveSlowFromEnemy(enemy);
            }
        }
        
        CancelInvoke();
    }
}