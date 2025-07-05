using UnityEngine;
using TMPro;
using Unity.Collections;
using System;

public class PlayerNameDIsplay : MonoBehaviour
{
    [SerializeField] private TankPlayer player;

    [SerializeField] private TMP_Text playerNameText;

    // Update is called once per frame
    private void Start()
    {
        UpdatePlayerNameText(string.Empty, player.PlayerName.Value);

        player.PlayerName.OnValueChanged += UpdatePlayerNameText;
    }

    private void UpdatePlayerNameText(FixedString32Bytes previousValue, FixedString32Bytes newValue)
    {
       playerNameText.text = newValue.ToString();
    }

    private void OnDestroy()
    {
        player.PlayerName.OnValueChanged -= UpdatePlayerNameText;
    }
}
