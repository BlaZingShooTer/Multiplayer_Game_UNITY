using Unity.Netcode;
using UnityEngine;

public class CoinSpawnner : NetworkBehaviour
{
    [SerializeField] private RespawningCoin coinPrefab;

    [SerializeField] private int maxCoins;

    [SerializeField] private int coinValue;

    [SerializeField] private Vector2 xSpawnRange;

    [SerializeField] private Vector2 ySpawnRange;

    [SerializeField] private LayerMask layerMask;

    private Collider2D[] coinBuffers = new Collider2D[1];

    private float coinRadius;
    public override void OnNetworkSpawn()
    {
        if(!IsServer) return;

        coinRadius = coinPrefab.GetComponent<CircleCollider2D>().radius;

        for (int i = 0; i < maxCoins; i++)
        {
            SpawnCoin();
        }
    }
    private void SpawnCoin() 
    {
        RespawningCoin coinInstance = Instantiate(coinPrefab, GetSpawnPosition(), Quaternion.identity);

        coinInstance.SetValue(coinValue);

        coinInstance.GetComponent<NetworkObject>().Spawn();

        coinInstance.onCollected += OnCoinCollected;

    }

    private void OnCoinCollected(RespawningCoin coin)
    {
        coin.transform.position = GetSpawnPosition();
        coin.Reset();
    }

    private Vector2 GetSpawnPosition() 
    {

        float x = 0;
        float y = 0;

        while (true) 
        {
            x = Random.Range(xSpawnRange.x, xSpawnRange.y);
            y = Random.Range(ySpawnRange.x, ySpawnRange.y);
            Vector2 spawnPosition = new Vector2(x, y);

            coinBuffers = Physics2D.OverlapCircleAll(spawnPosition, coinRadius, layerMask);
            if (coinBuffers.Length == 0)
            {
                return spawnPosition;
            }
        }
    }
}
