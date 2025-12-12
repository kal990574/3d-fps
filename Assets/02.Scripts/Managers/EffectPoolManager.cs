using UnityEngine;
using System.Collections;

public class EffectPoolManager : PoolManagerBase<ParticleSystem>
{
    public static EffectPoolManager Instance { get; private set; }

    protected override void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        base.Awake();
    }

    public void Play(string effectName, Vector3 position)
    {
        Play(effectName, position, Quaternion.identity);
    }

    public void Play(string effectName, Vector3 position, Quaternion rotation)
    {
        ParticleSystem effect = Get(effectName, position);
        if (effect != null)
        {
            effect.transform.rotation = rotation;
            effect.Play();
            StartCoroutine(AutoRelease(effectName, effect));
        }
    }

    private IEnumerator AutoRelease(string effectName, ParticleSystem effect)
    {
        yield return new WaitForSeconds(effect.main.duration);

        if (effect.gameObject.activeInHierarchy)
        {
            Release(effectName, effect);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
