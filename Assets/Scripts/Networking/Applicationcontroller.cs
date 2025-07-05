using System.Threading.Tasks;
using UnityEngine;

public class Applicationcontroller : MonoBehaviour
{
    [SerializeField] private ClientSingleton clientPrefab;

    [SerializeField] private HostSingleton hostPrefab;
    private  async void Start()
    {

        DontDestroyOnLoad(gameObject);

        await LaunchInMode(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null);
    }

    private async Task LaunchInMode(bool isDedicatedServer) 
    {
        if(isDedicatedServer)
        {
            // Launch in dedicated server mode
            Debug.Log("Launching in Dedicated Server Mode");
            // Add your dedicated server initialization logic here
        }
        else
        {


            HostSingleton hostSingleton = Instantiate(hostPrefab);
            hostSingleton.CreateHost();



            ClientSingleton clientSingleton = Instantiate(clientPrefab);
            bool auhtenticated  =  await clientSingleton.CreateClient();


            // Launch in client mode
            Debug.Log("Launching in Client Mode");
            // Add your client initialization logic here



            if (auhtenticated)
            {
                clientSingleton.clientGameManager.GoToMainMenu();
            }
        }
    }

 
}
