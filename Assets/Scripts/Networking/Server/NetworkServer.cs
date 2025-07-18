using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkServer : IDisposable
{
    private NetworkManager networkManager;

    private Dictionary<ulong , string> clientIdToAuth =  new Dictionary<ulong, string>();

    private Dictionary<string, UserData> authIdToUserData= new Dictionary<string, UserData>();

    public NetworkServer(NetworkManager networkManager) 
    {
        this.networkManager = networkManager;

        networkManager.NetworkConfig.ConnectionApproval = true;

        networkManager.ConnectionApprovalCallback += OnConnectionApproval;

        networkManager.OnServerStarted += OnNetworkReady;
    }

    private void OnNetworkReady()
    {
        networkManager.OnClientDisconnectCallback +=  OnClientDisconnected;


    }

    public UserData GetUserData(ulong clientId)
    {
        if(clientIdToAuth.TryGetValue(clientId, out string authId))
        {
            if(authIdToUserData.TryGetValue(authId, out UserData userData))
            {
                return userData;
            }

            return null;
        }
        return null;
    }

    private void OnClientDisconnected(ulong clientId)
    {
        if(clientIdToAuth.TryGetValue(clientId, out string authId))
        {
            clientIdToAuth.Remove(clientId);

            authIdToUserData.Remove(authId);

        }
    }

    private void OnConnectionApproval(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        string payload =  System.Text.Encoding.UTF8.GetString(request.Payload);

        UserData userData = JsonUtility.FromJson<UserData>(payload);

        clientIdToAuth[request.ClientNetworkId] = userData.UserAuthId;

        authIdToUserData[userData.UserAuthId] = userData;

        response.Approved = true;

        response.Position = SpawnPoint.GetRandomSpawnPoint();

        response.Rotation = Quaternion.identity;

        response.CreatePlayerObject = true;
    }

    public void Dispose()
    {
       if(networkManager  == null) { return; }

       networkManager.ConnectionApprovalCallback -= OnConnectionApproval;
        networkManager.OnServerStarted -= OnNetworkReady;
        networkManager.OnClientDisconnectCallback -= OnClientDisconnected;
       
        if(networkManager.IsListening)
        {
            networkManager.Shutdown();
        }
    }
}
