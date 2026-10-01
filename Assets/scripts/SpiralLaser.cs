using UnityEngine;
using System.Collections.Generic;

public class SpiralLaser : MonoBehaviour
{
    [Header("Настройки")]
    public float lifetime = 3f;
    public float tickInterval = 1f;
    public int damagePerTick = 3;
    
    [Header("Размеры")]
    public float playerHalfSize = 2f;
    public float laserHalfLength = 6f;
    public float extraOffset = -2f;

    [Header("Анимации")]
    public string bornAnimation = "lazer_born";
    public string loopAnimation = "lazer";
    public float bornDuration = 0.5f;

    private Animator animator;
    private Transform player;
    private PlayerBehavour playerBehaviour;
    private bool isActive = false;
    private List<GameObject> enemiesInRange = new List<GameObject>();
    private List<GameObject> enemiesToRemove = new List<GameObject>();

    public void Initialize(Transform playerTransform)
    {
        player = playerTransform;
        playerBehaviour = player.GetComponent<PlayerBehavour>();
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Destroy(gameObject);
            return;
        }

        animator.Play(bornAnimation);
        Invoke(nameof(StartLoop), bornDuration);
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        if (player == null || playerBehaviour == null) return;

        if (!isActive)
        {
            transform.position = player.position;
            return;
        }

        Vector2 dir = new Vector2(playerBehaviour.LastMoveX, playerBehaviour.LastMoveY).normalized;
        if (dir == Vector2.zero)
            dir = new Vector2(playerBehaviour.LastMoveX, playerBehaviour.LastMoveY).normalized;

        float angle = 0f;
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            angle = dir.x > 0 ? 0f : 180f;
        else
            angle = dir.y > 0 ? 90f : -90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        float totalOffset = playerHalfSize + laserHalfLength + extraOffset;
        Vector3 newPos = player.position;

        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            newPos.x = player.position.x + (dir.x > 0 ? totalOffset : -totalOffset);
            newPos.y = player.position.y - 0.75f;
        }
        else
        {
            newPos.y = player.position.y + (dir.y > 0 ? totalOffset : -totalOffset);
            newPos.x = player.position.x;
        }

        transform.position = newPos;
        
        // Обновляем список врагов в зоне каждый кадр
        UpdateEnemiesInRange();
    }

    void UpdateEnemiesInRange()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col == null) return;

        ContactFilter2D filter = new ContactFilter2D().NoFilter();
        filter.layerMask = SkillManager.Instance.enemyLayer;
        Collider2D[] results = new Collider2D[10];
        int count = col.Overlap(filter, results);
        
        enemiesInRange.Clear();
        for (int i = 0; i < count; i++)
        {
            enemiesInRange.Add(results[i].gameObject);
        }
    }

 void StartLoop()
{
    isActive = true;
    AudioManager.Instance?.PlaySpiralLaser(); // звук сам закончится
    animator.Play(loopAnimation);
    InvokeRepeating(nameof(DealDamage), 0f, tickInterval);
}

    void DealDamage()
    {
        if (!isActive) return;

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

        // Наносим урон
        foreach (var enemy in enemiesInRange)
        {
            if (enemy == null) continue;
            
            Health health = enemy.GetComponent<Health>();
            if (health != null)
                health.TakeDamage(damagePerTick, gameObject);
        }
    }

    void OnDestroy()
    {
        CancelInvoke();
    }
}