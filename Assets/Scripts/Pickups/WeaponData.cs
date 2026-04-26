using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Weapons/Weapon Data")]
public class WeaponData : ScriptableObject
{
    public WeaponType weaponName;
    public int damage;
    // stopping power
    public int numBullets;
    public float attackCooldown; // rate of fire for ranged
    public bool isMelee;
    public float shotSpeed;
    public float range;
    public int ammoCapacity;
    public int ammoReserves;
    public float ammoUsage; // if a bullet is consumed
    public float reloadTime;
    public float reloadUsage; // if a reload uses ammo
    // crit chance
    // crit damage
    // extra damage chance
    // extra damage val

    public Vector3 rotation;

    public GameObject weaponPrefab;
    public GameObject weaponPickup;
    public Sprite weaponSprite;
}
