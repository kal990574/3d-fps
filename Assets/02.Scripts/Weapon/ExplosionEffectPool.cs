using UnityEngine;
using UnityEngine.Pool;
using System.Collections;

public class ExplosionEffectPool : MonoBehaviour, IEffectPool
{
    [Header("Pool Settings")]
    [SerializeField] private ParticleSystem _effectPrefab;
    [SerializeField] private int _maxPoolSize = 5;

    private ObjectPool<ParticleSystem> _pool;

    private void Awake()
    {
        InitializePool();
    }

    private void InitializePool()
    {
        _pool = new ObjectPool<ParticleSystem>(
            createFunc: CreateEffect,
            actionOnGet: OnGetEffect,
            actionOnRelease: OnReleaseEffect,
            actionOnDestroy: OnDestroyEffect,
            collectionCheck: true,
            defaultCapacity: _maxPoolSize,
            maxSize: _maxPoolSize
        );
    }

    public void Play(Vector3 position)
    {
        ParticleSystem effect = _pool.Get();
        effect.transform.position = position;
        effect.Play();

        StartCoroutine(ReleaseAfterPlay(effect));
    }

    private IEnumerator ReleaseAfterPlay(ParticleSystem effect)
    {
        yield return new WaitForSeconds(effect.main.duration);

        if (effect.gameObject.activeInHierarchy)
        {
            _pool.Release(effect);
        }
    }

    private ParticleSystem CreateEffect()
    {
        ParticleSystem effect = Instantiate(_effectPrefab, transform);
        effect.gameObject.SetActive(false);
        return effect;
    }

    private void OnGetEffect(ParticleSystem effect)
    {
        effect.gameObject.SetActive(true);
    }

    private void OnReleaseEffect(ParticleSystem effect)
    {
        effect.Stop();
        effect.Clear();
        effect.gameObject.SetActive(false);
    }

    private void OnDestroyEffect(ParticleSystem effect)
    {
        Destroy(effect.gameObject);
    }
}
