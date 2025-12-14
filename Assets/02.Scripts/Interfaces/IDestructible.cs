public interface IDestructible
{
    float CurrentHealth { get; }
    float MaxHealth { get; }
    bool IsDestroyed { get; }

    void OnDestruct();
}