using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewSkill", menuName = "Skills/SkillData")]
public class SkillData : ScriptableObject
{
    public string skillName;
    
    [Header("Иконки")]
    public Sprite cardIcon;
    public Sprite slotIcon;
    
    [TextArea] public string description;
    
    [Header("Базовые параметры")]
    public float baseCooldown = 3f;
    public int baseDamage = 5;
    public int baseCount = 1;
    public float baseSearchRadius = 5f;
    public float baseEffectRadius = 3f;
    
    [Header("Настройки навыка")]
    public bool requiresTarget = true;
    public SkillType skillType = SkillType.Active;
    
    [Header("Префаб логики")]
    public GameObject skillPrefab;
    
    [Header("Улучшения (макс. 10 уровней)")]
    public SkillUpgrade[] upgrades = new SkillUpgrade[9];
}

[System.Serializable]
public class SkillUpgrade
{   
    [Header("Параметры улучшения")]
    public int damageAdd = 0;           // добавка к урону
    public float cooldownReduce = 0f;    // уменьшение перезарядки (например, -0.5)
    public int countAdd = 0;             // добавка количества снарядов
    public float searchRadiusAdd = 0f;   // добавка к радиусу поиска
    public float effectRadiusAdd = 0f;    // добавка к радиусу эффекта
    
    [Header("Информация")]
    public string upgradeDescription;    // описание для карточки улучшения
}

public enum SkillType
{
    Active,
    Passive,
    OnHit,
    OnKill
}