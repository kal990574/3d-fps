using UnityEngine;
using UnityEngine.Pool;

public class GenericObjectPool<T> where T : Component
{
    private readonly GameObject _prefab;
    private readonly Transform _parent;
    private readonly ObjectPool<T> _pool;

    public int CountActive => _pool.CountActive;
    public int CountInactive => _pool.CountInactive;
    public int CountAll => _pool.CountAll;

    public GenericObjectPool(
        GameObject prefab,
        Transform parent,
        int defaultCapacity = 10,
        int maxSize = 100)
    {
        _prefab = prefab;
        _parent = parent;

        _pool = new ObjectPool<T>(
            createFunc: Create,
            actionOnGet: OnGet,
            actionOnRelease: OnRelease,
            actionOnDestroy: OnDestroy,
            collectionCheck: true,
            defaultCapacity: defaultCapacity,
            maxSize: maxSize
        );
    }

    public T Get()
    {
        return _pool.Get();
    }

    public void Release(T obj)
    {
        if (obj == null || !obj.gameObject.activeInHierarchy)
        {
            return;
        }

        _pool.Release(obj);
    }

    public void Clear()
    {
        _pool.Clear();
    }

    private T Create()
    {
        GameObject go = Object.Instantiate(_prefab, _parent);
        go.SetActive(false);
        return go.GetComponent<T>();
    }

    private void OnGet(T obj)
    {
        obj.gameObject.SetActive(true);

        IPoolable poolable = obj as IPoolable ?? obj.GetComponent<IPoolable>();
        poolable?.OnGetFromPool();
    }

    private void OnRelease(T obj)
    {
        IPoolable poolable = obj as IPoolable ?? obj.GetComponent<IPoolable>();
        poolable?.OnReturnToPool();

        obj.gameObject.SetActive(false);
    }

    private void OnDestroy(T obj)
    {
        if (obj != null)
        {
            Object.Destroy(obj.gameObject);
        }
    }
}
