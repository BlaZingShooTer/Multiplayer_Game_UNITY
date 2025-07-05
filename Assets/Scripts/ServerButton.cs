using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ServerButton : MonoBehaviour
{
    [SerializeField] private Button client_button;
    [SerializeField] private Button host_button;



    private void Awake()
    {
        client_button.onClick.AddListener(() => 
        {
            Debug.Log("Client button clicked!");
            NetworkManager.Singleton.StartClient();
        });

        host_button.onClick.AddListener(() => 
        {
            Debug.Log("Host button clicked!"); 
            NetworkManager.Singleton.StartHost();
        });
    }
}
