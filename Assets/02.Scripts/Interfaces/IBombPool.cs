using UnityEngine;

public interface IBombPool
{
    bool CanGet { get; }
    int ActiveCount { get; }
    Bomb Get(Vector3 position);
    void Release(Bomb bomb);
}
