using UnityEngine;
using System.Collections.Generic;

public class OrbSkill : MonoBehaviour, ISkillLogic
{
    [Header("Настройки снаряда")]
    public GameObject projectilePrefab;  // Префаб снаряда (OrbScript)
    
    private SkillInstance skillInstance;
    
    public void Initialize(SkillInstance instance)
    {
        skillInstance = instance;
    }
    
    public void Activate(Transform target)
    {
        if (projectilePrefab == null || skillInstance?.Manager == null) return;
        
        // Получаем параметры из SkillInstance
        int count = skillInstance.Count;
        int damage = skillInstance.Damage;
        float searchRadius = skillInstance.SearchRadius;
        
        // Находим всех врагов в радиусе
        List<Transform> enemies = FindAllEnemies(skillInstance.Manager.transform.position, searchRadius);
        
        if (enemies.Count == 0)
        {
            Debug.Log("OrbSkill: нет врагов в радиусе");
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
            AudioManager.Instance?.PlayOrbShoot();
            OrbScript orb = proj.GetComponent<OrbScript>();
            if (orb != null)
            {
                orb.Initialize(targetEnemy);
                orb.damage = damage;
                // speed, lifetime остаются константами из префаба
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