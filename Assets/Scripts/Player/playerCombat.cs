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
    private static readonly int MouseDirHash = Animator.StringToHash("MouseDir");
    private static readonly int MeleeStateHash = Animator.StringToHash("Melee");
    private static readonly int ConditionStateHash = Animator.StringToHash("ConditionState");
    private const string AttackLayerName = "Attack Layer";

    private WeaponData weaponData;
    private playerController pc;
    private playerStats stats;
    public GameObject bullet;
    private float lastAttackTime;
    private int attackLayerIndex = -1;
    private Coroutine resetMeleeRoutine;
    
    private Animator animator;

    private void Awake()
    {
        stats = GetComponent<playerStats>();
        animator = GetComponent<Animator>();
        attackLayerIndex = animator.GetLayerIndex(AttackLayerName);
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

                resetMeleeRoutine = StartCoroutine(ResetMeleeState(weaponData.attackCooldown));
            }
            
        } else
        {
            if(Input.GetMouseButtonDown(0))
            {
                Instantiate(bullet, gameObject.transform.position, Quaternion.identity);
            }
        }
    }

    private int calculateDamage()
    {
        float damage = weaponData.damage;
        damage *= stats.damageMultiplier;

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
}
