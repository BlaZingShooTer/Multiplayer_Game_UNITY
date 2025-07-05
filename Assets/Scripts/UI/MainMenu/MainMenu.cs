using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TMP_InputField joinCodeInputField;
    public async void StartHost() 
    {
        await HostSingleton.Instance.hostGameManager.StartHostAsync();
    }

    public async void StartClient() 
    {
        await ClientSingleton.Instance.clientGameManager.StartClientAsync(joinCodeInputField.text);
    }
}
