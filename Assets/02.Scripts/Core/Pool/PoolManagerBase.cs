using UnityEngine;
using System;
using System.Collections.Generic;

public abstract class PoolManagerBase<T> : MonoBehaviour where T : Component
{
    [Serializable]
    public class PoolConfig
    {
        public string Name;
        public GameObject Prefab;
        public int InitialSize = 10;
        public int MaxSize = 100;
    }

    [SerializeField] protected PoolConfig[] _configs;
    protected Dictionary<string, GenericObjectPool<T>> _pools = new Dictionary<string, GenericObjectPool<T>>();

    protected virtual void Awake()
    {
        InitializePools();
    }

    protected void InitializePools()
    {
        foreach (var config in _configs)
        {
            if (config.Prefab == null)
            {
                Debug.LogError($"Pool config '{config.Name}' has null prefab");
                continue;
            }

            var pool = new GenericObjectPool<T>(
                config.Prefab,
                transform,
                config.InitialSize,
                config.MaxSize
            );
            _pools[config.Name] = pool;
        }
    }

    public T Get(string poolName, Vector3 position)
    {
        if (_pools.TryGetValue(poolName, out var pool))
        {
            T obj = pool.Get();
            if (obj != null)
            {
                obj.transform.position = position;
            }
            return obj;
        }

        Debug.LogWarning($"Pool '{poolName}' not found in {GetType().Name}");
        return null;
    }

    public void Release(string poolName, T obj)
    {
        if (_pools.TryGetValue(poolName, out var pool))
        {
            pool.Release(obj);
        }
        else
        {
            Debug.LogWarning($"Pool '{poolName}' not found in {GetType().Name}");
        }
    }

    public int GetActiveCount(string poolName)
    {
        if (_pools.TryGetValue(poolName, out var pool))
        {
            return pool.CountActive;
        }
        return 0;
    }

    public int GetInactiveCount(string poolName)
    {
        if (_pools.TryGetValue(poolName, out var pool))
        {
            return pool.CountInactive;
        }
        return 0;
    }
}
