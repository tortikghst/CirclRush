using UnityEngine;

public class SpiralLaserSkill : MonoBehaviour, ISkillLogic
{
    public GameObject laserPrefab;
    
    [Header("Параметры")]
    public float tickInterval = 1f;           // интервал урона (константа)
    public float bornDuration = 0.5f;         // длительность анимации появления (константа)
    public float playerHalfSize = 2f;         // половина игрока (константа)
    public float extraOffset = -2f;            // коррекция позиции (константа)

    private SkillInstance skillInstance;

    public void Initialize(SkillInstance instance)
    {
        skillInstance = instance;
    }

    public void Activate(Transform target)
    {
        if (laserPrefab == null || skillInstance?.Manager == null) return;

        // Получаем параметры из SkillInstance
        int damage = skillInstance.Damage;
        float lifetime = skillInstance.SearchRadius;      // время действия
        float laserLength = skillInstance.EffectRadius;    // длина лазера

        Transform player = skillInstance.Manager.transform;

        GameObject laser = Instantiate(laserPrefab, player.position, Quaternion.identity);
        SpiralLaser laserComp = laser.GetComponent<SpiralLaser>();
        
        if (laserComp != null)
        {
            laserComp.Initialize(player);
            laserComp.damagePerTick = damage;
            laserComp.lifetime = lifetime;
            laserComp.laserHalfLength = laserLength;
            laserComp.tickInterval = tickInterval;
            laserComp.bornDuration = bornDuration;
            laserComp.playerHalfSize = playerHalfSize;
            laserComp.extraOffset = extraOffset;
        }
    }

    public void Dispose()
    {
        Destroy(gameObject);
    }
}