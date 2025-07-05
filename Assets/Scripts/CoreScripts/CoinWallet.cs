using System;
using Unity.Netcode;
using UnityEngine;

public class CoinWallet : NetworkBehaviour
{
    public NetworkVariable<int> TotalCoinValue = new NetworkVariable<int>();

    [SerializeField] private Health health;

    [SerializeField] private BountyCoin coinPrefab;

    [SerializeField] private float bountyPercentage;
 
    [SerializeField] private float coinSpread = 3f;

    [SerializeField] private int bountyCoinCount = 10;

    [SerializeField] private int minBountyCoinValue = 5;

    [SerializeField] private LayerMask layerMask;

    private Collider2D[] coinBuffers = new Collider2D[1];

    private float coinRadius;


    public override void OnNetworkSpawn()
    {
        if(!IsServer) { return; }

        coinRadius = coinPrefab.GetComponent<CircleCollider2D>().radius;

        health.onDie += HandleDie;
    }


    public override void OnNetworkDespawn()
    {
        if(!IsServer){ return; }

        health.onDie -= HandleDie;
    }
    private void HandleDie(Health health)
    {
        int bountyValue = (int)(TotalCoinValue.Value * (bountyPercentage/ 100f));

        int bountyCoinValue = bountyValue / bountyCoinCount;

        if(bountyCoinValue < minBountyCoinValue) { return; }

        for (int i = 0; i < coinBuffers.Length; i++) 
        {
            BountyCoin coinInstance =  Instantiate(coinPrefab, GetSpawnPoint(), Quaternion.identity);
            coinInstance.SetValue(bountyCoinValue);
            coinInstance.NetworkObject.Spawn();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsServer) 
        {
            return;
        }

        if(collision.TryGetComponent<Coin>(out Coin coin))
        {
            int coinValue = coin.Collect();
            
            TotalCoinValue.Value += coinValue;
        }
    }


    private Vector2 GetSpawnPoint() 
    {
        while (true) 
        {
            Vector2 spawnPoint = (Vector2)transform.position + UnityEngine.Random.insideUnitCircle * coinSpread;

            coinBuffers = Physics2D.OverlapCircleAll(spawnPoint, coinRadius, layerMask);

            if(coinBuffers.Length == 0) 
            {
                return spawnPoint;
            }

        }
    }


    public void SpendCoins(int val) 
    {
        TotalCoinValue.Value -= val;
    }
}
