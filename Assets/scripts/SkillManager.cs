using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance { get; private set; }

    [Header("Слоты")]
    [SerializeField] private UISkillSlot[] skillSlots;
    
    [Header("Поиск врагов")]
    [SerializeField] public LayerMask enemyLayer;
    
    private List<SkillInstance> activeSkills = new List<SkillInstance>();

    // Публичные свойства для доступа из других классов
    public UISkillSlot[] SkillSlots => skillSlots;
    public List<SkillInstance> ActiveSkills => activeSkills;
    public int ActiveSkillsCount => activeSkills.Count;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        foreach (SkillInstance skill in activeSkills)
        {
            if (skill != null)
                skill.UpdateSkill();
        }
    }

    // Проверка, есть ли уже такой навык в слотах
    public bool HasSkill(SkillData skillData)
    {
        return activeSkills.Any(s => s.data == skillData);
    }

    // Получить экземпляр навыка по данным
    public SkillInstance GetSkillInstance(SkillData skillData)
    {
        return activeSkills.FirstOrDefault(s => s.data == skillData);
    }

    // Добавить новый навык или улучшить существующий
    public bool AddOrUpgradeSkill(SkillData skillData)
    {
        if (HasSkill(skillData))
        {
            // Улучшаем существующий
            SkillInstance instance = GetSkillInstance(skillData);
            if (instance != null)
            {
                return instance.Upgrade();
            }
            return false;
        }
        else
        {
            // Добавляем новый, если есть место
            if (activeSkills.Count >= skillSlots.Length)
            {
                Debug.Log("Все слоты заняты! Нельзя добавить новый навык.");
                return false;
            }

            SkillInstance instance = new SkillInstance(skillData, this);
            activeSkills.Add(instance);
            UpdateUI();
            return true;
        }
    }

    public Transform FindNearestEnemy(Vector3 position, float range)
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(position, range, enemyLayer);
        
        Transform nearest = null;
        float nearestDistance = float.MaxValue;

        foreach (Collider2D enemy in enemies)
        {
            float distance = Vector2.Distance(position, enemy.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearest = enemy.transform;
            }
        }

        return nearest;
    }
    
    public Transform GetRandomEnemyInRange(Vector3 position, float range, Transform exclude = null)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, range, enemyLayer);
        if (colliders.Length == 0) return null;

        List<Transform> candidates = new List<Transform>();
        foreach (Collider2D col in colliders)
        {
            if (exclude == null || col.transform != exclude)
                candidates.Add(col.transform);
        }

        if (candidates.Count == 0) return null;

        int randomIndex = Random.Range(0, candidates.Count);
        return candidates[randomIndex];
    }

    private void UpdateUI()
    {
        for (int i = 0; i < skillSlots.Length; i++)
        {
            if (i < activeSkills.Count)
                skillSlots[i].SetSkillInstance(activeSkills[i]);
            else
                skillSlots[i].ClearSlot();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (activeSkills == null) return;
        
        Gizmos.color = Color.yellow;
        foreach (SkillInstance skill in activeSkills)
        {
            if (skill != null && skill.data != null)
            {
                if (skill.data.requiresTarget)
                {
                    Gizmos.DrawWireSphere(transform.position, skill.SearchRadius);
                }
                
                if (skill.data.baseEffectRadius > 0)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireSphere(transform.position, skill.EffectRadius);
                }
            }
        }
    }
    public bool ReplaceAllSkills(List<SkillData> availableSkills)
{
    if (availableSkills == null || availableSkills.Count < 4) return false;
    
    // Проверяем, есть ли бонус замены
    int swapBonus = PlayerPrefs.GetInt("Bonus_SkillSwap", 0);
    if (swapBonus <= 0) return false;
    
    // Удаляем все текущие навыки
    foreach (var skill in activeSkills)
    {
        skill.Dispose();
    }
    activeSkills.Clear();
    
    // Перемешиваем доступные навыки
    List<SkillData> shuffled = new List<SkillData>(availableSkills);
    for (int i = 0; i < shuffled.Count; i++)
    {
        int rand = Random.Range(i, shuffled.Count);
        SkillData temp = shuffled[i];
        shuffled[i] = shuffled[rand];
        shuffled[rand] = temp;
    }
    
    // Добавляем 4 новых навыка
    for (int i = 0; i < 4 && i < shuffled.Count; i++)
    {
        SkillInstance instance = new SkillInstance(shuffled[i], this);
        activeSkills.Add(instance);
    }
    
    UpdateUI();
    
    // Уменьшаем счётчик замен или снимаем бонус (если одноразовый)
    PlayerPrefs.SetInt("Bonus_SkillSwap", swapBonus - 1);
    
    return true;
}
public bool ClearAllSkills()
{
    // Очищаем даже если скилов нет (всё равно возвращаем true)
    foreach (var skill in activeSkills)
    {
        skill.Dispose();
    }
    activeSkills.Clear();
    
    UpdateUI();
    return true; // всегда возвращаем true
}
}