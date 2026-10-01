using UnityEngine;
using System.Collections.Generic;

public class UfoBeamSkill : MonoBehaviour, ISkillLogic
{
    public GameObject beamPrefab;

    [Header("Константы")]
    public float liftHeight = 5.5f;      // высота подъёма (константа)

    private SkillInstance skillInstance;

    public void Initialize(SkillInstance instance)
    {
        skillInstance = instance;
    }

    public void Activate(Transform target)
    {
        if (beamPrefab == null || skillInstance?.Manager == null) return;

        // Получаем параметры из SkillInstance
        int damage = skillInstance.Damage;
        int count = skillInstance.Count;
        float duration = skillInstance.EffectRadius;      // время действия
        float searchRadius = skillInstance.SearchRadius;  // радиус поиска

        Transform player = skillInstance.Manager.transform;
        
        // Находим всех врагов в радиусе
        List<Transform> enemies = FindAllEnemies(player.position, searchRadius);

        if (enemies.Count == 0)
        {
            Debug.Log("UfoBeam: нет врагов в радиусе");
            return;
        }

        // Создаём лучи (каждый на своего врага)
        for (int i = 0; i < count && i < enemies.Count; i++)
        {
            Transform targetEnemy = enemies[i];

            GameObject beam = Instantiate(beamPrefab, targetEnemy.position, Quaternion.identity);
            UfoBeam ufoBeam = beam.GetComponent<UfoBeam>();

            if (ufoBeam != null)
            {
                ufoBeam.Initialize(targetEnemy);
                ufoBeam.damagePerTick = damage;
                ufoBeam.duration = duration;
                ufoBeam.liftHeight = liftHeight;
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