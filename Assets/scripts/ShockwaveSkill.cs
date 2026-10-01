using UnityEngine;

public class ShockwaveSkill : MonoBehaviour, ISkillLogic
{
    public GameObject shockwavePrefab;

    private SkillInstance skillInstance;

    public void Initialize(SkillInstance instance)
    {
        skillInstance = instance;
    }

    public void Activate(Transform target)
    {
        if (shockwavePrefab == null || skillInstance?.Manager == null) return;

        // Получаем только урон из SkillInstance
        int damage = skillInstance.Damage;

        Transform player = skillInstance.Manager.transform;
        
        GameObject wave = Instantiate(shockwavePrefab, player.position, Quaternion.identity);
        
        ShockWave shockWave = wave.GetComponent<ShockWave>();
        if (shockWave != null)
        {
            shockWave.damage = damage;
            // pushForce остаётся константой из префаба
        }
    }

    public void Dispose()
    {
        Destroy(gameObject);
    }
}