public enum StatType 
{ 
    AmmoCapacity, 
    MaxAmmoReserve, 
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