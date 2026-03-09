using System;
using System.Collections;
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
    [SerializeField] public MeleeWeaponData currentWeapon;
    [SerializeField] private GameObject currentWeaponInstance;

    private playerStats stats;
    private float lastAttackTime;
    
    private Animator animator;

    [SerializeField] private Transform weaponContainer;

    private void Awake()
    {
        stats = GetComponent<playerStats>();
        animator = GetComponent<Animator>();
    }

    public void EquipWeapon(MeleeWeaponData newWeapon)
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
            StartCoroutine(Wait(0.5f));
        }
    }

    void TryAttack()
    {
        if (currentWeapon == null)
        {
            return;
        }

        if (Time.time < lastAttackTime + currentWeapon.attackCooldown) return;

        //Debug.Log("Attacked");
        lastAttackTime = Time.time; 
        if(currentWeapon.isMelee && 
           currentWeaponInstance.TryGetComponent<Melee>(out var meleeWeapon))
        {
            Debug.Log("swinging");
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (mousePos - (Vector2)transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            if(45f <= angle && angle <= 135f) // up
            {
                animator.SetFloat("MouseDir", 0f);
                Debug.Log("mouse dir: up");
            }
            else if(-45f <= angle && angle < 45f) //right
            {
                animator.SetFloat("MouseDir", 1f);
                Debug.Log("mouse dir: right");
            }
            else if(-135f <= angle && angle <= -45f) // down
            {
                animator.SetFloat("MouseDir", 2f);
                Debug.Log("mouse dir: down");
            }
            else //left
            {
                animator.SetFloat("MouseDir", 3f);
                Debug.Log("mouse dir: left");
            }

            animator.SetBool("isSwinging", true);
            int damage = calculateDamage();
            meleeWeapon.Initialize(damage);
        }
    }

    private int calculateDamage()
    {
        float damage = currentWeapon.baseDamage;
        damage *= stats.damageMultiplier;

        return Mathf.RoundToInt(damage);
    }

    IEnumerator Wait(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        animator.SetBool("isSwinging", false);
    }
}
