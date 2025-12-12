using System;
using UnityEngine;

[Serializable]
public class Recoil
{
    [Header("Vertical Recoil (Up)")]
    [SerializeField] private float _verticalMin = 1f;
    [SerializeField] private float _verticalMax = 2f;

    [Header("Horizontal Recoil (Left/Right)")]
    [SerializeField] private float _horizontalMin = -0.5f;
    [SerializeField] private float _horizontalMax = 0.5f;

    public Vector2 GetRecoil()
    {
        float vertical = UnityEngine.Random.Range(_verticalMin, _verticalMax);
        float horizontal = UnityEngine.Random.Range(_horizontalMin, _horizontalMax);

        return new Vector2(vertical, horizontal);
    }
}
