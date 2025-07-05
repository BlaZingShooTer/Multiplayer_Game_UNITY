using System;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientGameManager : IDisposable
{
    private JoinAllocation allocation;
    
    private NetworkClient networkClient;

    private const string MenuSceneName = "Menu";    
    public async Task<bool> InitAsync() 
    {
        await UnityServices.InitializeAsync();

        networkClient = new NetworkClient(NetworkManager.Singleton);

        AuthState authState =  await AuthenticationWrapper.DoAuth();

        if (authState == AuthState.Authenticated)
        {

            return true;

        }

        return false;
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(MenuSceneName);
    }

    public async Task StartClientAsync(string joinCode)
    {
        try 
        {
            allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

        }

        catch (Exception e) 
        {
            Debug.LogError($"Error starting client: {e.Message}");
            return;
        }

        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        RelayServerData relayServerData = AllocationUtils.ToRelayServerData(allocation, "dtls");

        transport.SetRelayServerData(relayServerData);

        Debug.Log("staerting cliet");

        UserData userData = new UserData
        {
            UserName = PlayerPrefs.GetString(NameSelector.PlayerNamekey, "Missing Name"),
            UserAuthId = AuthenticationService.Instance.PlayerId
        };

        string payload = JsonUtility.ToJson(userData);

        byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);

        NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;



        NetworkManager.Singleton.StartClient();
    }

    public void Dispose()
    {
       networkClient?.Dispose();
    }
}
