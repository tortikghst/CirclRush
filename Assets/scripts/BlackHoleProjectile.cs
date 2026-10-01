using UnityEngine;
using System.Collections.Generic;

public class BlackHoleProjectile : MonoBehaviour
{
    [Header("Движение")]
    public float speed = 15f;
    public float arrivalThreshold = 0.5f;
    
    [Header("Зона действия")]
    public float duration = 3f;
    
    [Header("Урон")]
    public int damagePerTick = 5;
    public float tickInterval = 1f;
    
    [Header("Замедление")]
    public float slowFactor = 0.5f;
    public bool applySlow = true;
    
    [Header("Цели")]
    public LayerMask enemyLayer;
    
    private Transform target;
    private bool isActive = false;
    private bool isDying = false;
    private List<GameObject> enemiesInRange = new List<GameObject>();
    private Collider2D triggerCollider;
    private List<GameObject> enemiesToRemove = new List<GameObject>();
    private Dictionary<GameObject, float> originalSpeeds = new Dictionary<GameObject, float>();
    
    void Start()
    {
        triggerCollider = GetComponent<Collider2D>();
        if (triggerCollider != null)
            triggerCollider.enabled = false;
        
        Invoke(nameof(TimeOut), 5f);
    }
    
    void TimeOut()
    {
        if (!isActive && !isDying)
        {
            Die();
        }
    }
    
    public void Initialize(Transform targetPosition)
    {
        target = targetPosition;
    }
    
    void Update()
    {
        if (isDying) return;
        
        if (!isActive)
        {
            if (target == null)
            {
                Die();
                return;
            }
            
            Vector2 direction = (target.position - transform.position).normalized;
            transform.position += (Vector3)direction * speed * Time.deltaTime;
            
            if (Vector2.Distance(transform.position, target.position) < arrivalThreshold)
            {
                Arrive();
            }
        }
    }
    
    void Arrive()
    {
        isActive = true;
        speed = 0;
        
        if (triggerCollider != null)
            triggerCollider.enabled = true;
        
        InvokeRepeating(nameof(DealDamageTick), 0f, tickInterval);
        Invoke(nameof(Die), duration);
    }
    
    void DealDamageTick()
    {
        if (!isActive || isDying) return;
        
        // Очищаем мёртвых врагов
        enemiesToRemove.Clear();
        foreach (var enemy in enemiesInRange)
        {
            if (enemy == null)
                enemiesToRemove.Add(enemy);
        }
        
        foreach (var dead in enemiesToRemove)
        {
            enemiesInRange.Remove(dead);
        }
        
        // Наносим урон живым
        for (int i = enemiesInRange.Count - 1; i >= 0; i--)
        {
            GameObject enemy = enemiesInRange[i];
            
            Health health = enemy.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damagePerTick, gameObject);
            }
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActive || isDying) return;
        
        if (((1 << other.gameObject.layer) & enemyLayer) != 0)
        {
            GameObject enemy = other.gameObject;
            
            if (!enemiesInRange.Contains(enemy))
            {
                enemiesInRange.Add(enemy);
                
                if (applySlow)
                {
                    ApplySlowToEnemy(enemy);
                }
            }
        }
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        if (!isActive || isDying) return;
        
        GameObject enemy = other.gameObject;
        
        if (enemiesInRange.Contains(enemy))
        {
            enemiesInRange.Remove(enemy);
            
            if (applySlow)
            {
                RemoveSlowFromEnemy(enemy);
            }
        }
    }
    
    void ApplySlowToEnemy(GameObject enemy)
    {
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
    
    void Die()
    {
        if (isDying) return;
        isDying = true;
        
        if (applySlow)
        {
            // Создаём копию для безопасного перебора
            List<GameObject> enemiesCopy = new List<GameObject>(enemiesInRange);
            foreach (var enemy in enemiesCopy)
            {
                if (enemy != null)
                {
                    RemoveSlowFromEnemy(enemy);
                }
            }
        }
        
        CancelInvoke();
        Destroy(gameObject);
    }
}