using System;
using System.Collections;
using Combat;
using UnityEngine;

public enum WeaponType
{
    Revolver,
    Shotgun,
    LeverRifle,
    AssaultRifle,
    Dagger,
    Sword
}

public class playerCombat : MonoBehaviour
{
    private WeaponData weaponData;
    private playerController pc;
    private playerStats stats;
    public GameObject bullet;
    private float lastAttackTime;
    
    private Animator animator;

    private void Awake()
    {
        stats = GetComponent<playerStats>();
        animator = GetComponent<Animator>();
        pc = GetComponent<playerController>();
        weaponData = pc.weapons[pc.curSlot];
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {

    }

    // Update is called once per frame

    void Update()
    {
        weaponData = pc.weapons[pc.curSlot];
        if (Input.GetMouseButtonDown(0))
        {
            TryAttack();
        }
    }

    void TryAttack()
    {
        if (weaponData == null)
        {
            return;
        }

        if (Time.time < lastAttackTime + weaponData.attackCooldown) return;

        lastAttackTime = Time.time; 
        if(weaponData.isMelee)
        {
            Debug.Log("swinging");
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (mousePos - (Vector2)transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            if(45f <= angle && angle <= 135f) // up
            {
                animator.SetFloat("MouseDir", 0f);
            }
            else if(-45f <= angle && angle < 45f) //right
            {
                animator.SetFloat("MouseDir", 1f);
            }
            else if(-135f <= angle && angle <= -45f) // down
            {
                animator.SetFloat("MouseDir", 2f);
            }
            else //left
            {
                animator.SetFloat("MouseDir", 3f);
            }

            animator.SetBool("isSwinging", true);
        } else
        {
            if(Input.GetMouseButtonDown(0))
            {
                Instantiate(bullet, gameObject.transform.position, Quaternion.identity);
            }
        }
        StartCoroutine(Wait(weaponData.attackCooldown));
    }

    private int calculateDamage()
    {
        float damage = weaponData.damage;
        damage *= stats.damageMultiplier;

        return Mathf.RoundToInt(damage);
    }

    IEnumerator Wait(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        animator.SetBool("isSwinging", false);
    }
}
