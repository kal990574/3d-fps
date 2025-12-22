using UnityEngine;
using System.Linq;

public class GameplayPoolManager : PoolManagerBase<Transform>
{
    public static GameplayPoolManager Instance { get; private set; }

    private int _bombMaxSize;

    protected override void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        base.Awake();
        CacheBombPoolConfig();
    }

    private void Start()
    {
        NotifyBombCountChanged();
    }

    private void CacheBombPoolConfig()
    {
        var bombConfig = _configs.FirstOrDefault(c => c.Name == "Bomb");
        _bombMaxSize = bombConfig?.MaxSize ?? 5;
    }

    public Monster SpawnMonster(string monsterType, Vector3 position)
    {
        Transform obj = Get(monsterType, position);
        return obj != null ? obj.GetComponent<Monster>() : null;
    }

    public Bomb GetBomb(Vector3 position)
    {
        if (GetActiveCount("Bomb") >= _bombMaxSize)
        {
            return null;
        }

        Transform obj = Get("Bomb", position);
        if (obj == null)
        {
            return null;
        }

        NotifyBombCountChanged();
        return obj.GetComponent<Bomb>();
    }

    public void ReleaseBomb(Bomb bomb)
    {
        if (bomb != null)
        {
            Release("Bomb", bomb.transform);
            NotifyBombCountChanged();
        }
    }

    private void NotifyBombCountChanged()
    {
        int activeCount = GetActiveCount("Bomb");
        int availableCount = _bombMaxSize - activeCount;
        GameEvents.TriggerBombCountChanged(availableCount, _bombMaxSize);
    }

    public void ReleaseMonster(Monster monster)
    {
        if (monster != null)
        {
            string monsterType = monster.gameObject.name.Replace("(Clone)", "").Trim();
            Release(monsterType, monster.transform);
        }
    }

    public Coin SpawnCoin(Vector3 position)
    {
        Transform obj = Get("Coin", position);
        return obj != null ? obj.GetComponent<Coin>() : null;
    }

    public void ReleaseCoin(Coin coin)
    {
        if (coin != null)
        {
            Release("Coin", coin.transform);
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
