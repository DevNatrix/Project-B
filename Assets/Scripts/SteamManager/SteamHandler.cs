using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class SteamHandler : MonoBehaviour
{
    bool steam_initialized = SteamAPI.Init();

    public static string usernameSteam;
    public static CSteamID SteamID;
    // Start is called before the first frame update
    void Start()
    {
        SteamAPI.Init();

        DontDestroyOnLoad(this);

        if (!steam_initialized)
        {
            Debug.LogError("Steam is not opened, please open Steam");
            usernameSteam = "guest" + Random.Range(0, 9999);
        }
    }

    // Update is called once per frame
    void Update()
    {
        GetSteamValues();
    }

    public void GetSteamValues()
    {
        string SteamName = SteamFriends.GetPersonaName();
        usernameSteam = SteamName;

        SteamID = SteamUser.GetSteamID();
    }
}
