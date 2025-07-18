using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class LeaderBoardEntities : MonoBehaviour
{
    [SerializeField] private TMP_Text displaytext;
    [SerializeField] private Color myColor;

    private FixedString32Bytes playerName;
    public ulong ClientId {get; private set; }

    public int Coins { get; private set; }

    public void Initialise(ulong clientId, FixedString32Bytes playerName, int coins) 
    {
        ClientId = clientId;
        this.playerName = playerName;
        
        if(clientId == NetworkManager.Singleton.LocalClientId) 
        {
            displaytext.color = myColor;
        }

        UpdateCoins(coins);
    }


    public void UpdateCoins(int coins) 
    {
        Coins = coins;
        UpdateText();
    }

    public void UpdateText() 
    {
        displaytext.text = $"{transform.GetSiblingIndex() + 1}. {playerName} ({Coins})";
    }
}
