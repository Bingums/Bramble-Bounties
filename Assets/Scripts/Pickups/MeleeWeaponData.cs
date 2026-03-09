using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Weapons/weapon")]
public class MeleeWeaponData : ScriptableObject
{
    public string weaponName;
    public int baseDamage;
    public float attackCooldown;
    public float range;
    public bool isMelee;

    public GameObject weaponPrefab;
}
