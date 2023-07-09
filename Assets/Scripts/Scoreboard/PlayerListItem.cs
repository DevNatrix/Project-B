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

    // Start is called before the first frame update
    void Start()
    {

        GameObject newPlayerListItem = Instantiate(PlayerListItemPrefab, YourTeam.transform);
        PlayerListItemUI newPlayerListItemUI = newPlayerListItem.GetComponent<PlayerListItemUI>();

        newPlayerListItemUI.PlayerName.text = SteamHandler.usernameSteam;
    }
}
