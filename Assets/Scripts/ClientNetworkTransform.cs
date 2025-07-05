using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class ClientNetworkTransform : NetworkTransform
{
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        CanCommitToTransform = IsOwner;
    }

    public override void OnUpdate()
    {
        CanCommitToTransform = IsOwner; // Only allow the owner to commit to the transform
        base.OnUpdate();

        if(NetworkManager!= null)
        {

            if(NetworkManager.IsConnectedClient || NetworkManager.IsListening) 
            {
                if (CanCommitToTransform) 
                {
                    //trycommitTransformToServer(transform, NetworkManager.LocalTime.Time);
                }
            }

        }
    }

    protected override bool OnIsServerAuthoritative()
    {
        return false; // This client is not authoritative over the transform
    }
}
