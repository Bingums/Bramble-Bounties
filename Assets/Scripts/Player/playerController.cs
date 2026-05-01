using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Combat;
using Unity.Collections;
using Unity.VisualScripting;

public class playerController : MonoBehaviour, IDamageable
{
    private const int KnifeSlot = 0;
    private const int GunSlot = 1;
    private const int ShotgunSlot = 2;
    private const int AssaultSlot = 3;
    private const int LeverSlot = 4;
    private const int SwordSlot = 5;
    private static readonly Vector3 GunHoldOffset = new Vector3(0.35f, 0.05f, 0f);

    [SerializeField] private float BASE_SPEED = 5f;

    [Header("Dash Settings")]
    [SerializeField] private float staminaDrainRate = 5f;     // per second
    [SerializeField] private float staminaRegenRate = 3f;     // per second
    [SerializeField] private float dashMultiplier = 2f;
    [SerializeField] private float dashCooldownTime = 3f;

    [Header("Combat Debug")] 
    [SerializeField] private WeaponData pistol;
    [SerializeField] private WeaponData knife;
    [SerializeField] private WeaponData sword;
    [SerializeField] private WeaponData rifle;
    [SerializeField] private WeaponData shotgun;
    [SerializeField] private WeaponData lever;
    
    public event Action<WeaponData> OnEquippedWeaponChanged;
    
    private bool isOnCooldown = false;

    private Rigidbody2D rb;
    private Animator animator;

    public Weapon[] weapons = new Weapon[6];
    public int curSlot = 0;
    public GameObject displayedWeapon;
    private GameObject equippedWeaponObject;
    private int equippedSlot = -1;
    
    private AugmentData[] equippedAugments = new AugmentData[8];
    private AugmentData[] augmentInventory = new AugmentData[16];
    private AugmentPickup nearbyAugment;
    
    private PlayerState State => GameManager.Instance != null ? GameManager.Instance.PlayerState : null;

    public playerCombat combatScript;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        combatScript = GetComponent<playerCombat>();
    }

    void Start()
    {
        for (int i = 0; i < weapons.Length; i++)
            weapons[i] = new Weapon();
        
        weapons[0].InitializeWeapon(knife);
        weapons[1].InitializeWeapon(pistol);
        weapons[2].InitializeWeapon(shotgun);
        weapons[3].InitializeWeapon(rifle);
        weapons[4].InitializeWeapon(lever);
        weapons[5].InitializeWeapon(sword);
        EquipWeapon(KnifeSlot);
    }

    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        
        Vector2 dir = new Vector2(horizontal, vertical).normalized;
        bool isDashing = Input.GetKey(KeyCode.LeftShift);

        float speedToUse = BASE_SPEED;
        
        PlayerState state = State;
        
        if(state != null)
        {
            //Dash Logic
            if (isDashing && state.CurrentStamina > 0f && !isOnCooldown)
            {
                speedToUse *= dashMultiplier;

                float staminaCost = staminaDrainRate * Time.deltaTime;
                bool spent = state.TrySpendStamina((staminaCost));

                if (!spent || state.CurrentStamina <= 0f)
                {
                    state.SetStamina(0f);
                    StartCoroutine(DashCooldown());
                }
            }
            else
            {
                if (!isOnCooldown && state.CurrentStamina < state.MaxStamina)
                {
                    state.RestoreStamina(staminaRegenRate * Time.deltaTime);
                }
            }
        }

        // Apply movement
        if (dir.magnitude < 0.1f)
        {
            rb.linearVelocity = Vector2.zero;
        }
        else
        {
            rb.linearVelocity = dir * speedToUse;
        }
        // animator uses blend tree, only needs 2 inputs
        animator.SetFloat("InputX", horizontal);
        animator.SetFloat("InputY", vertical);

        if(Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            EquipWeapon(KnifeSlot);
        } else if(Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            EquipWeapon(GunSlot);
        } else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            EquipWeapon(ShotgunSlot);
        } else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            EquipWeapon(AssaultSlot);
        } else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            EquipWeapon(LeverSlot);
        } else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            EquipWeapon(SwordSlot);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (nearbyAugment != null)
            {
                if (equippedAugments.Length < 8)
                    EquipAugment(nearbyAugment.data, equippedAugments.Length - 1);
                else
                    PickupAugment(nearbyAugment.data);
            }
        }
    }
    
    public void TakeDamage(int damage)
    {
        Debug.Log("player " + damage);
        if (State == null)
        {
            return;
        }
        
        State.TakeDamage(damage);

        if (State.IsDead())
        {
            Debug.Log("Player is dead");
        }
    }

    private void EquipWeapon(int slot)
    {
        if (slot == equippedSlot)
        {
            return;
        }

        if (slot < 0 || slot >= weapons.Length || weapons[slot] == null 
            || weapons[slot].baseData.weaponPrefab == null)
        {
            Debug.LogWarning("Cannot equip weapon slot {slot}. Check the player weapons array.");
            return;
        }

        if (displayedWeapon == null)
        {
            Debug.LogWarning("Cannot display equipped weapon because displayedWeapon is not assigned.");
            curSlot = slot;
            equippedSlot = slot;
            OnEquippedWeaponChanged?.Invoke(weapons[curSlot].augmentedData);
            return;
        }

        if (equippedWeaponObject != null)
        {
            Destroy(equippedWeaponObject);
        }

        combatScript.CancelReload();
        curSlot = slot;
        equippedSlot = slot;

        Transform weaponParent = weapons[slot].baseData.isMelee ? transform : displayedWeapon.transform;
        equippedWeaponObject = Instantiate(weapons[slot].baseData.weaponPrefab, weaponParent);
        equippedWeaponObject.name = weapons[slot].baseData.weaponPrefab.name;
        equippedWeaponObject.transform.localPosition = weapons[slot].baseData.isMelee ? Vector3.zero : GunHoldOffset;
        equippedWeaponObject.transform.localRotation = Quaternion.Euler(weapons[slot].baseData.rotation);
        MatchWeaponSorting(equippedWeaponObject);

        if (equippedWeaponObject.name.Contains("word"))
        {
            equippedWeaponObject.name = "Knife";
        }

        animator.Rebind();
        animator.Update(0f);

        OnEquippedWeaponChanged?.Invoke(weapons[curSlot].augmentedData);
    }

    private void MatchWeaponSorting(GameObject weaponObject)
    {
        SpriteRenderer playerRenderer = GetComponent<SpriteRenderer>();
        SpriteRenderer weaponRenderer = weaponObject.GetComponent<SpriteRenderer>();

        if (playerRenderer == null || weaponRenderer == null)
        {
            return;
        }

        weaponRenderer.sortingLayerID = playerRenderer.sortingLayerID;
        weaponRenderer.sortingOrder = playerRenderer.sortingOrder + 1;
    }
    
    private void UpgradeWeapon(int slot, WeaponData toUpgrade)
    {
        weapons[slot].baseData = toUpgrade;
        equippedSlot = -1;
        EquipWeapon(slot);
    }
    
    private bool IsAugmentEquipped(AugmentData aug)
    {
        foreach(AugmentData augment in equippedAugments)
        {
            if (augment.augmentName == aug.augmentName && augment.rarity == aug.rarity)
                return true;
        }

        return false;
    }
    
    private bool EquipAugment(AugmentData aug, int slot)
    {
        if (equippedAugments[slot] != null)
            if (!RemoveEquippedAugment(slot))
                return false;
        
        equippedAugments[slot] = aug;
        RecalculateStats();
        return true;
    }
    
    private bool RemoveEquippedAugment(int slot)
    {
        int numAugmentsInInventory = augmentInventory.Length;
        if (numAugmentsInInventory == 16)
        {
            // make a popup window
            return false;
        }
        else
        {
            AugmentData augment = equippedAugments[slot];
            augmentInventory[numAugmentsInInventory-1] = augment;
            equippedAugments[slot] = null;
            RecalculateStats();
            return true;
        }
    }
    
    private void PickupAugment(AugmentData aug)
    {
        int numAugmentsInInventory = augmentInventory.Length;
        if (numAugmentsInInventory == 16)
        {
            // make a popup window
        }
        else
        {
            // get rid of augment on floor
            augmentInventory[numAugmentsInInventory-1] = aug;
        }
    }

    private void DropAugment(int slot)
    {
        // place on floor
        augmentInventory[slot] = null;
        if (slot != augmentInventory.Length - 1)
        {
            for (int i = slot; i <= augmentInventory.Length - 1; i++)
            {
                if(augmentInventory[i+1] == null)
                    continue;
                augmentInventory[i] = augmentInventory[i+1];
            }
        }
    }
    
    private void RecalculateStats()
    {
        foreach (Weapon weapon in weapons)
        {
            weapon.InitializeWeapon(weapon.baseData);

            foreach (AugmentData augment in equippedAugments)
            {
                if (augment == null)
                    continue;
                
                foreach(StatModifier modifier in augment.statModifiers)
                    if(modifier.changeType == ChangeType.Flat)
                        weapon.AugmentWeaponStats(augment);
            }
            
            foreach (AugmentData augment in equippedAugments)
            {
                if (augment == null)
                    continue;
                
                foreach(StatModifier modifier in augment.statModifiers)
                    if(modifier.changeType != ChangeType.Flat)
                        weapon.AugmentWeaponStats(augment);
            }
        }
    }

    IEnumerator DashCooldown()
    {
        isOnCooldown = true;

        yield return new WaitForSeconds(dashCooldownTime);

        isOnCooldown = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    { 
        if(collision.TryGetComponent(out AugmentPickup augment))
            nearbyAugment = augment;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out AugmentPickup augment))
        {
            if(augment == nearbyAugment)
                nearbyAugment = null;
        }
    }
}
