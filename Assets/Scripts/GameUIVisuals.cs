using UnityEngine;
using Steamworks;
using UnityEngine.UI;
using System;

public class GameUIVisuals : MonoBehaviour
{
    // Making the GameUIVisuals a Singleton
    public static GameUIVisuals uiVisualsInstance;
    public static Canvas uiInstance;
    // This is to have the DebugLog stuff be replaced with it updating on the UI.
    private static GameObject _outputLogInstance;
    // This is to prevent a potential issue with the Username not updating due to grabbing the wrong text.
    private static GameObject _userNameInstance;
    // This is needed in order to grab the UI within the level to be used in the world.
    private void Awake()
    {
        // Reused code from GameManager in order to make this a singleton.
        if (uiVisualsInstance == null)
        {
            uiVisualsInstance = this;
            // This here allows the Manager to not be destroyed on Load of a Scene.
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        uiInstance = gameObject.GetComponent<Canvas>();
        // This searches the entire scene to find the first object with the attached Tag.
        _outputLogInstance = GameObject.FindWithTag("UILog");
        // This does same as above but for the Username section.
        _userNameInstance = GameObject.FindWithTag("UIUsername");
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // This takes the Steam Username and sends it to Update Name to apply to UI
        if (SteamManager.Initialized)
        {
            // This grabs the player name via "PersonaName", the other option Nickname is Depreciated.
            // Same as Get Steam Image, it has been combined into 1 function to allow for reuse within Runtime.
            UpdateName();
            
            // This activates the function that does all the processes in order to update the profile picture.
            GetSteamImage();
        }
        // Testing Purposes.
        UpdateLog("Testing 123");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // This takes the string supplied and updates the UI name tag at the top left of the screen. (Same as Steam Image, Public to allow for use with UI Buttons)
    public void UpdateName()
    {
        string playerSteamUsername = SteamFriends.GetPersonaName();
        _userNameInstance.gameObject.GetComponent<Text>().text = playerSteamUsername;
        UpdateLog($"Steam Username Updated: " + playerSteamUsername);
    } 

    // This function processes all the required dependances in order to output the user's profile picture to the UI. (Public allows the button to reuse this)
    public void GetSteamImage()
    {
        // This section grabs the core info and data for the image
        Texture2D userAvatarTexture = null;
        uint ImageWidth;
        uint ImageHeight;
        // Removed from the core start function and brought it into the function.
        int playerAvatarID = SteamFriends.GetLargeFriendAvatar(SteamUser.GetSteamID());
        // This checks if the info gathered is valid ready for the rest of the function to happen.
        bool IsValid = SteamUtils.GetImageSize(playerAvatarID, out ImageWidth, out ImageHeight);

        if (IsValid)
        {
            // This creates the struct ready to house the bytes of info that would be the info in RGBA (Hence the 4x)
            byte[] Image = new byte[ImageWidth * ImageHeight * 4];

            // Doing another valid check to see if the RGBA returns a valid response, if not it won't proceed.
            IsValid = SteamUtils.GetImageRGBA(playerAvatarID, Image, (int)(ImageWidth * ImageHeight * 4));

            if (IsValid)
            {
                // This takes all the info and converts it from byte/Steam API to Texture2D to allow for Sprite Creation.
                userAvatarTexture = new Texture2D((int)ImageWidth, (int)ImageHeight, TextureFormat.RGBA32, false, true);
                userAvatarTexture.LoadRawTextureData(Image);
                userAvatarTexture.Apply();

                // Converting to Sprite for UI (Creating a new Sprite First)
                Sprite avatarSprite = Sprite.Create(userAvatarTexture, new Rect(0, 0, userAvatarTexture.width, userAvatarTexture.height), new Vector2(userAvatarTexture.width / 2, userAvatarTexture.height / 2));
                // Overrides the UI Sprite currently in the level with the new texture (Now converted to Sprite) version
                uiInstance.GetComponentInChildren<Image>().overrideSprite = avatarSprite;
                // This rotates the image by 180.0f on the X axis as it is flipped when left on default.
                uiInstance.GetComponentInChildren<Image>().transform.rotation = new Quaternion(180.0f, 0.0f, 0.0f, 0.0f);
            }
        }

    }

    // This function is to passthrough the string and output it to the UI instead of the Debug Log like before.
    public void UpdateLog(string desc)
    {
        // Using the Tag reference, it is able to grab the component of text and update it with the passed through description.
        _outputLogInstance.gameObject.GetComponent<Text>().text = desc;
    }
}
