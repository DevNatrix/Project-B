using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerListItemUI : MonoBehaviour
{
    public TextMeshProUGUI PlayerName;
    public RawImage PlayerIcon;
    public TextMeshProUGUI PingText;
    public TextMeshProUGUI DisplayKills;

    public int playerKills, playerDeaths, playerAssists = 0;

    void OnEnable()
    {
        StartCoroutine(UpdatePing());
    }

    private void Update()
    {
        PlayerName.text = SteamHandler.usernameSteam;

        DisplayKills.text = playerKills.ToString() + "/" + playerDeaths.ToString() + "/" + playerAssists.ToString();
    }

    IEnumerator UpdatePing()
    {
        PingText.text = Client.latency.ToString() + "ms";
        yield return new WaitForSeconds(0.5f);
        Debug.Log("Updated");
        StartCoroutine(UpdatePing());
    }

    public void Setup(string _PlayerName, Texture2D _PlayerIcon)
    {
        PlayerName.text = _PlayerName;
        PlayerIcon.texture = _PlayerIcon;
    }
}
