using UnityEngine;

public class Lunge : MonoBehaviour
{
    [Header("Настройки")]
    public int damage = 8;
    public float hitTime = 0.5f;
    public float animDuration = 1.17f;

    private bool damageDealt = false;
    private bool hasPlayedLaunchSound = false;

    void Start()
    {
        // Звук при запуске выпада
        AudioManager.Instance?.PlayLungeSwing();
        hasPlayedLaunchSound = true;
        
        // Запускаем таймеры
        Invoke(nameof(DealDamage), hitTime);
        Invoke(nameof(DestroySelf), animDuration);
    }

    void DealDamage()
    {
        if (damageDealt) return;
        damageDealt = true;
        
        // Используем коллайдер префаба как зону поражения (если есть)
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            ContactFilter2D filter = new ContactFilter2D().NoFilter();
            filter.layerMask = SkillManager.Instance.enemyLayer;
            Collider2D[] results = new Collider2D[10];
            int count = col.Overlap(filter, results);
            for (int i = 0; i < count; i++)
            {
                Health health = results[i].GetComponent<Health>();
                if (health != null) health.TakeDamage(damage, gameObject);
            }
        }
        else
        {
            // Если коллайдера нет – используем круг фиксированного радиуса
            float radius = 2f;
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius, SkillManager.Instance.enemyLayer);
            foreach (var hit in hits)
            {
                Health health = hit.GetComponent<Health>();
                if (health != null) health.TakeDamage(damage);
            }
        }
    }

    void DestroySelf()
    {
        Destroy(gameObject);
    }
}