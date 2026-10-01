using UnityEngine;

public class MeleeSwingSkill : MonoBehaviour, ISkillLogic
{
    public GameObject swingPrefab; // префаб с анимацией и компонентом MeleeSwing

    private SkillInstance skillInstance;

    public void Initialize(SkillInstance instance)
    {
        skillInstance = instance;
    }

    public void Activate(Transform target)
    {
        if (swingPrefab == null || skillInstance?.Manager == null) return;

        // Получаем только урон из SkillInstance
        int damage = skillInstance.Damage;

        Transform player = skillInstance.Manager.transform;

        // Создаём объект атаки и делаем его дочерним игрока
        GameObject swing = Instantiate(swingPrefab, player.position, Quaternion.identity);
        swing.transform.SetParent(player);

        // Передаём урон в компонент удара
        MeleeSwing melee = swing.GetComponent<MeleeSwing>();
        if (melee != null)
        {
            melee.damage = damage;
            // radius, hitTime, animDuration остаются константами из префаба
        }
    }

    public void Dispose()
    {
        Destroy(gameObject);
    }
}