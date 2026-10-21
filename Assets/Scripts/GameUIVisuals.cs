using UnityEngine;
using Steamworks;
using UnityEngine.UI;

public class GameUIVisuals : MonoBehaviour
{
    public static Canvas uiInstance;
    private string playerSteamUsername;

    private void Awake()
    {
        uiInstance = gameObject.GetComponent<Canvas>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // This takes the Steam Username and sends it to Update Name to apply to UI
        if (SteamManager.Initialized)
        {
            playerSteamUsername = SteamFriends.GetPersonaName();
            UpdateName(playerSteamUsername);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // This takes the string supplied and updates the UI name tag at the top left of the screen.
    void UpdateName(string name)
    {
        gameObject.GetComponentInChildren<Text>().text = name;
    }
}
