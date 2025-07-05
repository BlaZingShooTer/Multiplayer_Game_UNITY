using Unity.Netcode;
using UnityEngine;
using Unity.Cinemachine;
using Unity.Collections;
using System.Threading.Tasks;
using System;


[SerializeField]
public class TankPlayer : NetworkBehaviour
{
    [Header("References")]    

    [SerializeField] private CinemachineCamera virtualCamera;

    [field: SerializeField] public Health Health {get; private set; }

    [field: SerializeField] public CoinWallet Wallet { get; private set; }

    [SerializeField] private SpriteRenderer miniMapIconRenderer;



    [Header("Settings")]

    [SerializeField] private int ownerPriority = 15;

    [SerializeField] private Color OwnerColor;

    public NetworkVariable<FixedString32Bytes> PlayerName = new NetworkVariable<FixedString32Bytes>();


    public static event Action<TankPlayer> OnPlayerSpawned;

    public static event Action<TankPlayer> OnPlayerDeSpawned;






    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            UserData userData =
                 HostSingleton.Instance.hostGameManager.networkServer.GetUserData(OwnerClientId);

            PlayerName.Value = userData.UserName;

            OnPlayerSpawned?.Invoke(this);
        }

        if (IsOwner) 
        {
            virtualCamera.Priority = ownerPriority;

            miniMapIconRenderer.color = OwnerColor; 
        }
    }


    override public void OnNetworkDespawn()
    {
        if (IsServer)
        {
            OnPlayerDeSpawned?.Invoke(this);
        }

    }

}
