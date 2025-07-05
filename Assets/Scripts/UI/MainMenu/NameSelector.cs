using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NameSelector : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInputField;

    [SerializeField] private Button connectButton;

    [SerializeField] private int minNameLength = 1;

    [SerializeField] private int maxNameLength = 30;

    public const string PlayerNamekey = "PlayerName";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            return;
        }
  
        

        nameInputField.text =  PlayerPrefs.GetString(PlayerNamekey, string.Empty);

        HandleNameChanged();
    }


    public void HandleNameChanged() 
    {

        connectButton.interactable = nameInputField.text.Length >= minNameLength && 
                                      nameInputField.text.Length <= maxNameLength;
    }


    public void Connect()
    {
        PlayerPrefs.SetString(PlayerNamekey, nameInputField.text);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

}
