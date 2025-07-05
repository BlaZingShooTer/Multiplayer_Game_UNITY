using UnityEngine;

public class HostSingleton : MonoBehaviour
{
    private static HostSingleton instance;

    public HostGameManager hostGameManager { get; private set; }
    public static HostSingleton Instance
    {
        get
        {
            if (instance != null)
            {
                return instance;
            }

            instance = Object.FindFirstObjectByType<HostSingleton>();

            if (instance == null)
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


    public void CreateHost()
    {
        hostGameManager = new HostGameManager();   
    }


    private void OnDestroy()
    {
        hostGameManager?.Dispose();
    }
}
