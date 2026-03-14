using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Weapons/weapon")]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public int damage;
    public float attackCooldown; // rate of fire for ranged
    public bool isMelee;
    public float range;
    public int ammoCapacity;
    public int ammoReserves;
    public float shotSpeed;
    public float reloadTime;

    public Vector3 rotation;

    public GameObject weaponPrefab;
    public GameObject weaponPickup;
}
