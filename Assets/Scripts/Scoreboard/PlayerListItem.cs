using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Steamworks;

public class PlayerListItem : MonoBehaviour
{
    //public Texture2D PlayerIcon;
    //public TextMeshProUGUI PingText;
    //public TextMeshProUGUI PlayerNameText;
    public GameObject PlayerListItemPrefab;
    public static PlayerListItemUI playerListItemUI;
    public Transform YourTeam;
    public Transform EnemyTeam;

	//CSteamID cSteamId;

	// Start is called before the first frame update

	void Start()
    {
		//cSteamId = SteamUser.GetSteamID();

		//HandleSteamID((ulong)cSteamId);
		playerListItemUI = Instantiate(PlayerListItemPrefab, YourTeam).GetComponent<PlayerListItemUI>();
		playerListItemUI.setStartInfo("you");
    }

    /*public void HandleSteamID(ulong newSteamId)
    {
        cSteamId = new CSteamID(newSteamId);
        int imageId = SteamFriends.GetLargeFriendAvatar(cSteamId);
        if (imageId == -1) { return; }

        PlayerIcon = GetSteamImageAsTexture2D(imageId);
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
    }*/
}