using System.Threading.Tasks;
using UnityEngine;

public class ClientSingleton : MonoBehaviour
{
    private static ClientSingleton instance;

    public  ClientGameManager clientGameManager { get; private set; } 
    public static ClientSingleton Instance
    {
        get
        {
            if(instance != null) 
            {
                return instance;
            }
            
            instance = Object.FindFirstObjectByType<ClientSingleton>();

            if(instance == null)
            {
                return null; 
            }

            return instance;
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }


    public async Task<bool> CreateClient() 
    {
        clientGameManager = new ClientGameManager();

        return await clientGameManager.InitAsync();
    }

    private void OnDestroy()
    {
        clientGameManager?.Dispose();
    }
}
