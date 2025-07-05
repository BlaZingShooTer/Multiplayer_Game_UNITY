using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;


public class HostGameManager : IDisposable
{
    private const string sceneName = "Game";

    private Allocation allocation;

    private string joinCode;

    private string lobbyId;

    public  NetworkServer networkServer { get; private set; }   

    private const int maxConnections = 20;
    public async Task StartHostAsync() 
    {
        try 
        {
           allocation =  await RelayService.Instance.CreateAllocationAsync(maxConnections);
        }
        catch(Exception e) 
        {
            Debug.LogError($"Error starting host: {e.Message}");
            return;
        }

        try
        {
            joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log($"Join code: {joinCode}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error starting host: {e.Message}");
            return;
        }

        UnityTransport transport =  NetworkManager.Singleton.GetComponent<UnityTransport>();

        RelayServerData relayServerData = AllocationUtils.ToRelayServerData(allocation, "dtls");
        transport.SetRelayServerData(relayServerData);

       


        try 
        {
            CreateLobbyOptions lobbyOptions = new CreateLobbyOptions();
            lobbyOptions.IsPrivate = false;
            lobbyOptions.Data = new Dictionary<string, DataObject>()
            {
                {
                    "JoinCode", new DataObject(
                        visibility: DataObject.VisibilityOptions.Member,
                        value: joinCode
                        )
                }
            };

            Lobby lobby =  await LobbyService.Instance.CreateLobbyAsync(
                PlayerPrefs.GetString(NameSelector.PlayerNamekey, "Missing Name")
                , maxConnections, lobbyOptions);
           
            lobbyId = lobby.Id; 

            HostSingleton.Instance.StartCoroutine(HeatBeatLobby(15f));

        }

        catch (LobbyServiceException e) 
        {
            Debug.LogError($"Error starting host: {e.Message}");
            return;
        }

        networkServer = new NetworkServer(NetworkManager.Singleton);


        UserData userData = new UserData
        {
            UserName = PlayerPrefs.GetString(NameSelector.PlayerNamekey, "Missing Name"),
            UserAuthId = AuthenticationService.Instance.PlayerId
        };

        string payload = JsonUtility.ToJson(userData);

        byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);

        NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;



        NetworkManager.Singleton.StartHost();



        NetworkManager.Singleton.SceneManager.LoadScene(sceneName , LoadSceneMode.Single);


    }


    private IEnumerator HeatBeatLobby(float waitTime) 
    {
        WaitForSecondsRealtime delay = new WaitForSecondsRealtime(waitTime);

        while (true) 
        {
            LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
            yield return delay;
        }
    }

    public async void Dispose()
    {
        HostSingleton.Instance.StopCoroutine(nameof(HeatBeatLobby));

        if(string.IsNullOrEmpty(lobbyId)) 
        {
            try 
            {
                await LobbyService.Instance.DeleteLobbyAsync(lobbyId);
            }
            catch (LobbyServiceException e) 
            {
                Debug.LogError($"Error deleting lobby: {e.Message}");
            }


            lobbyId = string.Empty;
        }

        networkServer?.Dispose();
    }
}
