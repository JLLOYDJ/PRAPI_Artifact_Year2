using UnityEngine;
using Steamworks;
using UnityEngine.UI;
using System;

public class GameUIVisuals : MonoBehaviour
{
    public static Canvas uiInstance;
    private string playerSteamUsername;
    // This is needed in order to grab the UI within the level to be used in the world.
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
            // This grabs the player name via "PersonaName", the other option Nickname is Depreciated.
            // Same as Get Steam Image, it has been combined into 1 function to allow for reuse within Runtime.
            UpdateName();
            
            // This activates the function that does all the processes in order to update the profile picture.
            GetSteamImage();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // This takes the string supplied and updates the UI name tag at the top left of the screen. (Same as Steam Image, Public to allow for use with UI Buttons)
    public void UpdateName()
    {
        string playerSteamUsername = SteamFriends.GetPersonaName();
        gameObject.GetComponentInChildren<Text>().text = playerSteamUsername;
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
}
