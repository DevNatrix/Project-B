using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerListItemUI : MonoBehaviour
{

    public TextMeshProUGUI PlayerName;
    public RawImage PlayerIcon;

    public PlayerListItemUI Instance;

    public void Setup(string _PlayerName, Texture2D _PlayerIcon)
    {
        PlayerName.text = _PlayerName;
        PlayerIcon.texture = _PlayerIcon;
    }
}
