using UnityEngine;
using System.Collections.Generic;

public class GiftSkill : MonoBehaviour, ISkillLogic
{
    [Header("Префабы")]
    public GameObject shadowPrefab;
    public GameObject giftPrefab;

    private SkillInstance skillInstance;

    public void Initialize(SkillInstance instance)
    {
        skillInstance = instance;
    }

    public void Activate(Transform target)
    {
        if (shadowPrefab == null || giftPrefab == null || skillInstance?.Manager == null) return;

        // Получаем параметры из SkillInstance
        int count = skillInstance.Count;
        int damage = skillInstance.Damage;
        float effectRadius = skillInstance.EffectRadius;
        float searchRadius = skillInstance.SearchRadius;

        Transform player = skillInstance.Manager.transform;

        // Создаём несколько подарков
        for (int i = 0; i < count; i++)
        {
            // Случайная точка вокруг игрока
            Vector2 randomOffset = Random.insideUnitCircle * searchRadius;
            Vector3 landPos = player.position + new Vector3(randomOffset.x, randomOffset.y, 0);

            // Тень на земле
            GameObject shadow = Instantiate(shadowPrefab, landPos, Quaternion.identity);
            Shadow shadowComp = shadow.GetComponent<Shadow>();
            if (shadowComp != null)
            {
                shadowComp.Initialize(landPos);
            }

            // Подарок высоко в небе
            GameObject gift = Instantiate(giftPrefab, landPos, Quaternion.identity);
            Gift giftComp = gift.GetComponent<Gift>();
            if (giftComp != null)
            {
                giftComp.Initialize(landPos);
                giftComp.explosionRadius = effectRadius;
                giftComp.explosionDamage = damage;
            }
        }
    }

    public void Dispose()
    {
        Destroy(gameObject);
    }
}