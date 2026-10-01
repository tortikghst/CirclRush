using UnityEngine;

public class LungeSkill : MonoBehaviour, ISkillLogic
{
    [Header("Префаб выпада")]
    public GameObject lungePrefab;

    [Header("Параметры")]
    public float offsetDistance = 2f;   // расстояние от игрока (константа)
    public float hitTime = 0.5f;        // момент нанесения урона (константа)
    public float animDuration = 1.17f;  // длина анимации (константа)
    public float effectRadius = 2f;     // радиус поражения (константа)

    private SkillInstance skillInstance;

    public void Initialize(SkillInstance instance)
    {
        skillInstance = instance;
    }

    public void Activate(Transform target)
    {
        if (lungePrefab == null || skillInstance?.Manager == null) return;

        // Получаем ТОЛЬКО урон из SkillInstance (остальное константы)
        int damage = skillInstance.Damage;

        Transform player = skillInstance.Manager.transform;
        PlayerBehavour playerBehaviour = player.GetComponent<PlayerBehavour>();

        // Определяем направление выпада
        Vector2 direction = Vector2.right;
        if (playerBehaviour != null)
        {
            if (playerBehaviour.LastMoveX != 0 || playerBehaviour.LastMoveY != 0)
                direction = new Vector2(playerBehaviour.LastMoveX, playerBehaviour.LastMoveY).normalized;
        }

        // Позиция спавна
        Vector3 spawnPos = player.position + (Vector3)direction * offsetDistance;
        GameObject lunge = Instantiate(lungePrefab, spawnPos, Quaternion.identity);

        // Поворот
        float angle = 0f;
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            angle = direction.x > 0 ? -90f : 90f;
        else
            angle = direction.y > 0 ? 0f : 180f;
        lunge.transform.rotation = Quaternion.Euler(0, 0, angle);

        // Передаём параметры (только damage из прокачки)
        Lunge lungeComponent = lunge.GetComponent<Lunge>();
        if (lungeComponent != null)
        {
            lungeComponent.damage = damage;
            lungeComponent.hitTime = hitTime;
            lungeComponent.animDuration = animDuration;
        }
    }

    public void Dispose()
    {
        Destroy(gameObject);
    }
}