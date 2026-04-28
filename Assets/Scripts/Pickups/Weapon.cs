using System.Linq;
using UnityEngine;

public class Weapon
{
    public WeaponData baseData;
    public WeaponData augmentedData;
    
    public void InitializeWeapon(WeaponData data)
    {
        baseData = data;
        augmentedData = baseData.GetCopy();
    }
    
    public void AugmentWeaponStats(AugmentData aug, bool apply)
    {
        if (aug == null)//|| aug.isActive)
        {
            augmentedData = baseData;
        }
        else if (baseData.isMelee && !(new[] { WeaponType.Any, WeaponType.Melee,
                                                 WeaponType.Dagger, WeaponType.Sword
                                             }.Contains(aug.weaponType)))
        {
            return;
        }
        else if (!baseData.isMelee && !(new[] { WeaponType.Any, WeaponType.Ranged,
                                                 WeaponType.Revolver, WeaponType.Shotgun,
                                                 WeaponType.LeverRifle, WeaponType.AssaultRifle
                                             }.Contains(aug.weaponType)))
        {
            return;
        }
        else
        {
            float rarityMultiplier = aug.possibleRarities[aug.rarity].valueMultiplier;
            foreach(StatModifier statModifier in aug.statModifiers) {
                StatType curStat = statModifier.statType;
                
                switch (curStat)
                {
                    case StatType.AmmoCapacity:
                        augmentedData.ammoCapacity = (apply) ? (int)ApplyChange(statModifier, rarityMultiplier, augmentedData.ammoCapacity) :
                                                                (int)RemoveChange(statModifier, rarityMultiplier, augmentedData.ammoCapacity);
                        break;
                    case StatType.AmmoReserve:
                        augmentedData.ammoReserves = (apply) ? (int)ApplyChange(statModifier, rarityMultiplier, augmentedData.ammoReserves) :
                                                                (int)RemoveChange(statModifier, rarityMultiplier, augmentedData.ammoReserves);
                        break;
                    case StatType.AmmoUsage:
                        augmentedData.ammoUsage = (apply) ? ApplyChange(statModifier, rarityMultiplier, augmentedData.ammoUsage) :
                                                            RemoveChange(statModifier, rarityMultiplier, augmentedData.ammoUsage);
                        break;
                    // case StatType.CritChance:
                    //     augmentedData. = ApplyChange(statModifier, rarityMultiplier, augmentedData.);
                    //     break;
                    // case StatType.CritDamage:
                    //     augmentedData. = ApplyChange(statModifier, rarityMultiplier, augmentedData.);
                    //     break;
                    case StatType.Damage:
                        augmentedData.damage = (apply) ? (int)ApplyChange(statModifier, rarityMultiplier, augmentedData.damage) :
                                                        (int)RemoveChange(statModifier, rarityMultiplier, augmentedData.damage);
                        break;
                    // case StatType.StoppingPower:
                    //     augmentedData. = ApplyChange(statModifier, augmentedData.);
                    //     break;
                    case StatType.NumBullets:
                        augmentedData.numBullets = (apply) ? (int)ApplyChange(statModifier, rarityMultiplier, augmentedData.numBullets) :
                                                            (int)RemoveChange(statModifier, rarityMultiplier, augmentedData.numBullets);
                        break;
                    case StatType.ShotSpeed:
                        augmentedData.shotSpeed = (apply) ? ApplyChange(statModifier, rarityMultiplier, augmentedData.shotSpeed) :
                                                            RemoveChange(statModifier, rarityMultiplier, augmentedData.shotSpeed);
                        break;
                    case StatType.Range:
                        augmentedData.range = (apply) ? ApplyChange(statModifier, rarityMultiplier, augmentedData.range) :
                                                        RemoveChange(statModifier, rarityMultiplier, augmentedData.range);
                        break;
                    case StatType.AttackCooldown:
                        augmentedData.attackCooldown = (apply) ? ApplyChange(statModifier, rarityMultiplier, augmentedData.attackCooldown) :
                                                                RemoveChange(statModifier, rarityMultiplier, augmentedData.attackCooldown);
                        break;
                    case StatType.ReloadTime:
                        augmentedData.reloadTime = (apply) ? ApplyChange(statModifier, rarityMultiplier, augmentedData.reloadTime) :
                                                            RemoveChange(statModifier, rarityMultiplier, augmentedData.reloadTime);
                        break;
                    case StatType.ReloadUsage:
                        augmentedData.reloadUsage = (apply) ? ApplyChange(statModifier, rarityMultiplier, augmentedData.reloadUsage) :
                                                                RemoveChange(statModifier, rarityMultiplier, augmentedData.reloadUsage);
                        break;
                    // case StatType.ExtraDamageChance:
                    //     augmentedData. = ApplyChange(aug, augmentedData.);
                    //     break;
                    // case StatType.ExtraDamageVal:
                    //     augmentedData. = ApplyChange(aug, augmentedData.);
                    //     break;
                    default:
                        Debug.Log("Unknown stat type: " + statModifier.statType);
                        break;
                }
            }
        }
    }

    private float ApplyChange(StatModifier modifier, float rarityMultiplier, float curValue)
    {
        ChangeType curChangeType = modifier.changeType;
        float curChangeValue = rarityMultiplier * modifier.changeValue;
        
        switch (curChangeType)
        {
            case ChangeType.Flat:
                curValue += curChangeValue;
                break;
            case ChangeType.Multiplier:
                curValue *= curChangeValue;
                break;
            case ChangeType.Percentage:
                curValue *= (1 + curChangeValue/100);
                break;
        }
        
        return curValue;
    }
    
    private float RemoveChange(StatModifier modifier, float rarityMultiplier, float curValue)
    {
        ChangeType curChangeType = modifier.changeType;
        float curChangeValue = rarityMultiplier * modifier.changeValue;
        
        switch (curChangeType)
        {
            case ChangeType.Flat:
                curValue -= curChangeValue;
                break;
            case ChangeType.Multiplier:
                curValue /= curChangeValue;
                break;
            case ChangeType.Percentage:
                curValue *= (curChangeValue/(100 + curChangeValue));
                break;
        }
        
        return curValue;
    }
}
