using System;

public static class GameEvents
{
    // Player Stats Events
    public static event Action<float, float> OnHealthChanged;  // (current, max)
    public static event Action<float, float> OnStaminaChanged; // (current, max)
    public static event Action OnPlayerDeath;

    // Bomb Pool Events
    public static event Action<int, int> OnBombCountChanged;   // (available, max)

    // Trigger Methods
    public static void TriggerHealthChanged(float current, float max)
        => OnHealthChanged?.Invoke(current, max);

    public static void TriggerStaminaChanged(float current, float max)
        => OnStaminaChanged?.Invoke(current, max);

    public static void TriggerPlayerDeath()
        => OnPlayerDeath?.Invoke();

    public static void TriggerBombCountChanged(int available, int max)
        => OnBombCountChanged?.Invoke(available, max);
}