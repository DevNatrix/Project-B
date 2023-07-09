using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Steamworks;

public class PlayerListItem : MonoBehaviour
{
    public string PlayerName;
    public ulong PlayerSteamID;

    public TextMeshProUGUI PlayerNameText;
    public RawImage PlayerIcon;
    public GameObject PlayerListItemPrefab;
    public GameObject YourTeam;

    CSteamID cSteamId;

    // Start is called before the first frame update
    void Start()
    {

        GameObject newPlayerListItem = Instantiate(PlayerListItemPrefab, YourTeam.transform);
        PlayerListItemUI newPlayerListItemUI = newPlayerListItem.GetComponent<PlayerListItemUI>();

        newPlayerListItemUI.PlayerName.text = SteamHandler.usernameSteam;
        newPlayerListItemUI.PlayerIcon.texture = PlayerIcon.texture;
    }

    public void HandleSteamID(ulong oldSteamId, ulong newSteamId)
    {
        cSteamId = new CSteamID(newSteamId);
        int imageId = SteamFriends.GetLargeFriendAvatar(cSteamId);
        if (imageId == -1) { return; }

        PlayerIcon.texture = GetSteamImageAsTexture2D(imageId);

    }

    public static Texture2D GetSteamImageAsTexture2D(int iImage)
    {
        Texture2D ret = null;
        uint ImageWidth;
        uint ImageHeight;
        bool bIsValid = SteamUtils.GetImageSize(iImage, out ImageWidth, out ImageHeight);

        if (bIsValid)
        {
            byte[] Image = new byte[ImageWidth * ImageHeight * 4];

            bIsValid = SteamUtils.GetImageRGBA(iImage, Image, (int)(ImageWidth * ImageHeight * 4));
            if (bIsValid)
            {
                ret = new Texture2D((int)ImageWidth, (int)ImageHeight, TextureFormat.RGBA32, false, true);
                ret.LoadRawTextureData(Image);
                ret.Apply();
            }
        }

        return ret;
    }
}
