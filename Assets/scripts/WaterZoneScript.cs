using UnityEngine;

public class WaterZoneSkill : MonoBehaviour, ISkillLogic
{
    public GameObject zonePrefab;

    [Header("Константы")]
    public float tickInterval = 1f;
    public float slowFactor = 0.5f;

    private SkillInstance skillInstance;

    public void Initialize(SkillInstance instance)
    {
        skillInstance = instance;
        Debug.Log($"WaterZoneSkill: Инициализирован для {instance?.data?.skillName}");
    }

    public void Activate(Transform target)
    {
        Debug.Log("WaterZoneSkill: Activate() вызван!");
        
        if (zonePrefab == null)
        {
            Debug.LogError("WaterZoneSkill: zonePrefab не назначен!");
            return;
        }
        
        if (skillInstance?.Manager == null)
        {
            Debug.LogError("WaterZoneSkill: SkillManager не найден!");
            return;
        }

        // Получаем параметры из SkillInstance
        int damage = skillInstance.Damage;
        float duration = skillInstance.EffectRadius;
        
        Debug.Log($"WaterZoneSkill: damage={damage}, duration={duration}");

        Transform player = skillInstance.Manager.transform;
        Debug.Log($"WaterZoneSkill: player position = {player.position}");
        
        GameObject zone = Instantiate(zonePrefab, player.position, Quaternion.identity);
        Debug.Log($"WaterZoneSkill: объект создан: {zone.name}");
        
        WaterZone waterZone = zone.GetComponent<WaterZone>();
        
        if (waterZone != null)
        {
            waterZone.damagePerTick = damage;
            waterZone.duration = duration;
            waterZone.tickInterval = tickInterval;
            waterZone.slowFactor = slowFactor;
            Debug.Log("WaterZoneSkill: параметры переданы в WaterZone");
        }
        else
        {
            Debug.LogError("WaterZoneSkill: компонент WaterZone не найден на префабе!");
        }
    }

    public void Dispose()
    {
        Debug.Log("WaterZoneSkill: Dispose()");
        Destroy(gameObject);
    }
}