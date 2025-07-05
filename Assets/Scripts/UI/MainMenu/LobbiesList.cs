using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbiesList : MonoBehaviour
{
    [SerializeField] private LobbyItem lobbyItemPrefab;

    [SerializeField] private Transform lobbyItemParent;

    private bool isJoining;

    private bool isRefreshing;

    private void OnEnable()
    {
        RefreshList();
    }


    public async void RefreshList()
    {
        if (isRefreshing) 
        {
            return;
        }

        isRefreshing = true;


        try 
        {
                QueryLobbiesOptions options = new QueryLobbiesOptions();
                options.Count = 20;

                options.Filters = new List<QueryFilter>()
                {

                new QueryFilter(
                    field: QueryFilter.FieldOptions.AvailableSlots,
                    op: QueryFilter.OpOptions.GT,
                    value: "0"
                    ),

                new QueryFilter(
                    field: QueryFilter.FieldOptions.IsLocked,
                    op: QueryFilter.OpOptions.EQ,
                    value: "0"
                    )
                };

                QueryResponse lobbies =  await LobbyService.Instance.QueryLobbiesAsync(options);

            foreach (Transform child in lobbyItemParent)
            {
                Destroy(child.gameObject);
            }

            foreach (Lobby lobby in lobbies.Results)
            {
                LobbyItem lobbyItem = Instantiate(lobbyItemPrefab, lobbyItemParent);
                lobbyItem.Initialise(this, lobby);
            }

        }

        catch (LobbyServiceException e)
        {
            Debug.LogError($"Error refreshing lobbies: {e.Message}");
        }

       


        isRefreshing = true;
    }

    public async void JoinAsync(Lobby lobby)
    {

        if (isJoining)
        {
            return;
        }


        isJoining = true;

        try 
        {


            Lobby joiningLobby  = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id);
            string joinCode = joiningLobby.Data["JoinCode"].Value;

            await ClientSingleton.Instance.clientGameManager.StartClientAsync(joinCode);
        }
        catch(LobbyServiceException e)
        {
            Debug.Log($"Error joining lobby: {e.Message}");

        }

        isJoining = false;
    }
}
