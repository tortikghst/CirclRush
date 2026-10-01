using UnityEngine;

[CreateAssetMenu(fileName = "NewUpgrade", menuName = "Shop/Upgrade")]
public class ShopUpgrade : ScriptableObject
{
    public string upgradeName;
    public UpgradeType upgradeType;
    public int maxLevel = 3;
    public LevelData[] levels;
}

[System.Serializable]
public class LevelData
{
    public float value;
    public int price;
}

public enum UpgradeType
{
    MaxHealth, HealthRegen, Damage, MoveSpeed, CooldownReduction,
    Range, DamageResist, DamageReflect, ExpMultiplier, PickupRadius,
    SkillSwap, ProjectileCount
}