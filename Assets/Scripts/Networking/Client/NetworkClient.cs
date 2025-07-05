using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkClient : IDisposable
{
    private NetworkManager networkManager;

    private const string MenuSceneName = "Menu";
    public NetworkClient(NetworkManager networkManager)
    {
        this.networkManager = networkManager;

        networkManager.OnClientDisconnectCallback += OnClientDisconnected;
    }



    private void OnClientDisconnected(ulong clientId)
    {
        if(clientId != 0  && clientId!= networkManager.LocalClientId) 
        {
            return;
        }


        if(SceneManager.GetActiveScene().name != MenuSceneName)
        {
            SceneManager.LoadScene(MenuSceneName);
        }


        if (networkManager.IsConnectedClient) 
        {
            networkManager.Shutdown();
        }
    }

    public void Dispose()
    {
       if(networkManager != null)
       {
            networkManager.OnClientDisconnectCallback -= OnClientDisconnected;
            networkManager = null;
        }
    }
}
