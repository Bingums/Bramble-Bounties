using System;
using Combat;
using UnityEngine;

public enum WeaponType
{
    Revolver,
    Shotgun,
    Lever,
    AssaultRifle,
    Dagger,
    Sword
}

public class playerCombat : MonoBehaviour
{
    //DEBUG LINE, REMOVE SERIALIZE FIELD
    [SerializeField] public WeaponData currentWeapon;
    [SerializeField] private GameObject currentWeaponInstance;

    private playerStats stats;
    private float lastAttackTime;

    [SerializeField] private Transform weaponContainer;

    private void Awake()
    {
        stats = GetComponent<playerStats>();
    }

    public void EquipWeapon(WeaponData newWeapon)
    {
        if(currentWeaponInstance != null)
        {
            Destroy(currentWeaponInstance);
        }
        
        currentWeapon = newWeapon;

        currentWeaponInstance = Instantiate(
            newWeapon.weaponPrefab, 
            weaponContainer);
        
        Debug.Log("Equipped: " + newWeapon.weaponName);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {

    }

    // Update is called once per frame

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryAttack();
        }
    }

    void TryAttack()
    {
        if (currentWeapon == null)
        {
            return;
        }

        if (Time.time < lastAttackTime + currentWeapon.attackCooldown) return;

        Debug.Log("Attacked");
        lastAttackTime = Time.time; 
        if(currentWeapon.isMelee && 
           currentWeaponInstance.TryGetComponent<Melee>(out var meleeWeapon))
        {
            int damage = calculateDamage();
            meleeWeapon.Initialize(damage);
            meleeWeapon.Swing();
        }

    }

    private int calculateDamage()
    {
        float damage = currentWeapon.baseDamage;
        damage *= stats.damageMultiplier;

        return Mathf.RoundToInt(damage);
    }

    private void OnDrawGizmosSelected()
    {
        if (currentWeapon != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, currentWeapon.range);
        }
    }
}
