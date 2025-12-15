using System;

public static class GameEvents
{
    // Player Stats Events
    public static event Action<float, float> OnHealthChanged;  // (current, max)
    public static event Action<float, float> OnStaminaChanged; // (current, max)
    public static event Action OnPlayerDeath;
    public static event Action OnPlayerDamaged;

    // Bomb Pool Events
    public static event Action<int, int> OnBombCountChanged;   // (available, max)

    // Game State Events
    public static event Action<EGameState> OnGameStateChanged;
    public static event Action OnGamePaused;
    public static event Action OnGameResumed;

    // Trigger Methods
    public static void TriggerHealthChanged(float current, float max)
        => OnHealthChanged?.Invoke(current, max);

    public static void TriggerStaminaChanged(float current, float max)
        => OnStaminaChanged?.Invoke(current, max);

    public static void TriggerPlayerDeath()
        => OnPlayerDeath?.Invoke();

    public static void TriggerPlayerDamaged()
        => OnPlayerDamaged?.Invoke();

    public static void TriggerBombCountChanged(int available, int max)
        => OnBombCountChanged?.Invoke(available, max);

    public static void TriggerGameStateChanged(EGameState newState)
        => OnGameStateChanged?.Invoke(newState);

    public static void TriggerGamePaused()
        => OnGamePaused?.Invoke();

    public static void TriggerGameResumed()
        => OnGameResumed?.Invoke();
}