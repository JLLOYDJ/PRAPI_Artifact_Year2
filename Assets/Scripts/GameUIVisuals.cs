using UnityEngine;
using Steamworks;
using UnityEngine.UI;
using System;

public class GameUIVisuals : MonoBehaviour
{
    public static Canvas uiInstance;
    private string playerSteamUsername;
    private int playerAvatarID;
    private Texture2D playerImage;
    private bool playerAvatarBool;
    private UInt32 widthAvatar;
    private UInt32 heightAvatar;

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
            playerSteamUsername = SteamFriends.GetPersonaName();
            UpdateName(playerSteamUsername);

            // Function to get the Texture 2D in order to pass it to the in game implementation.
            // To passthrough the avatar/image
            playerAvatarID = SteamFriends.GetLargeFriendAvatar(SteamUser.GetSteamID());

            playerImage = GetSteamImage(playerAvatarID);

            

            uiInstance.GetComponentInChildren<Image>().overrideSprite = Sprite.Create(playerImage, new Rect(0, 0, playerImage.width, playerImage.height), new Vector2(playerImage.width / 2, playerImage.height / 2));
            // Rotating the Image to ensure it is the right way up.
            uiInstance.GetComponentInChildren<Image>().transform.rotation = new Quaternion(180.0f, 0.0f, 0.0f, 0.0f);
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

    Texture2D GetSteamImage(int iImage)
    {
        Texture2D ret = null;
        uint ImageWidth;
        uint ImageHeight;
        bool IsValid = SteamUtils.GetImageSize(iImage, out ImageWidth, out ImageHeight);

        if (IsValid)
        {
            byte[] Image = new byte[ImageWidth * ImageHeight * 4];

            IsValid = SteamUtils.GetImageRGBA(iImage, Image, (int)(ImageWidth * ImageHeight * 4));

            if (IsValid)
            {
                ret = new Texture2D((int)ImageWidth, (int)ImageHeight, TextureFormat.RGBA32, false, true);
                ret.LoadRawTextureData(Image);
                ret.Apply();
            }
        }
        return ret;

    }
}
