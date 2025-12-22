using UnityEngine;

public static class CoinSpawner
{
    private const float SCATTER_RADIUS = 0.5f;
    private const float SPAWN_HEIGHT_OFFSET = 1f;

    public static void SpawnCoins(Vector3 position, int count)
    {
        if (GameplayPoolManager.Instance == null)
        {
            Debug.LogWarning("[CoinSpawner] GameplayPoolManager not found!");
            return;
        }

        Vector3 spawnPosition = position + Vector3.up * SPAWN_HEIGHT_OFFSET;

        for (int i = 0; i < count; i++)
        {
            Vector3 scatterDirection = GetRandomScatterDirection();
            Vector3 coinPosition = spawnPosition + scatterDirection * SCATTER_RADIUS * 0.5f;

            Coin coin = GameplayPoolManager.Instance.SpawnCoin(coinPosition);
            if (coin != null)
            {
                coin.Initialize(scatterDirection);
            }
        }
    }

    private static Vector3 GetRandomScatterDirection()
    {
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
    }
}
