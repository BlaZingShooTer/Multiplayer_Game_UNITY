using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LeaderBoard :NetworkBehaviour
{
    [SerializeField] private Transform leaderboardEntityHolder;

    [SerializeField] private LeaderBoardEntities leaderboardEntityPrefab;

    [SerializeField] private int entitiesToDisplay = 8;


    private NetworkList<LeaderBoardEntityState> leaderBoardEntities;

    private List<LeaderBoardEntities> entityDisplays = new List<LeaderBoardEntities>();
    private void Awake()
    {
        leaderBoardEntities = new NetworkList<LeaderBoardEntityState>();

    }

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            leaderBoardEntities.OnListChanged += HandleLeaderBoardEntitiesChanged;
            foreach (LeaderBoardEntityState entity in leaderBoardEntities)
            {
                HandleLeaderBoardEntitiesChanged(new NetworkListEvent<LeaderBoardEntityState>
                {
                    Type = NetworkListEvent<LeaderBoardEntityState>.EventType.Add,
                    Value = entity
                });
            }
        }

            if (IsServer)
            {
                TankPlayer[] players = Object.FindObjectsByType<TankPlayer>(FindObjectsSortMode.None);

                foreach (TankPlayer player in players)
                {
                    HandlePlayerSpawn(player);
                }

                TankPlayer.OnPlayerSpawned += HandlePlayerSpawn;
                TankPlayer.OnPlayerDeSpawned += HandlePlayerDespawn;
            }
        
    }

    private void HandleLeaderBoardEntitiesChanged(NetworkListEvent<LeaderBoardEntityState> changeEvent)
    {
        switch (changeEvent.Type) 
        {
            case NetworkListEvent<LeaderBoardEntityState>.EventType.Add:
                if(!entityDisplays.Any(x=> x.ClientId == changeEvent.Value.ClientId)) 
                {
                   LeaderBoardEntities leaderBoardEntities = 
                        Instantiate(leaderboardEntityPrefab, leaderboardEntityHolder);

                    leaderBoardEntities.Initialise(changeEvent.Value.ClientId, 
                        changeEvent.Value.PlayerName, changeEvent.Value.Coins);

                    entityDisplays.Add(leaderBoardEntities);

                }
                break;
            case NetworkListEvent<LeaderBoardEntityState>.EventType.Remove:
                LeaderBoardEntities displayToRemove = 
                    entityDisplays.FirstOrDefault(x => x.ClientId == changeEvent.Value.ClientId);
                if (displayToRemove != null)
                {
                    displayToRemove.transform.SetParent(null);
                    Destroy(displayToRemove.gameObject);
                    entityDisplays.Remove(displayToRemove);
                }
                break;

            case NetworkListEvent<LeaderBoardEntityState>.EventType.Value:
                LeaderBoardEntities displayToUpdate = 
                    entityDisplays.FirstOrDefault(x => x.ClientId == changeEvent.Value.ClientId);
                if (displayToUpdate != null) 
                {
                    displayToUpdate.UpdateCoins(changeEvent.Value.Coins);
                }
                    break;
        }

        entityDisplays.Sort((x, y) => y.Coins.CompareTo(x.Coins));

        for(int i =0;i < entityDisplays.Count; i++)
        {
            entityDisplays[i].transform.SetSiblingIndex(i);
            entityDisplays[i].UpdateText();

            bool shouldDisplay = i <= entitiesToDisplay -1;
            entityDisplays[i].gameObject.SetActive(shouldDisplay);
        }

       LeaderBoardEntities myDisplays =  entityDisplays.FirstOrDefault(x => x.ClientId == NetworkManager.Singleton.LocalClientId);

        if (myDisplays != null)
        {
           if(myDisplays.transform.GetSiblingIndex() >= entitiesToDisplay)
            {
                leaderboardEntityHolder.GetChild(entitiesToDisplay - 1).gameObject.SetActive(false);
                myDisplays.gameObject.SetActive(true);
            }
           
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsClient) 
        {
            leaderBoardEntities.OnListChanged -= HandleLeaderBoardEntitiesChanged;
        }


        if (!IsServer)
        {
            return;
        }
        TankPlayer.OnPlayerSpawned -= HandlePlayerSpawn;
        TankPlayer.OnPlayerDeSpawned -= HandlePlayerDespawn;
        // Clear the leaderboard entities
        
    }


    private void HandlePlayerSpawn(TankPlayer player) 
    {

        leaderBoardEntities.Add(new LeaderBoardEntityState
        {
            ClientId = player.OwnerClientId,
            PlayerName = player.PlayerName.Value,
            Coins = 0
        });

        player.Wallet.TotalCoinValue.OnValueChanged += (oldCoins, newCoins) => HandleCoinsChanged(player.OwnerClientId, newCoins);
    }

    private void HandleCoinsChanged(ulong clientId, int newCoins)
    {
       for (int i = 0; i < leaderBoardEntities.Count; i++)
        {
            if (leaderBoardEntities[i].ClientId != clientId)
            {
                continue;
            }

            leaderBoardEntities[i] = new LeaderBoardEntityState
            {
                ClientId = leaderBoardEntities[i].ClientId,
                PlayerName = leaderBoardEntities[i].PlayerName,
                Coins = newCoins
            };

            return;
        }
        
    }

    private void HandlePlayerDespawn(TankPlayer player)
    {
        if(leaderBoardEntities == null)
        {
            return;
        }

        foreach (LeaderBoardEntityState entity in leaderBoardEntities)
        {
            if (entity.ClientId != player.OwnerClientId)
            {
                continue;
            }

            leaderBoardEntities.Remove(entity);
        }

        player.Wallet.TotalCoinValue.OnValueChanged -= (oldCoins, newCoins) => HandleCoinsChanged(player.OwnerClientId, newCoins);
    }
}
