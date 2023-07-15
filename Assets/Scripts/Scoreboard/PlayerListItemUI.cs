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

    void OnEnable()
    {
        StartCoroutine(UpdatePing());
    }

    private void Update()
    {
        PlayerName.text = SteamHandler.usernameSteam;
    }

    IEnumerator UpdatePing()
    {
        PingText.text = UDPServer.latency.ToString() + "ms";
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
