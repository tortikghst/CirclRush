using UnityEngine;

public class ShieldSkill : MonoBehaviour, ISkillLogic
{
    [Header("Префаб щита")]
    public GameObject shieldPrefab;

    private SkillInstance skillInstance;

    public void Initialize(SkillInstance instance)
    {
        skillInstance = instance;
    }

    public void Activate(Transform target)
    {
        if (shieldPrefab == null || skillInstance?.Manager == null) return;

        // Получаем параметры из SkillInstance
        float duration = skillInstance.Damage;           // время действия (5)
        float speedMultiplier = skillInstance.SearchRadius; // ускорение (0.5 → 0.8)

        Transform player = skillInstance.Manager.transform;

        GameObject shield = Instantiate(shieldPrefab, player.position, Quaternion.identity);
        shield.transform.SetParent(player);

        Shield shieldComponent = shield.GetComponent<Shield>();
        if (shieldComponent != null)
        {
            shieldComponent.duration = duration;
            shieldComponent.speedMultiplier = speedMultiplier;
        }
    }

    public void Dispose()
    {
        Destroy(gameObject);
    }
}