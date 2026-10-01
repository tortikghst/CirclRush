using UnityEngine;

[System.Serializable]
public class SkillInstance
{
    public SkillData data;
    public int currentLevel = 1;
    public float currentCooldown;
    public bool isWaitingForEnemy;
    public Transform currentTarget;
    
    private SkillManager manager;
    private ISkillLogic skillLogic;

    public SkillManager Manager => manager;

    public float Cooldown 
    { 
        get 
        {
            float total = data.baseCooldown;
            for (int i = 0; i < currentLevel - 1 && i < data.upgrades.Length; i++)
                total -= data.upgrades[i].cooldownReduce;
            
            // Бонус от магазина (уменьшение перезарядок)
            float bonus = PlayerPrefs.GetFloat("Bonus_CooldownReduction", 0f);
            total = total * (1f - bonus);
            
            return Mathf.Max(0.1f, total);
        }
    }

    public int Count 
    { 
        get 
        {
            int total = data.baseCount;
            for (int i = 0; i < currentLevel - 1 && i < data.upgrades.Length; i++)
                total += data.upgrades[i].countAdd;
            
            // Бонус от магазина (число снарядов)
            int bonus = PlayerPrefs.GetInt("Bonus_ProjectileCount", 0);
            total += bonus;
            
            return total;
        }
    }

    public float SearchRadius 
    { 
        get 
        {
            float total = data.baseSearchRadius;
            for (int i = 0; i < currentLevel - 1 && i < data.upgrades.Length; i++)
                total += data.upgrades[i].searchRadiusAdd;
            
            // Бонус от магазина (дальность)
            float bonus = PlayerPrefs.GetFloat("Bonus_Range", 0f);
            total += bonus;
            
            return total;
        }
    }

    public float EffectRadius 
    { 
        get 
        {
            float total = data.baseEffectRadius;
            for (int i = 0; i < currentLevel - 1 && i < data.upgrades.Length; i++)
                total += data.upgrades[i].effectRadiusAdd;
            return total;
        }
    }

    public int Damage 
    { 
        get 
        {
            int total = data.baseDamage;
            for (int i = 0; i < currentLevel - 1 && i < data.upgrades.Length; i++)
                total += data.upgrades[i].damageAdd;
            
            // Бонус от магазина (процентный)
            float bonus = PlayerPrefs.GetFloat("Bonus_Damage", 0f);
            total = Mathf.RoundToInt(total * (1f + bonus));
            
            return total;
        }
    }

    public SkillInstance(SkillData skillData, SkillManager skillManager)
    {
        data = skillData;
        manager = skillManager;
        currentCooldown = 0f;
        isWaitingForEnemy = false;
        currentLevel = 1;

        if (data.skillPrefab != null)
        {
            GameObject logicObject = Object.Instantiate(data.skillPrefab);
            skillLogic = logicObject.GetComponent<ISkillLogic>();
            if (skillLogic != null)
            {
                skillLogic.Initialize(this);
            }
            else
            {
                Debug.LogError($"SkillInstance: на префабе {data.skillPrefab.name} нет компонента ISkillLogic!");
            }
        }
    }

    public bool Upgrade()
    {
        if (currentLevel >= 10) return false;
        currentLevel++;
        Debug.Log($"Навык {data.skillName} улучшен до уровня {currentLevel}");
        return true;
    }

    public void UpdateSkill()
    {
        if (currentCooldown > 0)
            currentCooldown -= Time.deltaTime;

        if (data.skillType == SkillType.Active)
        {
            HandleActiveSkill();
        }
    }

    private void HandleActiveSkill()
    {
        if (currentCooldown <= 0)
        {
            if (data.requiresTarget)
            {
                Transform enemy = manager.FindNearestEnemy(manager.transform.position, SearchRadius);
                if (enemy != null)
                {
                    Activate(enemy);
                    currentCooldown = Cooldown;
                    isWaitingForEnemy = false;
                }
                else
                {
                    isWaitingForEnemy = true;
                }
            }
            else
            {
                Activate(null);
                currentCooldown = Cooldown;
                isWaitingForEnemy = false;
            }
        }
    }

    public void Activate(Transform target = null)
    {
        if (skillLogic != null)
        {
            skillLogic.Activate(target);
        }
    }

    public void Dispose()
    {
        if (skillLogic != null)
        {
            skillLogic.Dispose();
        }
    }

    public float GetCooldownPercent()
    {
        return currentCooldown / Cooldown;
    }
}