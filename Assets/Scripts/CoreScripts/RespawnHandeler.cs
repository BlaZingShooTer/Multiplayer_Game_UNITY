using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class RespawnHandeler : NetworkBehaviour
{
    [SerializeField] TankPlayer playerPrefab;

    [SerializeField] private float keptCoinPercentage;

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            return;
        }

        TankPlayer[] players = UnityEngine.Object.FindObjectsByType<TankPlayer>(FindObjectsSortMode.None);

        foreach (TankPlayer player in players)
        {
            OnPlayerSpawned(player);
        }

        TankPlayer.OnPlayerDeSpawned += OnPlayerDeSpawned;
        TankPlayer.OnPlayerSpawned += OnPlayerSpawned;
    }

    private void OnPlayerSpawned(TankPlayer player)
    {
        player.Health.onDie += (health) => 
        {
            HandlePlayerDie(player);
           
        };
    }

    private void OnPlayerDeSpawned(TankPlayer player)
    {
        player.Health.onDie -= (health) =>
        {
            HandlePlayerDie(player);

        };

    }


    private void HandlePlayerDie(TankPlayer player) 
    {
        int keptCoins = (int)(player.Wallet.TotalCoinValue.Value * (keptCoinPercentage / 100));

        Destroy(player.gameObject);

        StartCoroutine(RespawnPlayer(player.OwnerClientId , keptCoins));

    }

    private IEnumerator RespawnPlayer (ulong ownerclientId , int keptCoins)
    {
        yield return null;

        TankPlayer playerInstance = Instantiate(playerPrefab, SpawnPoint.GetRandomSpawnPoint(), Quaternion.identity);
        

        playerInstance.NetworkObject.SpawnAsPlayerObject(ownerclientId);

        playerInstance.Wallet.TotalCoinValue.Value += keptCoins;

    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            TankPlayer.OnPlayerDeSpawned -= OnPlayerDeSpawned;
            TankPlayer.OnPlayerSpawned -= OnPlayerSpawned;
        }
    }
}
