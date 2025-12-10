using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [Header("Shake Settings")]
    [SerializeField] private float _maxShakeAngle = 2f;
    [SerializeField] private float _frequency = 60f;
    [SerializeField] private float _traumaDecay = 2.5f;

    private float _trauma;
    private float _seed;
    private Vector3 _originalRotation;

    private void Awake()
    {
        _seed = Random.value * 1000f;
    }

    private void LateUpdate()
    {
        if (_trauma <= 0f)
        {
            return;
        }

        ApplyShake();
        DecayTrauma();
    }

    public void AddTrauma(float amount)
    {
        _trauma = Mathf.Clamp01(_trauma + amount);
    }

    private void ApplyShake()
    {
        float shake = _trauma * _trauma;
        float time = Time.time * _frequency;

        float offsetX = GetPerlinValue(_seed, time) * _maxShakeAngle * shake;
        float offsetY = GetPerlinValue(_seed + 1f, time) * _maxShakeAngle * shake;
        float offsetZ = GetPerlinValue(_seed + 2f, time) * _maxShakeAngle * shake * 0.5f;

        transform.localRotation *= Quaternion.Euler(offsetX, offsetY, offsetZ);
    }

    private float GetPerlinValue(float seedOffset, float time)
    {
        return (Mathf.PerlinNoise(seedOffset, time) - 0.5f) * 2f;
    }

    private void DecayTrauma()
    {
        _trauma = Mathf.Max(0f, _trauma - _traumaDecay * Time.deltaTime);
    }
}
