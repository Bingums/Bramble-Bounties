using System.Linq;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public WeaponData baseData;
    public WeaponData augmentedData;

    void Awake()
    {
        augmentedData = baseData;
    }

    // rename to something since it would apply and get rid of augments
    // add a boolean as parameter to use as a tertiary operator
    // if true then apply (apply change) and if not remove (remove change)
    
    // might need to change since some augments could change multiple stats
    // would prob need an array of booleans as well then
    public void ApplyAugment(AugmentData aug)
    {
        if (aug == null || aug.isActive)
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
            // put switch inside a for loop if i do use array
            switch (aug.statType)
            {
                case StatType.AmmoCapacity:
                    augmentedData.ammoCapacity = (int)ApplyChange(aug, augmentedData.ammoCapacity);
                    break;
                case StatType.AmmoReserve:
                    augmentedData.ammoReserves = (int)ApplyChange(aug, augmentedData.ammoReserves);
                    break;
                case StatType.AmmoUsage:
                    augmentedData.ammoUsage = ApplyChange(aug, augmentedData.ammoUsage);
                    break;
                // case StatType.CritChance:
                //     augmentedData. = ApplyChange(aug, augmentedData.);
                //     break;
                // case StatType.CritDamage:
                //     augmentedData. = ApplyChange(aug, augmentedData.);
                //     break;
                case StatType.Damage:
                    augmentedData.damage = (int)ApplyChange(aug, augmentedData.damage);
                    break;
                // case StatType.StoppingPower:
                //     augmentedData. = ApplyChange(aug, augmentedData.);
                //     break;
                case StatType.NumBullets:
                    augmentedData.numBullets = (int)ApplyChange(aug, augmentedData.numBullets);
                    break;
                case StatType.ShotSpeed:
                    augmentedData.shotSpeed = ApplyChange(aug, augmentedData.shotSpeed);
                    break;
                case StatType.Range:
                    augmentedData.range = ApplyChange(aug, augmentedData.range);
                    break;
                case StatType.AttackCooldown:
                    augmentedData.attackCooldown = ApplyChange(aug, augmentedData.attackCooldown);
                    break;
                case StatType.ReloadTime:
                    augmentedData.reloadTime = ApplyChange(aug, augmentedData.reloadTime);
                    break;
                case StatType.ReloadUsage:
                    augmentedData.reloadUsage = ApplyChange(aug, augmentedData.reloadUsage);
                    break;
                // case StatType.ExtraDamageChance:
                //     augmentedData. = ApplyChange(aug, augmentedData.);
                //     break;
                // case StatType.ExtraDamageVal:
                //     augmentedData. = ApplyChange(aug, augmentedData.);
                //     break;
            }
        }
    }

    private float ApplyChange(AugmentData aug, float curValue)
    {
        switch (aug.changeType)
        {
            case ChangeType.Flat:
                curValue += aug.changeValue;
                break;
            case ChangeType.Multiplier:
                curValue *= aug.changeValue;
                break;
            case ChangeType.Percentage:
                curValue *= (1 + aug.changeValue/100);
                break;
        }
        
        return curValue;
    }
    
    private float RemoveChange(AugmentData aug, float curValue)
    {
        switch (aug.changeType)
        {
            case ChangeType.Flat:
                curValue -= aug.changeValue;
                break;
            case ChangeType.Multiplier:
                curValue /= aug.changeValue;
                break;
            case ChangeType.Percentage:
                curValue *= (aug.changeValue/(100 + aug.changeValue));
                break;
        }
        
        return curValue;
    }
}
