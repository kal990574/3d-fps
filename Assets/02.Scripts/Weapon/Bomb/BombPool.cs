using UnityEngine;
using UnityEngine.Pool;

public class BombPool : MonoBehaviour, IBombPool
{
    [Header("Pool Settings")]
    [SerializeField] private Bomb _bombPrefab;
    [SerializeField] private int _maxPoolSize = 5;

    private ObjectPool<Bomb> _pool;

    public bool CanGet => ActiveCount < _maxPoolSize;
    public int ActiveCount => _pool.CountActive;
    public int MaxPoolSize => _maxPoolSize;
    public int AvailableCount => _maxPoolSize - ActiveCount;

    private void Awake()
    {
        InitializePool();
    }

    private void InitializePool()
    {
        _pool = new ObjectPool<Bomb>(
            createFunc: CreateBomb,
            actionOnGet: OnGetBomb,
            actionOnRelease: OnReleaseBomb,
            actionOnDestroy: OnDestroyBomb,
            collectionCheck: true,
            defaultCapacity: _maxPoolSize,
            maxSize: _maxPoolSize
        );
    }

    public Bomb Get(Vector3 position)
    {
        if (!CanGet)
        {
            return null;
        }

        Bomb bomb = _pool.Get();
        bomb.transform.position = position;
        return bomb;
    }

    public void Release(Bomb bomb)
    {
        if (!bomb.gameObject.activeInHierarchy)
        {
            return;
        }
        
        _pool.Release(bomb);
    }

    private Bomb CreateBomb()
    {
        Bomb bomb = Instantiate(_bombPrefab, transform);
        bomb.Initialize(this);
        bomb.gameObject.SetActive(false);
        return bomb;
    }

    private void OnGetBomb(Bomb bomb)
    {
        bomb.gameObject.SetActive(true);
    }

    private void OnReleaseBomb(Bomb bomb)
    {
        bomb.ResetState();
        bomb.gameObject.SetActive(false);
    }

    private void OnDestroyBomb(Bomb bomb)
    {
        Destroy(bomb.gameObject);
    }
}
