using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class SteamHandler : MonoBehaviour
{
    bool steam_initialized = SteamAPI.Init();

    public string username;
    // Start is called before the first frame update
    void Start()
    {
        SteamAPI.Init();

        DontDestroyOnLoad(this.gameObject);

        if (!steam_initialized)
        {
            Debug.LogError("Steam is not opened, please open Steam");
        }
    }

    // Update is called once per frame
    void Update()
    {
        GetSteamUsername();
    }

    public void GetSteamUsername()
    {
        CSteamID SteamID = SteamUser.GetSteamID();
        string SteamName = SteamFriends.GetPersonaName();
        username = SteamName;
    }
}
