using System;
using System.Collections;
using Combat;
using UnityEngine;

public class playerCombat : MonoBehaviour
{
    private static readonly int MouseDirHash = Animator.StringToHash("MouseDir");
    private static readonly int MeleeStateHash = Animator.StringToHash("Melee");
    private static readonly int ConditionStateHash = Animator.StringToHash("ConditionState");
    private const string AttackLayerName = "Attack Layer";

    private WeaponData weapon;
    private playerController pc;
    private playerStats stats;
    public GameObject bullet;
    private float lastAttackTime;
    private int attackLayerIndex = -1;
    private Coroutine resetMeleeRoutine;
    
    private Animator animator;

    void Start()
    {
        stats = GetComponent<playerStats>();
        animator = GetComponent<Animator>();
        attackLayerIndex = animator.GetLayerIndex(AttackLayerName);
        pc = GetComponent<playerController>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            TryAttack();
    }

    void TryAttack()
    {
        weapon = pc.weapons[pc.curSlot].augmentedData;
        if (weapon == null)
        {
            return;
        }

        if (Time.time < lastAttackTime + weapon.attackCooldown) return;

        lastAttackTime = Time.time; 
        if(weapon.isMelee)
        {
            Debug.Log("swinging");
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (mousePos - (Vector2)transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            if(45f <= angle && angle <= 135f) // up
            {
                animator.SetFloat(MouseDirHash, 0f);
            }
            else if(-45f <= angle && angle < 45f) //right
            {
                animator.SetFloat(MouseDirHash, 1f);
            }
            else if(-135f <= angle && angle <= -45f) // down
            {
                animator.SetFloat(MouseDirHash, 2f);
            }
            else //left
            {
                animator.SetFloat(MouseDirHash, 3f);
            }

            if (attackLayerIndex >= 0)
            {
                animator.Play(MeleeStateHash, attackLayerIndex, 0f);
                if (resetMeleeRoutine != null)
                {
                    StopCoroutine(resetMeleeRoutine);
                }
                resetMeleeRoutine = StartCoroutine(ResetMeleeState(weapon.attackCooldown));
            }
        } 
        else
        {
            switch (weapon.weaponName)
            {
                case(WeaponType.Revolver):

                    break;
                case(WeaponType.Shotgun):

                    break;
                case(WeaponType.AssaultRifle):

                    break;
                case(WeaponType.LeverRifle):

                    break;
            }
            
            if(Input.GetMouseButtonDown(0))
            {
                GameObject newBullet = Instantiate(bullet, gameObject.transform.position, Quaternion.identity);
                PlayerBullet firedBullet = newBullet.GetComponent<PlayerBullet>();
                firedBullet.InitializeBullet(weapon);
            }
        }
    }

    private int calculateDamage()
    {
        float damage = weapon.damage;
        damage *= stats.damageMultiplier;
        
        // grab augmented data to perform calcs & rng

        return Mathf.RoundToInt(damage);
    }

    private IEnumerator ResetMeleeState(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        if (attackLayerIndex >= 0)
        {
            animator.Play(ConditionStateHash, attackLayerIndex, 0f);
        }

        resetMeleeRoutine = null;
    }

    private void UpgradeWeapon(int slot, int weaponVal)
    {
        
    }
}
