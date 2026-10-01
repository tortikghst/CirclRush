using UnityEngine;
using System.Collections.Generic;

public class OrbitingDiskSkill : MonoBehaviour, ISkillLogic
{
    [Header("Префаб диска")]
    public GameObject diskPrefab;

    [Header("Параметры")]
    public float orbitRadius = 4f;        // радиус орбиты (константа)
    public float rotationSpeed = 180f;     // скорость вращения (константа)
    public float lifeTime = 5f;            // время жизни (константа)

    private SkillInstance skillInstance;
    private List<GameObject> activeDisks = new List<GameObject>();
    private bool isActive = false;

    public void Initialize(SkillInstance instance)
    {
        skillInstance = instance;
    }

    public void Activate(Transform target)
    {
        // Очищаем список от уничтоженных дисков
        activeDisks.RemoveAll(d => d == null);

        // Сбрасываем флаг, если дисков нет
        if (isActive && activeDisks.Count == 0)
        {
            isActive = false;
        }

        if (isActive) return;

        if (diskPrefab == null || skillInstance?.Manager == null) return;

        // Получаем параметры из SkillInstance
        int count = skillInstance.Count;      // количество дисков
        int damage = skillInstance.Damage;    // урон

        Transform player = skillInstance.Manager.transform;

        float angleStep = 360f / count;
        activeDisks.Clear();

        for (int i = 0; i < count; i++)
        {
            float startAngle = i * angleStep;
            GameObject diskObj = Instantiate(diskPrefab, player.position, Quaternion.identity);
            OrbitingDisk disk = diskObj.GetComponent<OrbitingDisk>();
            if (disk != null)
            {
                disk.orbitRadius = orbitRadius;
                disk.rotationSpeed = rotationSpeed;
                disk.lifeTime = lifeTime;
                disk.damage = damage;
                disk.Initialize(player, startAngle);

                DiskTracker tracker = diskObj.AddComponent<DiskTracker>();
                tracker.skill = this;

                activeDisks.Add(diskObj);
            }
        }

        isActive = true;
    }

    public void OnDiskDestroyed()
    {
        activeDisks.RemoveAll(d => d == null);

        if (activeDisks.Count == 0)
        {
            isActive = false;
        }
    }

    public void Dispose()
    {
        foreach (var disk in activeDisks)
        {
            if (disk != null) Destroy(disk);
        }
        activeDisks.Clear();
        isActive = false;
        Destroy(gameObject);
    }
}

public class DiskTracker : MonoBehaviour
{
    public OrbitingDiskSkill skill;

    void OnDestroy()
    {
        if (skill != null)
            skill.OnDiskDestroyed();
    }
}