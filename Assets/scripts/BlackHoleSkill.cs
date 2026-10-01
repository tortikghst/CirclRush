using UnityEngine;
using System.Collections.Generic;

public class BlackHoleSkill : MonoBehaviour, ISkillLogic
{
    [Header("Настройки")]
    public GameObject projectilePrefab;  // Префаб BlackHoleProjectile

    private SkillInstance skillInstance;

    public void Initialize(SkillInstance instance)
    {
        skillInstance = instance;
        Debug.Log($"BlackHoleSkill инициализирован для {skillInstance.data.skillName}");
    }

    public void Activate(Transform target)
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("BlackHoleSkill: не назначен префаб!");
            return;
        }

        if (skillInstance?.Manager == null)
        {
            Debug.LogError("BlackHoleSkill: SkillManager не найден!");
            return;
        }

        // Получаем актуальные параметры из SkillInstance
        int count = skillInstance.Count;
        int damage = skillInstance.Damage;
        float searchRadius = skillInstance.SearchRadius;
        float effectRadius = skillInstance.EffectRadius;

        // Находим всех врагов в радиусе
        List<Transform> enemies = FindAllEnemies(skillInstance.Manager.transform.position, searchRadius);

        if (enemies.Count == 0)
        {
            Debug.Log("BlackHoleSkill: нет врагов в радиусе");
            return;
        }

        // Создаём снаряды (каждый на свою цель)
        for (int i = 0; i < count; i++)
        {
            if (enemies.Count == 0) break;

            // Выбираем случайного врага
            int randomIndex = Random.Range(0, enemies.Count);
            Transform targetEnemy = enemies[randomIndex];
            enemies.RemoveAt(randomIndex); // убираем, чтобы не выбрать дважды

            // Небольшой разброс по позиции
            Vector3 offset = Random.insideUnitSphere * 0.3f;
            offset.z = 0;

            GameObject proj = Instantiate(
                projectilePrefab,
                skillInstance.Manager.transform.position + offset,
                Quaternion.identity
            );
            AudioManager.Instance?.PlayBlackHole();
            BlackHoleProjectile blackHole = proj.GetComponent<BlackHoleProjectile>();
            if (blackHole != null)
            {
                blackHole.Initialize(targetEnemy);
                blackHole.damagePerTick = damage;
                blackHole.duration = effectRadius; // длительность = радиус эффекта
                blackHole.slowFactor = 0.5f; // можно добавить в прокачку позже
            }
        }
    }

    private List<Transform> FindAllEnemies(Vector3 center, float radius)
    {
        List<Transform> result = new List<Transform>();

        Collider2D[] colliders = Physics2D.OverlapCircleAll(center, radius, SkillManager.Instance.enemyLayer);
        foreach (Collider2D col in colliders)
        {
            result.Add(col.transform);
        }

        return result;
    }

    public void Dispose()
    {
        Destroy(gameObject);
    }
}