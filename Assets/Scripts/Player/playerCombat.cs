using System;
using System.Collections;
using Combat;
using UnityEngine;
using Random = UnityEngine.Random;

public class playerCombat : MonoBehaviour
{
    private static readonly int MouseDirHash = Animator.StringToHash("MouseDir");
    private static readonly int MeleeStateHash = Animator.StringToHash("Melee");
    private static readonly int ConditionStateHash = Animator.StringToHash("ConditionState");
    private const string AttackLayerName = "Attack Layer";

    public WeaponData weapon;
    private playerController pc;
    private playerStats stats;
    public GameObject bullet;
    private float lastAttackTime;
    private int attackLayerIndex = -1;
    private Coroutine resetMeleeRoutine;
    
    private Animator animator;
    
    public bool isReloading = false;
    public float reloadProgress = 0f;

    void Start()
    {
        stats = GetComponent<playerStats>();
        animator = GetComponent<Animator>();
        attackLayerIndex = animator.GetLayerIndex(AttackLayerName);
        pc = GetComponent<playerController>();
    }

    void Update()
    {
        weapon = pc.weapons[pc.curSlot].augmentedData;
        if (((Input.GetKeyDown(KeyCode.R) && weapon.currentAmmo < weapon.ammoCapacity) || 
            (Input.GetMouseButtonDown(0) && weapon.currentAmmo == 0)) && 
            !weapon.isMelee && weapon.ammoReserves > 0)
            Reload();
        else if(Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
            TryAttack();
    }

    void TryAttack()
    {
        if (weapon == null || isReloading || Time.time < lastAttackTime + weapon.attackCooldown)
        {
            return;
        }

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - (Vector2)transform.position).normalized;
        lastAttackTime = Time.time; 
        if(weapon.isMelee)
        {
            //Debug.Log("swinging");
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
            //Debug.Log("firing");
            if (weapon.weaponName == WeaponType.Shotgun)
            {
                for (int i = 0; i < weapon.numBullets; i++)
                {
                    float bulletAngle = Random.Range(-weapon.bulletSpread/2f, weapon.bulletSpread/2f);
                    float angleRadians = bulletAngle * Mathf.Deg2Rad;
                    Vector2 bulletDirection = new Vector2(Mathf.Cos(angleRadians) * direction.x - Mathf.Sin(angleRadians) * direction.y, 
                        Mathf.Sin(angleRadians) * direction.x + Mathf.Cos(angleRadians) * direction.y);
                    
                    GameObject newBullet = Instantiate(bullet, transform.position, Quaternion.identity);
                    newBullet.GetComponent<PlayerBullet>().InitializeBullet(weapon, bulletDirection);
                }
            }
            else
            {
                Vector2 perpendicular = new Vector2(-direction.y, direction.x);
                float spacing = 0.3f;
                float bulletSpread = spacing * (weapon.numBullets - 1);
                float initialOffset = -bulletSpread / 2f;

                for (int i = 0; i < weapon.numBullets; i++)
                {
                    float curOffset = initialOffset + (i * spacing);
                    Vector2 bulletPos = (Vector2)transform.position + perpendicular * curOffset;
                    GameObject newBullet = Instantiate(bullet, bulletPos, Quaternion.identity);
                    newBullet.GetComponent<PlayerBullet>().InitializeBullet(weapon, direction);
                }
            }
            
            float ammoRoll = Random.Range(0f, 1f);
            if (ammoRoll <= weapon.ammoUsage)
                weapon.currentAmmo--;
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

    public void Reload()
    {
        StartCoroutine(ReloadCoroutine());
    }

    public void CancelReload()
    {
        if (isReloading)
        {
            StopCoroutine(ReloadCoroutine());
            isReloading = false;
            reloadProgress = 0f;
        }
    }
    
    private IEnumerator ReloadCoroutine()
    {
        int usedAmmo = weapon.ammoCapacity - weapon.currentAmmo;
        isReloading = true;
        float elapsedTime = 0f;
        float reloadTime = weapon.reloadTime;

        while (elapsedTime < reloadTime)
        {
            elapsedTime += Time.deltaTime;
            reloadProgress = elapsedTime / reloadTime;
            yield return null;
        }
        
        bool reduceAmmo = pc.weapons[pc.curSlot].Reload();
        if (reduceAmmo)
            weapon.ammoReserves -= usedAmmo;
        
        isReloading = false;
        reloadProgress = 0f;
    }
}
