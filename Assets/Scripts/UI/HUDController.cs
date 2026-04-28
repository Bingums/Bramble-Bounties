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

    private PlayerState boundState;

    private void OnEnable()
    {
        BindState();
        BindPlayer();
        RefreshAll();
    }

    private void OnDisable()
    {
        UnbindState();
        UnbindPlayer();
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

        UpdateWeaponIcon(player.weapons[player.curSlot].augmentedData);
    }

    private void UpdateWeaponIcon(WeaponData weapon)
    {
        if (weaponIcon == null)
        {
            return;
        }

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

