using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [Header("Bar Fills")]
    [SerializeField] private Image healthFill;
    [SerializeField] private Image staminaFill;

    [Header("Weapon UI")]
    [SerializeField] private Image weaponIcon;
    [SerializeField] private playerController player;
    [SerializeField] private Slider reloadBar;
    [SerializeField] private TMP_Text reloadText;
    [SerializeField] private TMP_Text curAmmoText;
    [SerializeField] private TMP_Text ammoReservesText;
    
    [Header("Wave UI")]
    // cur wave and num waves
    // enemies left
    
    [Header("Alerts")]
    // incoming wave
    // room cleared
    [SerializeField] private TMP_Text noAmmoText;
    // augment inventory full
    
    private PlayerState boundState;
    private WeaponData displayedWeapon;
    private playerCombat combatScript;
    // use EnemySpawnManager.Instance.currentRoom

    private void OnEnable()
    {
        TryInitialize();
    }

    private void OnDisable()
    {
        UnbindState();
        UnbindPlayer();
        displayedWeapon = null;
    }

    private void Start()
    {
        reloadBar.gameObject.SetActive(false);
        reloadText.gameObject.SetActive(false);
        curAmmoText.gameObject.SetActive(false);
        ammoReservesText.gameObject.SetActive(false);
        noAmmoText.gameObject.SetActive(false);
    }

    private void Update()
    {
        // Retry setup until GameManager and player initialization are ready.
        if (boundState == null || player == null)
        {
            TryInitialize();
            return;
        }

        RefreshWeaponIcon();
        
        reloadBar.gameObject.SetActive(combatScript.isReloading);
        reloadText.gameObject.SetActive(combatScript.isReloading);
        if (combatScript.isReloading)
            reloadBar.value = combatScript.reloadProgress;

        WeaponData curWeapon = combatScript.weapon;
        curAmmoText.gameObject.SetActive(!curWeapon.isMelee);
        ammoReservesText.gameObject.SetActive(!curWeapon.isMelee);
        if (!curWeapon.isMelee)
        {
            noAmmoText.gameObject.SetActive(curWeapon.ammoReserves == 0 && curWeapon.currentAmmo == 0);
            curAmmoText.text = curWeapon.currentAmmo + " / " + curWeapon.ammoCapacity;
            ammoReservesText.text = curWeapon.ammoReserves.ToString();
        }
    }

    private void TryInitialize()
    {
        BindState();
        BindPlayer();
        RefreshAll();
    }

    private void BindState()
    {
        if (GameManager.Instance == null || GameManager.Instance.PlayerState == null)
        {
            return;
        }

        if (boundState == GameManager.Instance.PlayerState)
        {
            return;
        }

        UnbindState();

        boundState = GameManager.Instance.PlayerState;
        boundState.OnHealthChanged += HandleHealthChanged;
        boundState.OnStaminaChanged += HandleStaminaChanged;
    }

    private void UnbindState()
    {
        if (boundState == null)
        {
            return;
        }

        boundState.OnHealthChanged -= HandleHealthChanged;
        boundState.OnStaminaChanged -= HandleStaminaChanged;
        boundState = null;
    }

    private void BindPlayer()
    {
        if (player == null)
        {
            player = FindFirstObjectByType<playerController>();
        }

        if (player == null)
        {
            return;
        }

        combatScript = player.GetComponent<playerCombat>();
        player.OnEquippedWeaponChanged -= HandleWeaponChanged;
        player.OnEquippedWeaponChanged += HandleWeaponChanged;
    }

    private void UnbindPlayer()
    {
        if (player == null)
        {
            return;
        }

        player.OnEquippedWeaponChanged -= HandleWeaponChanged;
    }

    private void HandleHealthChanged(float currentHealth, float maxHealth)
    {
        UpdateFill(healthFill, currentHealth, maxHealth);
    }

    private void HandleStaminaChanged(float currentStamina, float maxStamina)
    {
        UpdateFill(staminaFill, currentStamina, maxStamina);
    }

    private void HandleWeaponChanged(WeaponData weapon)
    {
        UpdateWeaponIcon(weapon);
    }

    private void RefreshAll()
    {
        RefreshBars();
        RefreshWeaponIcon();
    }

    private void RefreshBars()
    {
        if (boundState == null)
        {
            UpdateFill(healthFill, 0f, 1f);
            UpdateFill(staminaFill, 0f, 1f);
            return;
        }

        UpdateFill(healthFill, boundState.CurrentHealth, boundState.MaxHealth);
        UpdateFill(staminaFill, boundState.CurrentStamina, boundState.MaxStamina);
    }

    private void RefreshWeaponIcon()
    {
        if (player == null || player.weapons == null || player.curSlot < 0 || player.curSlot >= player.weapons.Length)
        {
            UpdateWeaponIcon(null);
            return;
        }

        Weapon currentWeapon = player.weapons[player.curSlot];
        if (currentWeapon == null)
        {
            UpdateWeaponIcon(null);
            return;
        }

        WeaponData iconSource = currentWeapon.augmentedData;
        if (iconSource == null || iconSource.weaponSprite == null)
        {
            iconSource = currentWeapon.baseData;
        }

        UpdateWeaponIcon(iconSource);
    }

    private void UpdateWeaponIcon(WeaponData weapon)
    {
        if (weaponIcon == null)
        {
            return;
        }

        if (weapon == displayedWeapon)
        {
            return;
        }

        displayedWeapon = weapon;

        if (weapon == null || weapon.weaponSprite == null)
        {
            weaponIcon.sprite = null;
            weaponIcon.enabled = false;
            return;
        }

        weaponIcon.sprite = weapon.weaponSprite;
        weaponIcon.enabled = true;
        weaponIcon.preserveAspect = true;
    }

    private void UpdateFill(Image fillImage, float currentValue, float maxValue)
    {
        if (fillImage == null)
        {
            return;
        }

        float fillAmount = maxValue <= 0f ? 0f : Mathf.Clamp01(currentValue / maxValue);
        fillImage.fillAmount = fillAmount;
    }
}

