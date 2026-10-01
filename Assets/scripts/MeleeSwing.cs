using UnityEngine;

public class MeleeSwing : MonoBehaviour
{
    [Header("Настройки")]
    public float radius = 3f;               // радиус поражения
    public int damage = 8;                   // урон
    public float hitTime = 0.25f;            // момент нанесения урона (сек от старта)
    public float animDuration = 0.517f;      // длительность анимации

    private bool damageDealt = false;

    void Start()
    {
        // Запускаем таймеры
        Invoke(nameof(DealDamage), hitTime);
        Invoke(nameof(DestroySelf), animDuration);
    }

    void DealDamage()
    {
        if (damageDealt) return;
        damageDealt = true;
        AudioManager.Instance?.PlayMeleeSwing(); // <-- добавить
        // Позиция для поиска – позиция игрока (родителя)
        Vector2 center = transform.parent != null ? transform.parent.position : transform.position;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(center, radius, SkillManager.Instance.enemyLayer);
        foreach (Collider2D col in colliders)
        {
            Health health = col.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage, gameObject);
                Debug.Log($"Круговая атака нанесла {damage} урона {col.name}");
            }
        }
    }

    void DestroySelf()
    {
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
