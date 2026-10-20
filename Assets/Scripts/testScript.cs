using UnityEngine;
using Steamworks;
// This is a testing script to make sure I am doing this correctly + checking functionality of Steam SDK.
public class testScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // This checks if SteamManager is Initialized, it doesn't need to be found in the level in order to access.
        // Requires "Using Steamworks" to be in the script in order for this to work.
        if (SteamManager.Initialized) 
        {
            // Here it goes into the SteamFriends API and accesses the User's Profile Name
            // In my case for my Steam Account, it would be TrickfireRAGE
            string name = SteamFriends.GetPersonaName();
            Debug.Log(name);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
