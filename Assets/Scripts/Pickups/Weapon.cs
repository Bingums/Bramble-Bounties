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
        if (aug == null)
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
        else if (baseData.isMelee && baseData.weaponName != aug.weaponType && 
                  !(new[] { WeaponType.Any, WeaponType.Melee}.Contains(aug.weaponType)))
        {
            return;
        } 
        else if (!baseData.isMelee && baseData.weaponName != aug.weaponType && 
                 !(new[] { WeaponType.Any, WeaponType.Ranged}.Contains(aug.weaponType)))
        {
            return;
        } else {
            foreach (StatModifier modifier in aug.statModifiers)
            {
                switch (modifier.statType)
                {
                    case StatType.AmmoCapacity:
                        augmentedData.ammoCapacity = (int)ApplyChange(modifier, augmentedData.ammoCapacity);
                        break;
                    case StatType.MaxAmmoReserve:
                        augmentedData.maxAmmoReserves = (int)ApplyChange(modifier, augmentedData.maxAmmoReserves);
                        break;
                    case StatType.AmmoUsage:
                        augmentedData.ammoUsage = ApplyChange(modifier, augmentedData.ammoUsage);
                        break;
                    case StatType.Damage:
                        augmentedData.damage = (int)ApplyChange(modifier, augmentedData.damage);
                        break;
                    case StatType.NumBullets:
                        augmentedData.numBullets = (int)ApplyChange(modifier, augmentedData.numBullets);
                        break;
                    case StatType.ShotSpeed:
                        augmentedData.shotSpeed = ApplyChange(modifier, augmentedData.shotSpeed);
                        break;
                    case StatType.Range:
                        augmentedData.range = ApplyChange(modifier, augmentedData.range);
                        break;
                    case StatType.AttackCooldown:
                        augmentedData.attackCooldown = ApplyChange(modifier, augmentedData.attackCooldown);
                        break;
                    case StatType.ReloadTime:
                        augmentedData.reloadTime = ApplyChange(modifier, augmentedData.reloadTime);
                        break;
                    case StatType.ReloadUsage:
                        augmentedData.reloadUsage = ApplyChange(modifier, augmentedData.reloadUsage);
                        break;
                    default:
                        Debug.Log("Unknown stat type: " + modifier.statType);
                        break;
                }
            }
        }
    }

    private float ApplyChange(StatModifier modifier, float curValue)
    {
        switch (modifier.changeType)
        {
            case ChangeType.Flat:
                curValue += modifier.changeValue;
                break;
            case ChangeType.Multiplier:
                curValue *= modifier.changeValue;
                break;
            case ChangeType.Percentage:
                curValue *= (1 + modifier.changeValue);
                break;
        }

        return curValue;
    }
        
    public bool Reload()
    {
        if(augmentedData.ammoReserves >= augmentedData.ammoCapacity)
            augmentedData.currentAmmo = augmentedData.ammoCapacity;
        else 
            augmentedData.currentAmmo = augmentedData.ammoReserves;
        float reloadRoll = Random.Range(0f, 1f);
        if (reloadRoll <= augmentedData.reloadUsage)
            return true;
        return false;
    }
}
