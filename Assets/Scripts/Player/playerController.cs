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
    public static playerController Instance;

    private const int KnifeSlot = 0;
    private const int GunSlot = 1;
    private const int ShotgunSlot = 2;
    private const int AssaultSlot = 3;
    private const int LeverSlot = 4;
    private const int SwordSlot = 5;
    private static readonly Vector3 GunHoldOffset = new Vector3(0.19f, -0.1f, 0f);

    [SerializeField] private float BASE_SPEED = 5f;

    [Header("Dash Settings")] [SerializeField]
    private float staminaDrainRate = 5f; // per second

    [SerializeField] private float staminaRegenRate = 3f; // per second
    [SerializeField] private float dashMultiplier = 2f;
    [SerializeField] private float dashCooldownTime = 3f;

    [Header("Combat Debug")] [SerializeField]
    private WeaponData pistol;

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
    public int openEquipSlot = 0;
    private AugmentData[] augmentInventory = new AugmentData[16];
    private int openInventorySlot = 0;
    private AugmentPickup nearbyAugment;
    private HUDController hc;

    private PlayerState State => GameManager.Instance != null ? GameManager.Instance.PlayerState : null;

    public playerCombat combatScript;

    void Awake()
    {
        Instance = this;
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

        StartCoroutine(WaitForHUD());
    }

    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector2 dir = new Vector2(horizontal, vertical).normalized;
        bool isDashing = Input.GetKey(KeyCode.LeftShift);

        float speedToUse = BASE_SPEED;

        PlayerState state = State;

        if (state != null)
        {
            //Dash Logic
            if (isDashing && state.CurrentStamina > 0f && !isOnCooldown && (horizontal != 0f || vertical != 0f))
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

        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            EquipWeapon(KnifeSlot);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            EquipWeapon(GunSlot);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            EquipWeapon(ShotgunSlot);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            EquipWeapon(AssaultSlot);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            EquipWeapon(LeverSlot);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            EquipWeapon(SwordSlot);
        }

        if (Input.GetKeyDown(KeyCode.E) && nearbyAugment != null)
        {
            if (openEquipSlot < 8)
                EquipAugment(nearbyAugment.data, openEquipSlot);
            else
                PickupAugment(nearbyAugment.data);

            Destroy(nearbyAugment.gameObject);
            nearbyAugment = null;

            if (hc != null)
            {
                hc.SetAugmentPromptVisible(false);
            }
        }
        
        AimEquippedGunAtMouse();
    }

    public void TakeDamage(int damage)
    {
        Debug.Log(damage);
        if (State == null)
        {
            return;
        }

        State.TakeDamage(damage);

        if (State.IsDead())
        {
            hc.GameOver();
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

        combatScript.CancelReload();
        combatScript.CancelMeleeReset();

        if (equippedWeaponObject != null)
        {
            equippedWeaponObject.name = "UnequippedWeapon";
            equippedWeaponObject.transform.SetParent(null);
            equippedWeaponObject.SetActive(false);
            Destroy(equippedWeaponObject);
        }

        curSlot = slot;
        equippedSlot = slot;

        Transform weaponParent = weapons[slot].baseData.isMelee ? transform : displayedWeapon.transform;
        equippedWeaponObject = Instantiate(weapons[slot].baseData.weaponPrefab, weaponParent);
        equippedWeaponObject.name = weapons[slot].baseData.weaponPrefab.name;
        if (weapons[slot].baseData.isMelee)
        {
            equippedWeaponObject.name = "Knife";
        }

        equippedWeaponObject.transform.localPosition = Vector3.zero;
        equippedWeaponObject.transform.localRotation = Quaternion.Euler(weapons[slot].baseData.rotation);
        MatchWeaponSorting(equippedWeaponObject);
        
        SetEquippedWeaponVisible(!weapons[slot].baseData.isMelee);

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
    
    private void AimEquippedGunAtMouse()
    {
        if (weapons[curSlot].baseData.isMelee || displayedWeapon == null || equippedWeaponObject == null)
            return;

        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 aimDirection = mouseWorld - transform.position;

        bool aimingRight = aimDirection.x >= 0f;

        displayedWeapon.transform.localPosition = aimingRight
            ? GunHoldOffset
            : new Vector3(-GunHoldOffset.x, GunHoldOffset.y, GunHoldOffset.z);

        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;

        equippedWeaponObject.transform.rotation = Quaternion.Euler(0f, 0f, angle);

        SpriteRenderer weaponRenderer = equippedWeaponObject.GetComponent<SpriteRenderer>();
        SpriteRenderer playerRenderer = GetComponent<SpriteRenderer>();

        if (weaponRenderer != null)
        {
            weaponRenderer.flipY = !aimingRight;
        }

        if (playerRenderer != null && weaponRenderer != null)
        {
            weaponRenderer.sortingOrder = aimingRight
                ? playerRenderer.sortingOrder + 1
                : playerRenderer.sortingOrder - 1;
        }
    }


    private void UpgradeWeapon(int slot, WeaponData toUpgrade)
    {
        weapons[slot].baseData = toUpgrade;
        equippedSlot = -1;
        EquipWeapon(slot);
    }

    private void EquipAugment(AugmentData aug, int slot)
    {
        equippedAugments[slot] = aug;
        openEquipSlot++;
        hc.EquipAugment(aug);
        RecalculateStats();
    }

    public void RemoveEquippedAugment(int slot)
    {
        AugmentData augment = equippedAugments[slot];
        equippedAugments[slot] = null;
        if (slot != openEquipSlot - 1)
        {
            for (int i = slot; i < openEquipSlot; i++)
            {
                if (equippedAugments[i + 1] == null)
                    continue;
                equippedAugments[i] = equippedAugments[i + 1];
            }
        }

        openEquipSlot--;

        if (openInventorySlot < 16)
        {
            augmentInventory[openInventorySlot] = augment;
            openInventorySlot++;
            hc.AddInventoryAugment(augment);
        }
        else
            Instantiate(augment.pickupPrefab, transform.position, Quaternion.identity);

        hc.RefreshEquippedSlots(equippedAugments, openEquipSlot);
        RecalculateStats();
    }

    private void PickupAugment(AugmentData aug)
    {
        if (openInventorySlot == 16)
            StartCoroutine(FullInventory());
        else
        {
            augmentInventory[openInventorySlot] = aug;
            openInventorySlot++;
            hc.AddInventoryAugment(aug);
        }
    }

    public void RemoveAugment(int slot)
    {
        AugmentData augment = augmentInventory[slot];

        augmentInventory[slot] = null;
        if (slot != openInventorySlot - 1)
        {
            for (int i = slot; i < openInventorySlot; i++)
            {
                if (augmentInventory[i + 1] == null)
                    continue;
                augmentInventory[i] = augmentInventory[i + 1];
            }
        }

        openInventorySlot--;
        hc.RefreshInventorySlots(augmentInventory, openInventorySlot);

        if (openEquipSlot < 8)
            EquipAugment(augment, openEquipSlot);
        else
            Instantiate(augment.pickupPrefab, transform.position, Quaternion.identity);
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

                foreach (StatModifier modifier in augment.statModifiers)
                    if (modifier.changeType == ChangeType.Flat)
                        weapon.AugmentWeaponStats(augment);
            }

            foreach (AugmentData augment in equippedAugments)
            {
                if (augment == null)
                    continue;

                foreach (StatModifier modifier in augment.statModifiers)
                    if (modifier.changeType != ChangeType.Flat)
                        weapon.AugmentWeaponStats(augment);
            }
        }
    }

    private void RefillAmmo()
    {
        foreach (Weapon weapon in weapons)
        {
            int maxAmmo = weapon.augmentedData.maxAmmoReserves;
            int refillAmount = Mathf.FloorToInt(0.1f * maxAmmo);
            weapon.augmentedData.ammoReserves += refillAmount;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out AugmentPickup augment))
        {
            nearbyAugment = augment;

            if (hc != null)
            {
                hc.SetAugmentPromptVisible(true);
            }
        }
        else if (collision.CompareTag("Ammo"))
        {
            RefillAmmo();
            Destroy(collision.gameObject);
        }
        else if (collision.CompareTag("Health"))
        {
            State.Heal(State.MaxHealth * 0.15f);
            Destroy(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out AugmentPickup augment))
        {
            if (augment == nearbyAugment)
            {
                nearbyAugment = null;
                
                if (hc != null)
                {
                    hc.SetAugmentPromptVisible(false);
                }
            }
        }
    }

    IEnumerator DashCooldown()
    {
        isOnCooldown = true;

        yield return new WaitForSeconds(dashCooldownTime);

        isOnCooldown = false;
    }
    
    public void SetEquippedWeaponVisible(bool visible)
    {
        if (equippedWeaponObject == null)
            return;

        SpriteRenderer[] renderers = equippedWeaponObject.GetComponentsInChildren<SpriteRenderer>(true);
        foreach (SpriteRenderer renderer in renderers)
        {
            renderer.enabled = visible;
        }

    }

    IEnumerator WaitForHUD()
    {
        while (hc == null)
        {
            hc = GameObject.Find("HUD Container").GetComponentInChildren<HUDController>();
            yield return null;
        }
    }

    IEnumerator FullInventory()
    {
        hc.fullInventoryText.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.8f);
        hc.fullInventoryText.gameObject.SetActive(false);
    }

}
