using System;

[Serializable]
public class PlayerState
{
    public float CurrentHealth { get; private set; }
    public float MaxHealth { get; private set; }

    public float CurrentStamina { get; private set; }
    public float MaxStamina { get; private set; }

    public int DamageMultiplier { get; private set; }

    public event Action OnStateChanged;
    public event Action<float, float> OnHealthChanged;
    public event Action<float, float> OnStaminaChanged;

    public PlayerState(
        float currentHealth,
        float maxHealth,
        float currentStamina,
        float maxStamina,
        int damageMultiplier)
    {
        MaxHealth = Math.Max(1f, maxHealth);
        CurrentHealth = Clamp(currentHealth, 0f, MaxHealth);

        MaxStamina = Math.Max(1f, maxStamina);
        CurrentStamina = Clamp(currentStamina, 0f, MaxStamina);

        DamageMultiplier = Math.Max(1, damageMultiplier);
    }

    public void TakeDamage(float amount)
    {
        if (amount <= 0f)
        {
            return;
        }

        CurrentHealth = Clamp(CurrentHealth - amount, 0f, MaxHealth);
        NotifyHealthChanged();
    }

    public void Heal(float amount)
    {
        if (amount <= 0f)
        {
            return;
        }

        CurrentHealth = Clamp(CurrentHealth + amount, 0f, MaxHealth);
        NotifyHealthChanged();
    }

    public void SetHealth(float value)
    {
        CurrentHealth = Clamp(value, 0f, MaxHealth);
        NotifyHealthChanged();
    }

    public void SetMaxHealth(float value, bool refillToFull = false)
    {
        MaxHealth = Math.Max(1f, value);
        CurrentHealth = refillToFull ? MaxHealth : Clamp(CurrentHealth, 0f, MaxHealth);
        NotifyHealthChanged();
    }

    public bool HasEnoughStamina(float amount)
    {
        return CurrentStamina >= amount;
    }

    public bool TrySpendStamina(float amount)
    {
        if (amount <= 0f)
        {
            return true;
        }

        if (CurrentStamina < amount)
        {
            return false;
        }

        CurrentStamina = Clamp(CurrentStamina - amount, 0f, MaxStamina);
        NotifyStaminaChanged();
        return true;
    }

    public void RestoreStamina(float amount)
    {
        if (amount <= 0f)
        {
            return;
        }

        CurrentStamina = Clamp(CurrentStamina + amount, 0f, MaxStamina);
        NotifyStaminaChanged();
    }

    public void SetStamina(float value)
    {
        CurrentStamina = Clamp(value, 0f, MaxStamina);
        NotifyStaminaChanged();
    }

    public void SetMaxStamina(float value, bool refillToFull = false)
    {
        MaxStamina = Math.Max(1f, value);
        CurrentStamina = refillToFull ? MaxStamina : Clamp(CurrentStamina, 0f, MaxStamina);
        NotifyStaminaChanged();
    }

    public void SetDamageMultiplier(int value)
    {
        DamageMultiplier = Math.Max(1, value);
        NotifyStateChanged();
    }

    public bool IsDead()
    {
        return CurrentHealth <= 0f;
    }

    private void NotifyHealthChanged()
    {
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
        NotifyStateChanged();
    }

    private void NotifyStaminaChanged()
    {
        OnStaminaChanged?.Invoke(CurrentStamina, MaxStamina);
        NotifyStateChanged();
    }

    private void NotifyStateChanged()
    {
        OnStateChanged?.Invoke();
    }

    private float Clamp(float value, float min, float max)
    {
        if (value < min)
        {
            return min;
        }

        if (value > max)
        {
            return max;
        }

        return value;
    }
}
