using UnityEngine;
using Steamworks; // To have access to the manager in order to create it without requiring it in the scene.

public class GameManager : MonoBehaviour
{
    // This section creates the static references needed to access these objects within the level without duplicates.
    public static SteamManager SteamInstance;
    public static GameManager GameInstance;
    
    // Using Awake for things that need to be loaded before the engine is ready for the object.
    private void Awake() // Keeps it out of the public access allowing it to function without issues.
    {
        // Ensuring there is only 1 Game Manager within the Project
        if (GameInstance == null)
        {
            GameInstance = this;
            // This here allows the Manager to not be destroyed on Load of a Scene.
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }    

        // Steamworks.NET API creation/initialisation within the scene.
        if (SteamInstance != null) // This checks if the newInstance has something within it.
        {
            Debug.Log("Steam Manager is Already Active");
        }
        else if (FindAnyObjectByType<SteamManager>() == true) // This checks if there is a Steam Manager already in the scene
        {
            Debug.Log("Steam Manager found, bypassing creation and setting Manager to Variable.");
            SteamInstance = FindAnyObjectByType<SteamManager>();
        }
        else // If nothing else, it will create a new one.
        {
            Debug.Log("No Steam Manager Present");
            SteamInstance = new GameObject("SteamManager").AddComponent<SteamManager>();
        }

    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (SteamManager.Initialized)
        {
            string name = SteamFriends.GetPersonaName();
            Debug.Log(name);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
