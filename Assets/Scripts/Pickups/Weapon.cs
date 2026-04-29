using System.Linq;
using UnityEngine;

public class Weapon
{
    public WeaponData baseData;
    public WeaponData augmentedData;
    
    public void InitializeWeapon(WeaponData data)
    {
        baseData = data;
        augmentedData = data.GetCopy();
    }
    
    public void AugmentWeaponStats(AugmentData aug)
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
            foreach(StatModifier modifier in aug.statModifiers) {
                StatType curStat = modifier.statType;
                
                switch (curStat)
                {
                    case StatType.AmmoCapacity:
                        augmentedData.ammoCapacity =
                            (int)ApplyChange(modifier, rarityMultiplier, augmentedData.ammoCapacity);
                        break;
                    case StatType.AmmoReserve:
                        augmentedData.ammoReserves = (int)ApplyChange(modifier, rarityMultiplier, augmentedData.ammoReserves);
                        break;
                    case StatType.AmmoUsage:
                        augmentedData.ammoUsage = ApplyChange(modifier, rarityMultiplier, augmentedData.ammoUsage);
                        break;
                    // case StatType.CritChance:
                    //     augmentedData. = ApplyChange(statModifier, rarityMultiplier, augmentedData.);
                    //     break;
                    // case StatType.CritDamage:
                    //     augmentedData. = ApplyChange(statModifier, rarityMultiplier, augmentedData.);
                    //     break;
                    case StatType.Damage:
                        augmentedData.damage = (int)ApplyChange(modifier, rarityMultiplier, augmentedData.damage);
                        break;
                    // case StatType.StoppingPower:
                    //     augmentedData. = ApplyChange(statModifier, augmentedData.);
                    //     break;
                    case StatType.NumBullets:
                        augmentedData.numBullets = (int)ApplyChange(modifier, rarityMultiplier, augmentedData.numBullets);
                        break;
                    case StatType.ShotSpeed:
                        augmentedData.shotSpeed = ApplyChange(modifier, rarityMultiplier, augmentedData.shotSpeed);
                        break;
                    case StatType.Range:
                        augmentedData.range = ApplyChange(modifier, rarityMultiplier, augmentedData.range);
                        break;
                    case StatType.AttackCooldown:
                        augmentedData.attackCooldown = ApplyChange(modifier, rarityMultiplier, augmentedData.attackCooldown);
                        break;
                    case StatType.ReloadTime:
                        augmentedData.reloadTime = ApplyChange(modifier, rarityMultiplier, augmentedData.reloadTime);
                        break;
                    case StatType.ReloadUsage:
                        augmentedData.reloadUsage = ApplyChange(modifier, rarityMultiplier, augmentedData.reloadUsage);
                        break;
                    // case StatType.ExtraDamageChance:
                    //     augmentedData. = ApplyChange(aug, augmentedData.);
                    //     break;
                    // case StatType.ExtraDamageVal:
                    //     augmentedData. = ApplyChange(aug, augmentedData.);
                    //     break;
                    default:
                        Debug.Log("Unknown stat type: " + modifier.statType);
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
                curValue *= (1 + curChangeValue);
                break;
        }
        
        return curValue;
    }
    
    public bool Reload()
    {
        augmentedData.currentAmmo = augmentedData.ammoCapacity;
        float reloadRoll = Random.Range(0f, 1f);
        if (reloadRoll <= augmentedData.reloadUsage)
            return true;
        return false;
    }
}
