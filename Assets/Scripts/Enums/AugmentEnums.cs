public enum Rarity { Common, Rare, Epic, Legendary }
public enum StatType 
{ 
    AmmoCapacity, 
    AmmoReserve, 
    AmmoUsage,
    ReloadUsage,
    //CritChance,
    //CritDamage,
    Damage, 
    //StoppingPower,
    NumBullets,
    ShotSpeed, 
    Range,
    AttackCooldown, 
    ReloadTime
    //ExtraDamageChance
    //ExtraDamageVal
}

public enum ChangeType { Flat, Multiplier, Percentage }

public enum WeaponType
{
    Any,
    Ranged,
    Melee,
    Revolver,
    Shotgun,
    LeverRifle,
    AssaultRifle,
    Dagger,
    Sword
}