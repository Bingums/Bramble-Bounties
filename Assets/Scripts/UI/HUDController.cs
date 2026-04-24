using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [Header("Bar Fills")]
    [SerializeField] private Image healthFill;
    [SerializeField] private Image staminaFill;

    private PlayerState boundState;

    private void OnEnable()
    {
        TryBindToPlayerState();
        RefreshBars();
    }

    private void Update()
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        if (boundState != GameManager.Instance.PlayerState)
        {
            TryBindToPlayerState();
        }
    }

    private void OnDisable()
    {
        UnbindFromPlayerState();
    }

    private void TryBindToPlayerState()
    {
        if (GameManager.Instance == null || GameManager.Instance.PlayerState == null)
        {
            return;
        }

        if (boundState == GameManager.Instance.PlayerState)
        {
            return;
        }

        UnbindFromPlayerState();

        boundState = GameManager.Instance.PlayerState;
        boundState.OnHealthChanged += HandleHealthChanged;
        boundState.OnStaminaChanged += HandleStaminaChanged;

        RefreshBars();
    }

    private void UnbindFromPlayerState()
    {
        if (boundState == null)
        {
            return;
        }

        boundState.OnHealthChanged -= HandleHealthChanged;
        boundState.OnStaminaChanged -= HandleStaminaChanged;
        boundState = null;
    }

    private void HandleHealthChanged(float currentHealth, float maxHealth)
    {
        UpdateFill(healthFill, currentHealth, maxHealth);
    }

    private void HandleStaminaChanged(float currentStamina, float maxStamina)
    {
        UpdateFill(staminaFill, currentStamina, maxStamina);
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
