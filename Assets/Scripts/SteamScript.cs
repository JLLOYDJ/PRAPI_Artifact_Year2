using UnityEngine;
using Steamworks;

// The Following script is from the Steamworks.NET documentation

public class SteamScript : MonoBehaviour
{
    void Start()
    {
        if (SteamManager.Initialized)
        {
            // This grabs the User ID Name and puts it as a string that can be used in the game.
            // AKA this can be a great idea for the users name within the game.
            string name = SteamFriends.GetPersonaName();
            Debug.Log(name);
            
        }
    }

}
